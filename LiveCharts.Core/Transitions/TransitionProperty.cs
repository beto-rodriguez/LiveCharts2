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

namespace LiveChartsCore.Transitions
{
    /// <summary>
    /// The <see cref="TransitionProperty{T}"/> object tracks where a property of a <see cref="Animatable"/> is in a time line.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TransitionProperty<T>: ITransitionProperty
    {
        protected internal T fromValue;
        protected internal T toValue;
        private Animation animation;
        private readonly string propertyName;

        public TransitionProperty(string propertyName)
        {
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Gets the value where the transition began.
        /// </summary>
        public T FromValue { get => fromValue; }

        /// <summary>
        /// Gets the value where the transition finished or will finish.
        /// </summary>
        public T ToValue { get => toValue; }

        /// <summary>
        /// Gets or sets the animation to define the transition.
        /// </summary>
        public Animation Animation { get => animation; set => animation = value; }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName => propertyName;

        /// <summary>
        /// Moves to he specified value.
        /// </summary>
        /// <param name="value">The value to move to.</param>
        /// <param name="visual">The <see cref="Visual"/> instance that is moving.</param>
        public void MoveTo(T value, Animatable visual)
        {
            fromValue = GetCurrentMovement(visual);
            toValue = value;
            if (animation != null) { animation.Restart(visual.currentTime); animation.animationCompletedCount = 0; }
            visual.Invalidate();
        }

        /// <summary>
        /// Gets the current movement in the <see cref="Animation"/>.
        /// </summary>
        /// <param name="animatable"></param>
        /// <returns></returns>
        public T GetCurrentMovement(Animatable animatable)
        {
            if (animation == null || animation.isCompleted) return OnGetMovement(1);

            // at this points we are sure that the animatable has not finished at least with this property.
            animatable.isCompleted = false;

            if (animatable.currentTime - animation.startTime <= 0) return OnGetMovement(0);

            unchecked
            {
                var p = (animatable.currentTime - animation.startTime) / (float)(animation.endTime - animation.startTime);

                if (p >= 1)
                {
                    // at this point the animation is completed
                    p = 1;
                    animation.animationCompletedCount++;
                    animation.isCompleted = animation.repeatTimes != int.MaxValue && animation.repeatTimes < animation.animationCompletedCount;
                    if (!animation.isCompleted) animation.Restart(animatable.currentTime);
                }

                var fp = animation.EasingFunction(p);
                return OnGetMovement(fp);
            }
        }

        protected abstract T OnGetMovement(float progress);
    }
}
