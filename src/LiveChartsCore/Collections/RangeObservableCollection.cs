﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

// https://stackoverflow.com/questions/670577/observablecollection-doesnt-support-addrange-method-so-i-get-notified-for-each
// https://gist.github.com/weitzhandler/65ac9113e31d12e697cb58cd92601091
// https://github.com/dotnet/runtime/issues/18087#issuecomment-359197102
// namespace System.Collections.ObjectModel
// {
namespace LiveChartsCore.Collections // we use this namespace, because .Net might officially include it in the framework
{

    /// <summary>
    /// Implementation of a dynamic data collection based on generic Collection&lt;T&gt;,
    /// implementing INotifyCollectionChanged to notify listeners
    /// when items get added, removed or the whole list is refreshed.
    /// </summary>
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields    
        [NonSerialized]
        private DeferredEventsCollection? _deferredEvents;
        #endregion Private Fields


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors
        /// <summary>
        /// Initializes a new instance of ObservableCollection that is empty and has default initial capacity.
        /// </summary>
        public RangeObservableCollection() { }

        /// <summary>
        /// Initializes a new instance of the ObservableCollection class that contains
        /// elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the collection.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
        public RangeObservableCollection(IEnumerable<T> collection) : base(collection) { }

        /// <summary>
        /// Initializes a new instance of the ObservableCollection class
        /// that contains elements copied from the specified list
        /// </summary>
        /// <param name="list">The list whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the list.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> list is a null reference </exception>
        public RangeObservableCollection(List<T> list) : base(list) { }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties
        EqualityComparer<T>? _Comparer;

