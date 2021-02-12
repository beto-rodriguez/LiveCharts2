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

using LiveChartsCore.Transitions;
using System.Collections.Generic;

namespace LiveChartsCore.Drawing.Common
{
    public abstract class Animatable : IAnimatable
    {
        private Dictionary<string, ITransitionProperty> transitionProperties = new Dictionary<string, ITransitionProperty>();
        internal long currentTime;
        internal bool isCompleted = true;
        internal bool removeOnCompleted;

        public Animatable()
        {
        }

        bool IAnimatable.IsCompleted { get => isCompleted; set => isCompleted = value; }
        long IAnimatable.CurrentTime { get => currentTime; set => currentTime = value; }


        /// <summary>
        /// if true, the element will be removed from the UI the next time <see cref="TransitionCompleted"/> event occurs.
        /// </summary>
        public bool RemoveOnCompleted { get => removeOnCompleted; set => removeOnCompleted = value; }

        public void SetPropertyTransition(Animation animation, params string[] propertyName)
        {
            foreach (var name in propertyName)
            {
                transitionProperties[name].Animation = animation;
            }
        }

        public void RemovePropertyTransition(string propertyName)
        {
            transitionProperties[propertyName].Animation = null;
        }

        /// <summary>
        /// Completes the current transitions.
        /// </summary>
        public virtual void CompleteTransitions()
        {
            isCompleted = true;
        }

        public void Invalidate()
        {
            isCompleted = false;
        }

        protected T RegisterTransitionProperty<T>(T transition)
            where T : ITransitionProperty
        {
            transitionProperties[transition.PropertyName] = transition;
            return transition;
        }
    }
}
