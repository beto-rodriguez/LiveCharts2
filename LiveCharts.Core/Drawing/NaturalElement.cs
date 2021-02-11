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

namespace LiveChartsCore.Drawing.Common
{
    public class NaturalElement : IAnimatable
    {
        internal long startTime;
        internal long endTime;
        internal long currentTime;
        internal Animation transition = new Animation(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(300));
        internal int animationRepeatCount = 0;
        internal bool requiresStoryboardCalculation = false;
        internal bool isCompleted = true;
        internal bool removeOnCompleted;

        public bool RequiresStoryboardCalculation { get => requiresStoryboardCalculation; set => requiresStoryboardCalculation = value; }

        public bool IsCompleted => isCompleted;

        /// <summary>
        /// if true, the element will be removed from the UI the next time <see cref="TransitionCompleted"/> event occurs.
        /// </summary>
        public bool RemoveOnCompleted { get => removeOnCompleted; set => removeOnCompleted = value; }

        /// <summary>
        /// Occurs when the transition of every property is completed.
        /// </summary>
        public event Action<NaturalElement> TransitionCompleted;

        public virtual void SetStoryboard(long start, Animation transition)
        {
            startTime = start;
            endTime = start + transition.Duration;
            this.transition = transition;
            requiresStoryboardCalculation = false;
            animationRepeatCount = 0;
        }

        /// <summary>
        /// Sets the transition time, returns weather the transition of all the properties is completed or not.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public virtual void SetTime(long frameTime)
        {
            if (isCompleted) return;

            currentTime = frameTime;
            if (currentTime >= endTime)
            {
                isCompleted = true;
                TransitionCompleted?.Invoke(this);
                return;
            }

            return;
        }

        /// <summary>
        /// Completes the current transitions.
        /// </summary>
        public virtual void CompleteTransitions()
        {
            isCompleted = true;
            currentTime = endTime;
        }

        public void Invalidate()
        {
            requiresStoryboardCalculation = true;
            isCompleted = false;
        }
    }

}
