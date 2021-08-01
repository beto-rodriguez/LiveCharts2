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

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// Defines an animation.
    /// </summary>
    public class Animation
    {
        internal long _duration;
        internal int _animationCompletedCount = 0;
        internal int _repeatTimes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        public Animation()
        {
            EasingFunction = t => t;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <param name="easingFunction">The easing function.</param>
        /// <param name="duration">The duration.</param>
        public Animation(Func<float, float>? easingFunction, TimeSpan duration)
        {
            EasingFunction = easingFunction;
            _duration = (long)duration.TotalMilliseconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <param name="easingFunction">The easing function.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="repeatTimes">The repeat times.</param>
        public Animation(Func<float, float> easingFunction, TimeSpan duration, int repeatTimes)
        {
            EasingFunction = easingFunction;
            _duration = (long)duration.TotalMilliseconds;
            _repeatTimes = repeatTimes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <param name="easingFunction">The easing function.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="repeatTimes">The repeat times.</param>
        public Animation(Func<float, float> easingFunction, long duration, int repeatTimes)
        {
            EasingFunction = easingFunction;
            _duration = duration;
            _repeatTimes = repeatTimes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <param name="animation">The animation.</param>
        public Animation(Animation animation)
        {
            EasingFunction = animation.EasingFunction;
            _duration = animation._duration;
            _repeatTimes = animation._repeatTimes;
        }

        /// <summary>
        /// Gets or sets the easing function.
        /// </summary>
        public Func<float, float>? EasingFunction { get; set; }

        /// <summary>
        /// Gets or sets the duration of the transition in Milliseconds.
        /// </summary>
        public long Duration { get => _duration; set => _duration = value; }

        /// <summary>
        /// Gets or sets how many times the animation needs to repeat before it is completed, 
        /// use int.MaxValue to repeat it indefinitely number of times.
        /// </summary>
        public int Repeat { get => _repeatTimes; set => _repeatTimes = value; }

        /// <summary>
        /// Sets the easing function.
        /// </summary>
        /// <param name="easing">The easing.</param>
        /// <returns>The animation instance</returns>
        public Animation WithEasingFunction(Func<float, float>? easing)
        {
            EasingFunction = easing;
            return this;
        }

        /// <summary>
        /// Sets the duration.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>The animation instance</returns>
        public Animation WithDuration(TimeSpan duration)
        {
            _duration = (long)duration.TotalMilliseconds;
            return this;
        }

        /// <summary>
        /// Sets the duration.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>The animation instance</returns>
        public Animation WithDuration(long duration)
        {
            _duration = duration;
            return this;
        }

        /// <summary>
        /// Sets the repeats times.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <returns>The animation instance</returns>
        public Animation RepeatTimes(int times)
        {
            _repeatTimes = times;
            return this;
        }

        /// <summary>
        /// Sets he repeats times indefinitely number of times.
        /// </summary>
        /// <returns>The animation instance</returns>
        public Animation RepeatIndefinitely()
        {
            _repeatTimes = int.MaxValue;
            return this;
        }
    }
}
