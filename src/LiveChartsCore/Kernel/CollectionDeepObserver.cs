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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace LiveChartsCore.Kernel
{
    public class CollectionDeepObserver<T>
    {
        private readonly NotifyCollectionChangedEventHandler onCollectionChanged;
        private readonly PropertyChangedEventHandler onItemPropertyChanged;
        private readonly HashSet<INotifyPropertyChanged> itemsListening = new();

        protected bool implementsINPC;

        public CollectionDeepObserver(
            NotifyCollectionChangedEventHandler onCollectionChanged, PropertyChangedEventHandler onItemPropertyChanged)
        {
            this.onCollectionChanged = onCollectionChanged;
            this.onItemPropertyChanged = onItemPropertyChanged;
            implementsINPC = typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T));
        }

        public void Initialize(IEnumerable<T>? instance)
        {
            if (instance == null) return;

            if (instance is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += OnCollectionChanged;
            }

            if (implementsINPC)
                foreach (var item in instance.Cast<INotifyPropertyChanged>())
                    item.PropertyChanged += onItemPropertyChanged;
        }

        public void Dispose(IEnumerable<T>? instance)
        {
            if (instance == null) return;

            if (instance is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged -= OnCollectionChanged;
            }

            if (implementsINPC)
                foreach (var item in instance.Cast<INotifyPropertyChanged>())
                    item.PropertyChanged -= onItemPropertyChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (implementsINPC)
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in e.NewItems.Cast<INotifyPropertyChanged>())
                        {
                            item.PropertyChanged += onItemPropertyChanged;
                            itemsListening.Add(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var item in e.OldItems.Cast<INotifyPropertyChanged>())
                        {
                            item.PropertyChanged -= onItemPropertyChanged;
                            itemsListening.Remove(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        foreach (var item in e.NewItems.Cast<INotifyPropertyChanged>())
                        {
                            item.PropertyChanged += onItemPropertyChanged;
                            itemsListening.Add(item);
                        }
                        foreach (var item in e.OldItems.Cast<INotifyPropertyChanged>())
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
                        if (!(sender is IEnumerable<T> s)) break;
                        foreach (var item in s.Cast<INotifyPropertyChanged>())
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
    }
}
