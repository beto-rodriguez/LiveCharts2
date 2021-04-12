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
        private Animation? animation;
        protected internal T? fromValue = default;
        protected internal T? toValue = default;
        internal long startTime;
        internal long endTime;
        private bool isCompleted = false;
        private readonly string propertyName;
        private bool requiresToInitialize = true;

        public MotionProperty(string propertyName)
        {
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Gets the value where the transition began.
        /// </summary>
        public T? FromValue { get => fromValue; }

        /// <summary>
        /// Gets the value where the transition finished or will finish.
        /// </summary>
        public T? ToValue { get => toValue; }

        /// <summary>
        /// Gets or sets the animation to define the transition.
        /// </summary>
        public Animation? Animation { get => animation; set => animation = value; }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName => propertyName;

        public bool IsCompleted { get => isCompleted; set => isCompleted = value; }

        /// <summary>
        /// Moves to he specified value.
        /// </summary>
        /// <param name="value">The value to move to.</param>
        /// <param name="animatable">The <see cref="Visual"/> instance that is moving.</param>
        public void SetMovement(T value, Animatable animatable)
        {
            fromValue = GetMovement(animatable);
            toValue = value;
            if (animation != null)
            {
                if (animatable.currentTime == long.MinValue) // the animatable is not in the canvas yet.
                {
                    requiresToInitialize = true;
                }
                else
                {
                    startTime = animatable.currentTime;
                    endTime = animatable.currentTime + animation.duration;
                }
                animation.animationCompletedCount = 0;
                isCompleted = false;
                requiresToInitialize = true;
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
            if (animation == null || fromValue == null || isCompleted) return OnGetMovement(1);

            if (requiresToInitialize)
            {
                startTime = animatable.currentTime;
                endTime = animatable.currentTime + animation.duration;
                requiresToInitialize = false;
            }

            // at this points we are sure that the animatable has not finished at least with this property.
            animatable.isCompleted = false;

            // is this line necessary? ...
            //if (animatable.currentTime - startTime <= 0) return OnGetMovement(0);

            var p = (animatable.currentTime - startTime) / unchecked((float)(endTime - startTime));

            if (p >= 1)
            {
                // at this point the animation is completed
                p = 1;
                animation.animationCompletedCount++;
                isCompleted = animation.repeatTimes != int.MaxValue && animation.repeatTimes < animation.animationCompletedCount;
                if (!isCompleted)
                {
                    startTime = animatable.currentTime;
                    endTime = animatable.currentTime + animation.duration;
                    isCompleted = false;
                }
            }

            var fp = animation.EasingFunction(p);
            return OnGetMovement(fp);
        }

        protected abstract T OnGetMovement(float progress);
    }
}
