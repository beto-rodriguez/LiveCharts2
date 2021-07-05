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

using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;

namespace LiveChartsCore.Motion
{
    /// <summary>
    /// The <see cref="MotionProperty{T}"/> object tracks where a property of a <see cref="Animatable"/> is in a time line.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MotionProperty<T> : IMotionProperty
    {
        /// <summary>
        /// From value
        /// </summary>
        protected internal T? fromValue = default;

        /// <summary>
        /// To value
        /// </summary>
        protected internal T? toValue = default;
        internal long _startTime;
        internal long _endTime;
        private bool _requiresToInitialize = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionProperty{T}"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected MotionProperty(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the value where the transition began.
        /// </summary>
        public T? FromValue => fromValue;

        /// <summary>
        /// Gets the value where the transition finished or will finish.
        /// </summary>
        public T? ToValue => toValue;

        /// <summary>
        /// Gets or sets the animation to define the transition.
        /// </summary>
        public Animation? Animation { get; set; }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is completed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is completed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// Moves to he specified value.
        /// </summary>
        /// <param name="value">The value to move to.</param>
        /// <param name="animatable">The <see cref="IAnimatable"/> instance that is moving.</param>
        public void SetMovement(T value, Animatable animatable)
        {
            fromValue = GetMovement(animatable);
            toValue = value;
            if (Animation is not null)
            {
                if (animatable._currentTime == long.MinValue) // the animatable is not in the canvas yet.
                {
                    _requiresToInitialize = true;
                }
                else
                {
                    _startTime = animatable._currentTime;
                    _endTime = animatable._currentTime + Animation._duration;
                }
                Animation._animationCompletedCount = 0;
                IsCompleted = false;
                _requiresToInitialize = true;
            }
            animatable.Invalidate();
        }

        /// <summary>
        /// Gets the current movement in the <see cref="Animation"/>.
        /// </summary>
        /// <param name="animatable"></param>
        /// <returns></returns>
        public T GetMovement(Animatable animatable)
        {
            if (Animation is null || Animation.EasingFunction is null || fromValue is null || IsCompleted) return OnGetMovement(1);

            if (_requiresToInitialize)
            {
                _startTime = animatable._currentTime;
                _endTime = animatable._currentTime + Animation._duration;
                _requiresToInitialize = false;
            }

            // at this points we are sure that the animatable has not finished at least with this property.
            animatable._isCompleted = false;

            // is this line necessary? ...
            //if (animatable.currentTime - startTime <= 0) return OnGetMovement(0);

            var p = (animatable._currentTime - _startTime) / unchecked((float)(_endTime - _startTime));

            if (p >= 1)
            {
                // at this point the animation is completed
                p = 1;
                Animation._animationCompletedCount++;
                IsCompleted = Animation._repeatTimes != int.MaxValue && Animation._repeatTimes < Animation._animationCompletedCount;
                if (!IsCompleted)
                {
                    _startTime = animatable._currentTime;
                    _endTime = animatable._currentTime + Animation._duration;
                    IsCompleted = false;
                }
            }

            var fp = Animation.EasingFunction(p);
            return OnGetMovement(fp);
        }

        /// <summary>
        /// Called to get the movement at a specific progress.
        /// </summary>
        /// <param name="progress">The progress.</param>
        /// <returns></returns>
        protected abstract T OnGetMovement(float progress);
    }
}