        /// <summary>
        /// ...
        /// </summary>
        public EqualityComparer<T> Comparer
        {
            get => _Comparer ??= EqualityComparer<T>.Default;
            private set => _Comparer = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this collection acts as a <see cref="HashSet{T}"/>,
        /// disallowing duplicate items, based on <see cref="Comparer"/>.
        /// This might indeed consume background performance, but in the other hand,
        /// it will pay off in UI performance as less required UI updates are required.
        /// </summary>
        public bool AllowDuplicates { get; set; } = true;

        #endregion Public Properties

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the <see cref="ObservableCollection{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void AddRange(IEnumerable<T> collection)
        {
            InsertRange(Count, collection);
        }

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="ObservableCollection{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the list.
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not in the collection range.</exception>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (index > Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (!AllowDuplicates)
                collection =
                  collection
                  .Distinct(Comparer)
                  .Where(item => !Items.Contains(item, Comparer))
                  .ToList();

            if (collection is ICollection<T> countable)
            {
                if (countable.Count == 0)
                    return;
            }
            else if (!collection.Any())
                return;

            CheckReentrancy();

            //expand the following couple of lines when adding more constructors.
            var target = (List<T>)Items;
            target.InsertRange(index, collection);

            OnEssentialPropertiesChanged();

            if (!(collection is IList list))
                list = new List<T>(collection);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index));
        }


        /// <summary> 
        /// Removes the first occurrence of each item in the specified collection from the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection">The items to remove.</param>        
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void RemoveRange(IEnumerable<T> collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            if (Count == 0)
                return;
            else if (collection is ICollection<T> countable)
            {
                if (countable.Count == 0)
                    return;
                if (countable.Count == 1)
                {
                    using var enumerator = countable.GetEnumerator();
                    _ = enumerator.MoveNext();
                    _ = Remove(enumerator.Current);
                    return;
                }
            }
            else if (!collection.Any())
                return;

            CheckReentrancy();

            var clusters = new Dictionary<int, List<T>>();
            var lastIndex = -1;
            List<T>? lastCluster = null;
            foreach (var item in collection)
            {
                var index = IndexOf(item);
                if (index < 0)
                    continue;

                Items.RemoveAt(index);

                if (lastIndex == index && lastCluster is not null)
                    lastCluster.Add(item);
                else
                    clusters[lastIndex = index] = lastCluster = new List<T> { item };
            }

            OnEssentialPropertiesChanged();

            if (Count == 0)
                OnCollectionReset();
            else
                foreach (var cluster in clusters)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster.Value, cluster.Key));

        }

        /// <summary>
        /// Iterates over the collection and removes all items that satisfy the specified match.
        /// </summary>
        /// <remarks>The complexity is O(n).</remarks>
        /// <param name="match"></param>
        /// <returns>Returns the number of elements that where </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public int RemoveAll(Predicate<T> match)
        {
            return RemoveAll(0, Count, match);
        }

        /// <summary>
        /// Iterates over the specified range within the collection and removes all items that satisfy the specified match.
        /// </summary>
        /// <remarks>The complexity is O(n).</remarks>
        /// <param name="index">The index of where to start performing the search.</param>
        /// <param name="count">The number of items to iterate on.</param>
        /// <param name="match"></param>
        /// <returns>Returns the number of elements that where </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is out of range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public int RemoveAll(int index, int count, Predicate<T> match)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (index + count > Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (match is null)
                throw new ArgumentNullException(nameof(match));

            if (Count == 0)
                return 0;

            List<T>? cluster = null;
            var clusterIndex = -1;
            var removedCount = 0;

            using (BlockReentrancy())
            using (DeferEvents())
            {
                for (var i = 0; i < count; i++, index++)
                {
                    var item = Items[index];
                    if (match(item))
                    {
                        Items.RemoveAt(index);
                        removedCount++;

                        if (clusterIndex == index)
                        {
                            Debug.Assert(cluster is not null);
                            cluster!.Add(item);
                        }
                        else
                        {
                            cluster = new List<T> { item };
                            clusterIndex = index;
                        }

                        index--;
                    }
                    else if (clusterIndex > -1)
                    {
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
                        clusterIndex = -1;
                        cluster = null;
                    }
                }

                if (clusterIndex > -1)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
            }

            if (removedCount > 0)
                OnEssentialPropertiesChanged();

            return removedCount;
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="ObservableCollection{T}"/>>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified range is exceeding the collection.</exception>
        public void RemoveRange(int index, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (index + count > Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count == 0)
                return;

            if (count == 1)
            {
                RemoveItem(index);
                return;
            }

            //Items will always be List<T>, see constructors
            var items = (List<T>)Items;
            var removedItems = items.GetRange(index, count);

            CheckReentrancy();

            items.RemoveRange(index, count);

            OnEssentialPropertiesChanged();

            if (Count == 0)
                OnCollectionReset();
            else
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, index));
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified collection,
        /// using <see cref="Comparer"/>.
        /// </summary>             
        /// <param name="collection">The items to fill the collection with, after clearing it.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void ReplaceRange(IEnumerable<T> collection)
        {
            ReplaceRange(0, Count, collection);
        }

        /// <summary>
        /// Removes the specified range and inserts the specified collection in its position, leaving equal items in equal positions intact.
        /// </summary>
        /// <param name="index">The index of where to start the replacement.</param>
        /// <param name="count">The number of items to be replaced.</param>
        /// <param name="collection">The collection to insert in that location.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is out of range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void ReplaceRange(int index, int count, IEnumerable<T> collection)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (index + count > Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            if (!AllowDuplicates)
                collection =
                  collection
                  .Distinct(Comparer)
                  .ToList();

            if (collection is ICollection<T> countable)
            {
                if (countable.Count == 0)
                {
                    RemoveRange(index, count);
                    return;
                }
            }
            else if (!collection.Any())
            {
                RemoveRange(index, count);
                return;
            }

            if (index + count == 0)
            {
                InsertRange(0, collection);
                return;
            }

            if (!(collection is IList<T> list))
                list = new List<T>(collection);

            using (BlockReentrancy())
            using (DeferEvents())
            {
                var rangeCount = index + count;
                var addedCount = list.Count;

                var changesMade = false;
                List<T>?
                  newCluster = null,
                  oldCluster = null;


                var i = index;
                for (; i < rangeCount && i - index < addedCount; i++)
                {
                    //parallel position
                    T old = this[i], @new = list[i - index];
                    if (Comparer.Equals(old, @new))
                    {
                        OnRangeReplaced(i, newCluster!, oldCluster!);
                        continue;
                    }
                    else
                    {
                        Items[i] = @new;

                        if (newCluster is null)
                        {
                            Debug.Assert(oldCluster is null);
                            newCluster = new List<T> { @new };
                            oldCluster = new List<T> { old };
                        }
                        else
                        {
                            newCluster.Add(@new);
                            oldCluster!.Add(old);
                        }

                        changesMade = true;
                    }
                }

                OnRangeReplaced(i, newCluster!, oldCluster!);

                //exceeding position
                if (count != addedCount)
                {
                    var items = (List<T>)Items;
                    if (count > addedCount)
                    {
                        var removedCount = rangeCount - addedCount;
                        var removed = new T[removedCount];
                        items.CopyTo(i, removed, 0, removed.Length);
                        items.RemoveRange(i, removedCount);
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, i));
                    }
                    else
                    {
                        var k = i - index;
                        var added = new T[addedCount - k];
                        for (var j = k; j < addedCount; j++)
                        {
                            var @new = list[j];
                            added[j - k] = @new;
                        }
                        items.InsertRange(i, added);
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added, i));
                    }

                    OnEssentialPropertiesChanged();
                }
                else if (changesMade)
                {
                    OnIndexerPropertyChanged();
                }
            }
        }

        #endregion Public Methods


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// Called by base class Collection&lt;T&gt; when the list is being cleared;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void ClearItems()
        {
            if (Count == 0)
                return;

            CheckReentrancy();
            base.ClearItems();
            OnEssentialPropertiesChanged();
            OnCollectionReset();
        }

        /// <inheritdoc/>
        protected override void InsertItem(int index, T item)
        {
            if (!AllowDuplicates && Items.Contains(item))
                return;

            base.InsertItem(index, item);
        }

        /// <inheritdoc/>
        protected override void SetItem(int index, T item)
        {
            if (AllowDuplicates)
            {
                if (Comparer.Equals(this[index], item))
                    return;
            }
            else
              if (Items.Contains(item, Comparer))
                return;

            CheckReentrancy();
            var oldItem = this[index];
            base.SetItem(index, item);

            OnIndexerPropertyChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem!, item!, index);
        }

        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this ObservableCollection will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        /// <remarks>
        /// When overriding this method, either call its base implementation
        /// </remarks>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_deferredEvents is not null)
            {
                _deferredEvents.Add(e);
                return;
            }
            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual IDisposable DeferEvents()
        {
            return new DeferredEventsCollection(this);
        }

        #endregion Protected Methods


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        /// <summary>
        /// Helper to raise Count property and the Indexer property.
        /// </summary>
        void OnEssentialPropertiesChanged()
        {
            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnIndexerPropertyChanged();
        }

        /// <summary>
        /// /// Helper to raise a PropertyChanged event for the Indexer property
        /// /// </summary>
        void OnIndexerPropertyChanged()
        {
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
        }

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners
        /// </summary>
        void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event with action == Reset to any listeners
        /// </summary>
        void OnCollectionReset()
        {
            OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
        }

        /// <summary>
        /// Helper to raise event for clustered action and clear cluster.
        /// </summary>
        /// <param name="followingItemIndex">The index of the item following the replacement block.</param>
        /// <param name="newCluster"></param>
        /// <param name="oldCluster"></param>
        //TODO should have really been a local method inside ReplaceRange(int index, int count, IEnumerable<T> collection, IEqualityComparer<T> comparer),
        //move when supported language version updated.
        void OnRangeReplaced(int followingItemIndex, ICollection<T> newCluster, ICollection<T> oldCluster)
        {
            if (oldCluster is null || oldCluster.Count == 0)
            {
                Debug.Assert(newCluster is null || newCluster.Count == 0);
                return;
            }

            OnCollectionChanged(
              new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Replace,
                new List<T>(newCluster),
                new List<T>(oldCluster),
                followingItemIndex - oldCluster.Count));

            oldCluster.Clear();
            newCluster.Clear();
        }

        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Private Types
        //
        //------------------------------------------------------

        #region Private Types
        sealed class DeferredEventsCollection : List<NotifyCollectionChangedEventArgs>, IDisposable
        {
            readonly RangeObservableCollection<T> _collection;
            public DeferredEventsCollection(RangeObservableCollection<T> collection)
            {
                Debug.Assert(collection is not null);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Debug.Assert(collection._deferredEvents is null);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _collection = collection;
                _collection._deferredEvents = this;
            }

            public void Dispose()
            {
                _collection._deferredEvents = null;
                foreach (var args in this)
                    _collection.OnCollectionChanged(args);
            }
        }

        #endregion Private Types

    }

    /// <remarks>
    /// To be kept outside <see cref="ObservableCollection{T}"/>, since otherwise, a new instance will be created for each generic type used.
    /// </remarks>
    internal static class EventArgsCache
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs("Item[]");
        internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }
}
