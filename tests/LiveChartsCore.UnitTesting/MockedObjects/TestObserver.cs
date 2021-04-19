using LiveChartsCore.Kernel;
using System;
using System.Collections.Generic;

namespace LiveChartsCore.UnitTesting.MockedObjects
{
    public class TestObserver<T> : IDisposable
    {
        private readonly CollectionDeepObserver<T> observerer;
        private IEnumerable<T> observedCollection;

        public TestObserver()
        {
            observerer = new CollectionDeepObserver<T>(
                (sender, e) =>
                {
                    CollectionChangedCount++;
                },
                (sender, e) =>
                {
                    PropertyChangedCount++;
                });
        }

        public IEnumerable<T> MyCollection
        {
            get => observedCollection;
            set
            {
                observerer.Dispose(observedCollection);
                observerer.Initialize(value);
                observedCollection = value;
            }
        }

        public int CollectionChangedCount { get; private set; }
        public int PropertyChangedCount { get; private set; }

        public void Dispose()
        {
            observerer.Dispose(observedCollection);
        }
    }
}
