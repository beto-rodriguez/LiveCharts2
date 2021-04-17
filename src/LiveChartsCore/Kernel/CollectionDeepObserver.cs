// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// A helper class that tracks both, <see cref="INotifyCollectionChanged.CollectionChanged"/> and 
    /// <see cref="INotifyPropertyChanged.PropertyChanged"/> events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionDeepObserver<T>
    {
        private readonly NotifyCollectionChangedEventHandler onCollectionChanged;
        private readonly PropertyChangedEventHandler onItemPropertyChanged;
        private readonly HashSet<INotifyPropertyChanged> itemsListening = new();

        /// <summary>
        /// The check i notify property changed
        /// </summary>
        protected bool checkINotifyPropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionDeepObserver{T}"/> class.
        /// </summary>
        /// <param name="onCollectionChanged">The on collection changed handler.</param>
        /// <param name="onItemPropertyChanged">The on item property changed handler.</param>
        /// <param name="checkINotifyPropertyChanged">if true, it will force the check to verify if the type {T} implements INotifyPropertyChanged.</param>
        public CollectionDeepObserver(
            NotifyCollectionChangedEventHandler onCollectionChanged,
            PropertyChangedEventHandler onItemPropertyChanged,
            bool? checkINotifyPropertyChanged = null)
        {
            this.onCollectionChanged = onCollectionChanged;
            this.onItemPropertyChanged = onItemPropertyChanged;

            if (checkINotifyPropertyChanged != null)
            {
                this.checkINotifyPropertyChanged = checkINotifyPropertyChanged.Value;
                return;
            }

            this.checkINotifyPropertyChanged = typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T));
        }

        /// <summary>
        /// Initializes the listeners.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public void Initialize(IEnumerable<T>? instance)
        {
            if (instance == null) return;

            if (instance is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += OnCollectionChanged;
            }

            if (checkINotifyPropertyChanged)
                foreach (var item in GetINotifyPropertyChangedItems(instance))
                    item.PropertyChanged += onItemPropertyChanged;
        }

        /// <summary>
        /// Disposes the listeners.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public void Dispose(IEnumerable<T>? instance)
        {
            if (instance == null) return;

            if (instance is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged -= OnCollectionChanged;
            }

            if (checkINotifyPropertyChanged)
                foreach (var item in GetINotifyPropertyChangedItems(instance))
                    item.PropertyChanged -= onItemPropertyChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (checkINotifyPropertyChanged)
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in GetINotifyPropertyChangedItems(e.NewItems))
                        {
                            item.PropertyChanged += onItemPropertyChanged;
                            itemsListening.Add(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var item in GetINotifyPropertyChangedItems(e.OldItems))
                        {
                            item.PropertyChanged -= onItemPropertyChanged;
                            itemsListening.Remove(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        foreach (var item in GetINotifyPropertyChangedItems(e.NewItems))
                        {
                            item.PropertyChanged += onItemPropertyChanged;
                            itemsListening.Add(item);
                        }
                        foreach (var item in GetINotifyPropertyChangedItems(e.OldItems))
                        {
                            item.PropertyChanged -= onItemPropertyChanged;
                            itemsListening.Remove(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        foreach (var item in itemsListening)
                        {
                            item.PropertyChanged -= onItemPropertyChanged;
                        }
                        itemsListening.Clear();
                        if (sender is not IEnumerable<T> s) break;
                        foreach (var item in GetINotifyPropertyChangedItems(s))
                        {
                            item.PropertyChanged += onItemPropertyChanged;
                            itemsListening.Remove(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        // ignored.
                        break;
                }

            onCollectionChanged(sender, e);
        }

        private IEnumerable<INotifyPropertyChanged> GetINotifyPropertyChangedItems(IEnumerable source)
        {
            foreach (var item in source)
            {
                if (item is not INotifyPropertyChanged inpc) continue;
                yield return inpc;
            }
        }
    }
}
