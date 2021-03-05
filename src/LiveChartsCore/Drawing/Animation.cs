﻿// The MIT License(MIT)

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

namespace LiveChartsCore.Drawing
{
    public class Animation
    {
        private Func<float, float> easingFunction;
        internal long duration;
        internal int animationCompletedCount = 0;
        internal int repeatTimes;

        public Animation()
        {
            easingFunction = t => t;
        }

        public Animation(Func<float, float> easingFunction, TimeSpan duration)
        {
            this.easingFunction = easingFunction;
            this.duration = (long)duration.TotalMilliseconds;
        }

        public Animation(Func<float, float> easingFunction, TimeSpan duration, int repeatTimes)
        {
            this.easingFunction = easingFunction;
            this.duration = (long)duration.TotalMilliseconds;
            this.repeatTimes = repeatTimes;
        }

        public Animation(Func<float, float> easingFunction, long duration, int repeatTimes)
        {
            this.easingFunction = easingFunction;
            this.duration = duration;
            this.repeatTimes = repeatTimes;
        }

        public Animation(Animation animation)
        {
            easingFunction = animation.easingFunction;
            duration = animation.duration;
            repeatTimes = animation.repeatTimes;
        }

        /// <summary>
        /// Gets or sets the easing function.
        /// </summary>
        public Func<float, float> EasingFunction { get => easingFunction; set => easingFunction = value; }

        /// <summary>
        /// Gets or sets the duration of the transition in Milliseconds.
        /// </summary>
        public long Duration { get => duration; set => duration = value; }

        /// <summary>
        /// Gets or sets how many times the animation needs to repeat before it is completed, 
        /// use int.MaxValue to repeat it indefinitely number of times.
        /// </summary>
        public int Repeat { get => repeatTimes; set => repeatTimes = value; }

        public Animation WithEasingFunction(Func<float, float> easing)
        {
            easingFunction = easing;
            return this;
        }

        public Animation WithDuration(TimeSpan duration)
        {
            this.duration = (long)duration.TotalMilliseconds;
            return this;
        }

        public Animation WithDuration(long duration)
        {
            this.duration = duration;
            return this;
        }

        public Animation RepeatTimes(int times)
        {
            repeatTimes = times;
            return this;
        }

        public Animation RepeatIndefinitely()
        {
            repeatTimes = int.MaxValue;
            return this;
        }
    }
}
