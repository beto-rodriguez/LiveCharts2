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

namespace LiveChartsCore.Context
{
    /// <summary>
    /// Defines bounds for both, X and Y axes.
    /// </summary>
    public class DimensinalBounds
    {
        private Bounds secondaryBounds;
        private Bounds primaryBounds;
        private Bounds tertiaryBounds;
        private Bounds quaternaryBounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensinalBounds"/> class.
        /// </summary>
        public DimensinalBounds()
        {
            secondaryBounds = new Bounds();
            primaryBounds = new Bounds();
            tertiaryBounds = new Bounds();
            quaternaryBounds = new Bounds();
        }

        /// <summary>
        /// Gets or sets the primary bounds.
        /// </summary>
        public Bounds PrimaryBounds { get => primaryBounds; set => primaryBounds = value; } 

        /// <summary>
        /// Gets or sets the secondary bounds.
        /// </summary>
        public Bounds SecondaryBounds { get => secondaryBounds; set =>  secondaryBounds = value; } 

        /// <summary>
        /// Gets or sets the tertiary bounds.
        /// </summary>
        public Bounds TertiaryBounds { get => tertiaryBounds; set => tertiaryBounds = value; }

        /// <summary>
        /// Gets or sets the quaternary bounds.
        /// </summary>
        public Bounds QuaternaryBounds { get => quaternaryBounds; set => quaternaryBounds = value; }
    }
}
