﻿// The MIT License(MIT)
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
using System.Threading.Tasks;

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// An object that is able to throttle an action.
    /// </summary>
    public class ActionThrottler
    {
        private readonly object _sync = new();
        private readonly Action _action;
        private bool _isWaiting = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionThrottler"/> class.
        /// </summary>
        /// <param name="targetAction">The target action to throttle.</param>
        /// <param name="time">The throttling time.</param>
        public ActionThrottler(Action targetAction, TimeSpan time)
        {
            _action = targetAction;
            ThrottlerTimeSpan = time;
        }

#if DEBUG
        /// <summary>
        /// Gets the calls.
        /// </summary>
        /// <value>
        /// The calls.
        /// </value>
        public int Calls { get; private set; } = 0;
#endif

        /// <summary>
        /// Gets or sets the throttler time span.
        /// </summary>
        /// <value>
        /// The throttler time span.
        /// </value>
        public TimeSpan ThrottlerTimeSpan { get; set; }

        /// <summary>
        /// Schedules a call to the target action.
        /// </summary>
        /// <returns></returns>
        public async void Call()
        {
            lock (_sync)
            {
#if DEBUG
                Calls++;
#endif

                if (_isWaiting) return;
                _isWaiting = true;
            }

            await Task.Delay(ThrottlerTimeSpan);

            // notice it is important that the unlock comes before invoking the Action
            // this way we can call the throttler again from the Action
            // otherwise calling the throttler from the Action will be ignored always.

            lock (_sync)
            {
                _isWaiting = false;
            }

            _action.Invoke();
        }

        /// <summary>
        /// Forces the call to the target action, this call is not throttled.
        /// </summary>
        /// <returns></returns>
        public void ForceCall()
        {
            _action.Invoke();
        }
    }
}
