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


using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LiveChartsCore.Defaults
{
    public class ObservablePoint : INotifyPropertyChanged
    {
        private double? x;
        private double? y;

        public ObservablePoint()
        {

        }

        public ObservablePoint(double? x, double? y)
        {
            this.x = x;
            this.y = y;
        }

        public double? X { get => x; set { x = value; OnPropertyChanged(); } }

        public double? Y { get => y; set { y = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(propertyName, new PropertyChangedEventArgs(propertyName));
        }
    }
}
