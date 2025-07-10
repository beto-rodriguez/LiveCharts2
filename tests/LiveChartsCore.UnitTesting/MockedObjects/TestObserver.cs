using System;
using System.Collections.Generic;
using LiveChartsCore.Kernel.Observers;

namespace LiveChartsCore.UnitTesting.MockedObjects;

public class TestObserver<T> : IDisposable
{
    private readonly CollectionDeepObserver _observerer;

    public TestObserver()
    {
        _observerer = new CollectionDeepObserver(() => ChangesCount++);
    }

    public IEnumerable<T> MyCollection
    {
        get;
        set
        {
            _observerer.Dispose();
            _observerer.Initialize(value);
            field = value;
        }
    }

    public int ChangesCount { get; private set; }

    public void Dispose() => _observerer.Dispose();
}
