using System;
using System.Collections.Generic;
using LiveChartsCore.Kernel.Observers;

namespace LiveChartsCore.UnitTesting.MockedObjects;

public class TestObserver<T> : IDisposable
{
    private readonly CollectionDeepObserver observerer;
    private IEnumerable<T> observedCollection;

    public TestObserver()
    {
        observerer = new CollectionDeepObserver(() => CollectionChangedCount++);
    }

    public IEnumerable<T> MyCollection
    {
        get => observedCollection;
        set
        {
            observerer.Dispose();
            observerer.Initialize(value);
            observedCollection = value;
        }
    }

    public int CollectionChangedCount { get; private set; }
    public int PropertyChangedCount { get; private set; }

    public void Dispose()
    {
        observerer.Dispose();
    }
}
