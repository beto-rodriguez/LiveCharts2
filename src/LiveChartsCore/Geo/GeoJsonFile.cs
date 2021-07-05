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

using System.Collections.Generic;

namespace LiveChartsCore.Geo
{
    /// <summary>
    /// Defines a map using the GeoJson format.
    /// </summary>
    public class GeoJsonFile
    {
        private Dictionary<string, GeoJsonFeature>? _indexedFeatures;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public object? Properties { get; set; }

        /// <summary>
        /// Gets or sets the features.
        /// </summary>
        /// <value>
        /// The features.
        /// </value>
        public GeoJsonFeature[]? Features { get; set; }

        /// <summary>
        /// Finds the feature by short name.
        /// </summary>
        /// <param name="shortName">The short name.</param>
        /// <returns></returns>
        public GeoJsonFeature FindFeature(string shortName)
        {
            var features = _indexedFeatures ??= IndexFeatures();
            return features[shortName];
        }

        private Dictionary<string, GeoJsonFeature> IndexFeatures()
        {
            var index = new Dictionary<string, GeoJsonFeature>();

            if (Features is null) return index;

            foreach (var feature in Features)
            {
                if (feature.Geometry is null || feature.Geometry.Coordinates is null) continue;

                foreach (var geometry in feature.Geometry.Coordinates)
                {
                    foreach (var segment in geometry)
                    {
                        foreach (var point in segment)
                        {
                            var x = point[0];
                            var y = point[1];

                            if (x > feature.MaxBounds[0]) feature.MaxBounds[0] = x;
                            if (y > feature.MaxBounds[1]) feature.MaxBounds[1] = y;
                            if (x < feature.MinBounds[0]) feature.MinBounds[0] = x;
                            if (y < feature.MinBounds[1]) feature.MinBounds[1] = y;
                        }
                    }
                }

                index.Add((feature.Properties?["shortName"] ?? "?").ToLowerInvariant(), feature);
            }

            return index;
        }
    }
}
