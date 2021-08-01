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

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// Defines an object that can is able to animate its properties.
    /// </summary>
    public interface IAnimatable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is completed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is completed; otherwise, <c>false</c>.
        /// </value>
        bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the current time.
        /// </summary>
        /// <value>
        /// The current time.
        /// </value>
        long CurrentTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the element is removed when all the transitions complete.
        /// </summary>
        /// <value>
        ///   <c>true</c> if remove on completed; otherwise, <c>false</c>.
        /// </value>
        bool RemoveOnCompleted { get; set; }

        /// <summary>
        /// Sets the properties transitions.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="propertyName">Name of the property.</param>
        void SetPropertiesTransitions(Animation? animation, params string[] propertyName);

        /// <summary>
        /// Removes a property transition.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        void RemovePropertyTransition(string propertyName);

        /// <summary>
        /// Removes all the current transitions.
        /// </summary>
        void RemoveTransitions();

        /// <summary>
        /// Completes all transitions.
        /// </summary>
        void CompleteAllTransitions();

        /// <summary>
        /// Completes the transitions.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        void CompleteTransitions(params string[] propertyName);

        /// <summary>
        /// Gets the transition property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        IMotionProperty GetTransitionProperty(string propertyName);
    }
}
