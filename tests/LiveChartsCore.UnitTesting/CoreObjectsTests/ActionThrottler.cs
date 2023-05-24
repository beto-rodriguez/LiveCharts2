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

using System;
using System.Threading;
using System.Threading.Tasks;
using LiveChartsCore.Kernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting.CoreObjectsTests;

[TestClass]
public class ActionThrottlerTester
{
    [TestMethod]
    public async Task SameThread()
    {
        var runs = 0;

        Func<Task> action =
            () => Task.Run(() =>
            {
                runs++;
            });

        var throttler = new ActionThrottler(action, TimeSpan.FromMilliseconds(50));

        throttler.Call();
        throttler.Call();
        throttler.Call();
        throttler.Call();
        throttler.Call();
        throttler.Call();

        await Task.Delay(TimeSpan.FromSeconds(0.5));

        Assert.IsTrue(runs == 1 && throttler.Calls == 6);
    }

    [TestMethod]
    public async Task MultipleThreads()
    {
        var runs = 0;

        Func<Task> action =
            () => Task.Run(() =>
            {
                runs++;
            });

        var throttler = new ActionThrottler(action, TimeSpan.FromMilliseconds(50));

        new Thread(() => throttler.Call()).Start();
        new Thread(() => throttler.Call()).Start();
        new Thread(() => throttler.Call()).Start();

        await Task.WhenAll(
            Task.Factory.StartNew(() => throttler.Call()),
            Task.Factory.StartNew(() => throttler.Call()),
            Task.Factory.StartNew(() => throttler.Call()),
            Task.Run(() => throttler.Call()),
            Task.Run(() => throttler.Call()),
            Task.Run(() => throttler.Call()));

        await Task.Delay(TimeSpan.FromSeconds(0.5));

        var calls = throttler.Calls;
        Assert.IsTrue(runs == 1 && throttler.Calls == 9);
    }
}
