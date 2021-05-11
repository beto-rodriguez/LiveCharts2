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

using System;

namespace LiveChartsCore.Drawing.Common
{
    /// <summary>
    /// The Transition builder class helps to build transitions using fluent syntax.
    /// </summary>
    public class TransitionBuilder
    {
        private readonly string[] _properties;
        private readonly IAnimatable _target;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionBuilder"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="properties">The properties.</param>
        public TransitionBuilder(IAnimatable target, string[] properties)
        {
            _target = target;
            _properties = properties;
        }

        /// <summary>
        /// Sets the animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <returns>The transition</returns>
        public TransitionBuilder WithAnimation(Animation animation)
        {
            _target.SetPropertiesTransitions(animation, _properties);

            return this;
        }

        /// <summary>
        /// Sets the animation.
        /// </summary>
        /// <param name="animationBuilder">The animation builder.</param>
        /// <returns>The transition</returns>
        public TransitionBuilder WithAnimation(Action<Animation> animationBuilder)
        {
            var animation = new Animation();
            animationBuilder(animation);
            return WithAnimation(animation);
        }

        /// <summary>
        /// Sets the current transitions.
        /// </summary>
        /// <returns>The transition</returns>
        public TransitionBuilder CompleteCurrentTransitions()
        {
            _target.CompleteTransitions(_properties);
            return this;
        }
    }
}
