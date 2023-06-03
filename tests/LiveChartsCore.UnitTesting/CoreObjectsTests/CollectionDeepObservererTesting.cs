// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore.UnitTesting.MockedObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting.CoreObjectsTests;

[TestClass]
public class CollectionDeepObservererTesting
{
    [TestMethod]
    public void TestMethod1()
    {
        var propertyChanges = 0;
        var collectionChanges = 0;

        // CASE 0, DUMMY CASE
        var dummyObserver = new TestObserver<int>();
        var dummyList = new List<int>();
        dummyObserver.MyCollection = dummyList;
        dummyList.Add(10);

        Assert.IsTrue(dummyObserver.CollectionChangedCount == 0 && dummyObserver.PropertyChangedCount == 0);

        // CASE 1, ANOTHER DUMMY CASE
        var propertyChangedObserver = new TestObserver<PropertyChangedObject>();

        var caseOneCollection = new List<PropertyChangedObject>();
        propertyChangedObserver.MyCollection = caseOneCollection;

        var o1 = new PropertyChangedObject();

        caseOneCollection.Add(o1);
        o1.Value = 10;

        // it ignores both
        // because the collection does not implements INCC
        // the add occurs after we assigned the instance to propertyChangedObserver.MyCollection
        // sadly we can not detect this without consuming unnecessary resources.
        Assert.IsTrue(propertyChangedObserver.CollectionChangedCount == 0 && propertyChangedObserver.PropertyChangedCount == 0);

        // CASE 2, MUST DETECT BOTH: INCC, IPC
        var caseTwoCollection = new ObservableCollection<PropertyChangedObject>();
        propertyChangedObserver.MyCollection = caseTwoCollection;

        var o2 = new PropertyChangedObject();

        caseTwoCollection.Add(o2);
        collectionChanges++;

        o2.Value = 10;
        propertyChanges++;

        // o1 change must be ignored
        o1.Value = 30;

        Assert.IsTrue(
            propertyChangedObserver.CollectionChangedCount == collectionChanges &&
            propertyChangedObserver.PropertyChangedCount == propertyChanges);

        // CASE 3, INSTANCE CHANGED.
        var caseThreeCollection = new ObservableCollection<PropertyChangedObject>();
        propertyChangedObserver.MyCollection = caseThreeCollection;

        var o3 = new PropertyChangedObject();
        caseThreeCollection.Add(o3);
        collectionChanges++;

        var o4 = new PropertyChangedObject();
        caseThreeCollection.Add(o4);
        collectionChanges++;

        o3.Value = 5;
        propertyChanges++;
        o3.Value = 8;
        propertyChanges++;
        o4.Value = 3;
        propertyChanges++;

        // should ignore the next changes
        caseTwoCollection.Add(new PropertyChangedObject());
        o1.Value = 100;
        o2.Value = 100;

        Assert.IsTrue(
            propertyChangedObserver.CollectionChangedCount == collectionChanges &&
            propertyChangedObserver.PropertyChangedCount == propertyChanges);

        // CASE 4, IT MUST STOP LISTENING WHEN WE REMOVE THE OBJECT FROM THE COLLECTION 
        _ = caseThreeCollection.Remove(o4);
        collectionChanges++;
        o4.Value = 60;
        o4.Value = 20;
        o4.Value = 40;

        Assert.IsTrue(
            propertyChangedObserver.CollectionChangedCount == collectionChanges &&
            propertyChangedObserver.PropertyChangedCount == propertyChanges);

        // CASE 5, BUT IT MUST BE LISTENING FOR THE OBJECTS THAT ARE INCLUDED IN THE COLLECTION
        o3.Value = 30;
        propertyChanges++;
        o3.Value = 40;
        propertyChanges++;
        o3.Value = 50;
        propertyChanges++;

        Assert.IsTrue(
            propertyChangedObserver.CollectionChangedCount == collectionChanges &&
            propertyChangedObserver.PropertyChangedCount == propertyChanges);

        // CASE 5, REPLACEMENT
        var o5 = new PropertyChangedObject();
        caseThreeCollection[0] = o5;
        collectionChanges++;

        // it must stop listening for changes in o3 (index 0)
        o3.Value = 1;
        o3.Value = 2;
        o3.Value = 3;
        o3.Value = 4;

        // and now it should listen for changes in o5;
        o5.Value = 1;
        propertyChanges++;
        o5.Value = 2;
        propertyChanges++;

        Assert.IsTrue(
           propertyChangedObserver.CollectionChangedCount == collectionChanges &&
           propertyChangedObserver.PropertyChangedCount == propertyChanges);

        // CASE 6, COLLECTION CLEARED.
        caseThreeCollection.Clear();
        collectionChanges++;

        // must not count the following changes..
        o5.Value = 2;
        o5.Value = 2;
        o5.Value = 2;

        Assert.IsTrue(
          propertyChangedObserver.CollectionChangedCount == collectionChanges &&
          propertyChangedObserver.PropertyChangedCount == propertyChanges);
    }
}
