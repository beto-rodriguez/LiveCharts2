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

using System;
using System.Threading.Tasks;

namespace LiveChartsCore.Rx
{
    public abstract class BaseActionThrottler<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        private TimeSpan lockTime;
        private DateTime lockUntil = DateTime.Now;
        private bool willNotifyUnlock = false;

        public BaseActionThrottler(TimeSpan lockTime)
        {
            this.lockTime = lockTime;
        }

        public TimeSpan LockTime { get => lockTime; set => lockTime = value; }

        protected abstract void OnUnlocked(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param);

        protected void OnTryRun(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            var now = DateTime.Now;
            if (now < lockUntil)
            {
                WaitThenRun(param1, param2, param3, param4, param5);
                return;
            }

            lockUntil = now.Add(lockTime);
            OnUnlocked(param1, param2, param3, param4, param5);
        }

        private async void WaitThenRun(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            if (willNotifyUnlock) return;
            willNotifyUnlock = true;

            await Task.Delay(LockTime);
            willNotifyUnlock = false;
            OnUnlocked(param1, param2, param3, param4, param5);
        }
    }
}
