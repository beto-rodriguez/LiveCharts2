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

using LiveChartsCore.Motion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveChartsCore.Drawing.Common
{
    /// <inheritdoc cref="IAnimatable" />
    public abstract class Animatable : IAnimatable
    {
        /// <summary>
        /// The transition properties
        /// </summary>
        protected Dictionary<string, IMotionProperty> transitionProperties = new();
        internal long _currentTime = long.MinValue;
        internal bool _isCompleted = true;
        internal bool _removeOnCompleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="Animatable"/> class.
        /// </summary>
        protected Animatable() { }

        /// <inheritdoc cref="IAnimatable.IsValid" />
        bool IAnimatable.IsValid { get => _isCompleted; set => _isCompleted = value; }

        /// <inheritdoc cref="IAnimatable.CurrentTime" />
        long IAnimatable.CurrentTime { get => _currentTime; set => _currentTime = value; }

        /// <inheritdoc cref="IAnimatable.RemoveOnCompleted" />
        public bool RemoveOnCompleted { get => _removeOnCompleted; set => _removeOnCompleted = value; }

        /// <inheritdoc cref="SetPropertiesTransitions(Animation, string[])" />
        public void SetPropertiesTransitions(Animation? animation, params string[] properties)
        {
            var a = animation?.Duration == 0 ? null : animation;

            foreach (var name in properties)
            {
                transitionProperties[name].Animation = a;
            }
        }

        /// <inheritdoc cref="IAnimatable.RemovePropertyTransition(string)" />
        public void RemovePropertyTransition(string propertyName)
        {
            transitionProperties[propertyName].Animation = null;
        }

        /// <inheritdoc cref="IAnimatable.RemoveTransitions" />
        public void RemoveTransitions()
        {
            foreach (var property in transitionProperties)
            {
                property.Value.Animation = null;
            }
        }

        /// <summary>
        /// Invalidates this animatable.
        /// </summary>
        /// <returns></returns>
        public void Invalidate()
        {
            _isCompleted = false;
        }

        /// <inheritdoc cref="IAnimatable.CompleteTransitions(string[])" />
        public virtual void CompleteTransitions(params string[] propertyNames)
        {
            if (propertyNames is null || propertyNames.Length == 0)
                throw new Exception(
                    $"At least one property is required to call {nameof(CompleteTransitions)}.");

            foreach (var property in propertyNames)
            {
                if (!transitionProperties.TryGetValue(property, out var transitionProperty))
                    throw new Exception(
                        $"The property {property} is not a transition property of this instance.");

                if (transitionProperty.Animation is null) continue;
                transitionProperty.IsCompleted = true;
            }
        }

        /// <inheritdoc cref="IAnimatable.CompleteAllTransitions" />
        public virtual void CompleteAllTransitions()
        {
            var p = transitionProperties.Keys.ToArray();
            if (p.Length == 0) return;

            CompleteTransitions(p);
        }

        /// <inheritdoc cref="IAnimatable.GetTransitionProperty(string)" />
        public IMotionProperty GetTransitionProperty(string propertyName)
        {
            return !transitionProperties.TryGetValue(propertyName, out var transitionProperty)
                ? throw new Exception(
                    $"The property {propertyName} is not a transition property of this instance.")
                : transitionProperty;
        }

        /// <summary>
        /// Registers a motion property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="motionProperty">The transition.</param>
        /// <returns></returns>
        protected T RegisterMotionProperty<T>(T motionProperty)
            where T : IMotionProperty
        {
            transitionProperties[motionProperty.PropertyName] = motionProperty;
            return motionProperty;
        }

        /// <summary>
        /// Sets the current time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        protected void SetCurrentTime(long time)
        {
            _currentTime = time;
        }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        /// <returns></returns>
        protected long GetCurrentTime()
        {
            return _currentTime;
        }
    }
}
