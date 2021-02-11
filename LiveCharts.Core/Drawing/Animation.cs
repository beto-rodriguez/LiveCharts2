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

namespace LiveChartsCore.Drawing
{
    public class Animation
    {
        private Func<float, float> easingFunction;
        private long duration;
        private int repeat = 1;

        public Animation()
        {

        }

        public Animation(Func<float, float> easingFunction, TimeSpan duration)
        {
            EasingFunction = easingFunction;
            Duration = (long)duration.TotalMilliseconds;
        }

        public Animation(Func<float, float> easingFunction, TimeSpan duration, int repeat)
        {
            this.easingFunction = easingFunction;
            this.duration = (long)duration.TotalMilliseconds;
            this.repeat = repeat;
        }

        public Animation(Animation animation)
        {
            this.easingFunction = animation.EasingFunction;
            this.duration = animation.Duration;
            this.repeat = animation.Repeat;
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
        /// Gets or sets the number of times the Animation will be repeated, default is 1, use <see cref="int.MaxValue"/> to repeat the animation infinitely.
        /// </summary>
        public int Repeat { get => repeat; set => repeat = value; }
    }
}
