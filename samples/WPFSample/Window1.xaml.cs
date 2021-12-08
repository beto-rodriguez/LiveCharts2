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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace WPFSample
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private Random _random = new Random();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = (ViewModel1)DataContext;

            _ = vm.AddSeries(new ObservableCollection<DateTimePoint>
            {
                new DateTimePoint(DateTime.Today, _random.Next(0,10)),
                new DateTimePoint(DateTime.Today.AddDays(1), _random.Next(0,10)),
                new DateTimePoint(DateTime.Today.AddDays(2), _random.Next(0,10))
            }, "name " + vm.OSeries.Count, "unit");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var vm = (ViewModel1)DataContext;

            _ = vm.RemoveSeries(vm.OSeries.Last().Name);
        }
    }


    public class YaxesManager
    {
        public String Name;

        public ObservableCollection<String> SignalNameList;

        public YaxesManager(string name, String signalName)
        {
            SignalNameList = new ObservableCollection<string>();
            Name = name;
            SignalNameList.Add(signalName);
        }
    }

    public class ViewModel1
    {

        /// <summary>
        /// Collezione con i valori dei segnali da monitorare
        /// </summary>
        public ObservableCollection<LineSeries<DateTimePoint>> OSeries { get; set; }

        /// <summary>
        /// Collezione contenete i valori rappresentati sull'asse X
        /// </summary>
        public ObservableCollection<ICartesianAxis> OXAxes { get; set; }

        /// <summary>
        /// Collezione con i valori delle assi Y (una per ogni unita di misura)
        /// </summary>
        public ObservableCollection<ICartesianAxis> OYAxes { get; set; }

        public ObservableCollection<YaxesManager> OYManger { get; set; }

        //private static readonly ILog log = LogManagerWrapper.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ViewModel1()
        {
            #region VALUE

            // using object that implements INotifyPropertyChanged
            // will allow the chart to update everytime a property in a point changes.

            // LiveCharts already provides the ObservableValue class
            // notice you can plot any type, but you must let LiveCharts know how to handle it
            // for more info please see:
            // https://github.com/beto-rodriguez/LiveCharts2/blob/master/samples/ViewModelsSamples/General/UserDefinedTypes/ViewModel.cs#L2

            // using a collection that implements INotifyCollectionChanged as your series collection
            // will allow the chart to update every time a series is added, removed, replaced or the whole list was cleared
            // .Net already provides the System.Collections.ObjectModel.ObservableCollection class
            OSeries = new ObservableCollection<LineSeries<DateTimePoint>>();

            #endregion VALUE

            OYAxes = new ObservableCollection<ICartesianAxis>();

            OYManger = new ObservableCollection<YaxesManager>();

            // The UnitWidth is only required for column or financial series
            // because the library needs to know the width of each column, by default the
            // width is 1, but when you are using a different scale, you must let the library know it.
            OXAxes = new ObservableCollection<ICartesianAxis>
                {
                    new Axis
                    {
                        Labeler = value => new DateTime((long) value).ToString("dd/MM HH:mm"),
                        LabelsRotation = 65,

                        //UnitWidth = TimeSpan.FromMilliseconds(1).Ticks,
                        //MinStep = TimeSpan.FromMilliseconds(1).Ticks,
                        MinStep = 1,

                        // LabelsPaint = null will not draw the axis labels.
                        LabelsPaint = new SolidColorPaint{ Color = SKColors.CornflowerBlue },

                        // SeparatorsPaint = null will not draw the separator lines
                        SeparatorsPaint = new SolidColorPaint { Color = SKColors.LightBlue, StrokeThickness = 3 },
                    }
                };

        }

        /// <summary>
        /// In base all'unita di misura restituisce la posizione all'interno della collezione del'asse Y da utilizzare
        /// (nel caso non esista la crea)
        /// </summary>
        /// <param name="measureUnit"></param>
        /// <returns></returns>
        private int GetIndexYAxis(String measureUnit, String signalName)
        {
            if (OYManger.Count(x => x.Name == measureUnit) > 0)
            {
                int pos = OYManger.IndexOf(OYManger.Single(x => x.Name == measureUnit));
                OYManger[pos].SignalNameList.Add(signalName);
                return pos;
            }
            else
            {
                int pos = OYAxes.Count;
                OYAxes.Insert(pos, new Axis() // the "thousands" series will be scaled on this axis
                {
                    Name = measureUnit,
                    ShowSeparatorLines = true,
                    Position = LiveChartsCore.Measure.AxisPosition.End
                });
                OYManger.Insert(pos, new YaxesManager(measureUnit, signalName));
                return pos;
            }
        }

        /// <summary>
        /// Aggiunge un segnale al grafico
        /// </summary>
        /// <param name="oValues">Valori del segnale</param>
        /// <param name="name">Nome del segnale</param>
        /// <param name="measureUnit">Unita di misura del segnale</param>
        /// <returns></returns>
        public int AddSeries(ObservableCollection<DateTimePoint> oValues, String name, String measureUnit)
        {
            try
            {
                // Si controlla se è gia graficato il segnale passato
                if (OSeries.Where(x => x.Name == name).Count() > 0)
                {
                    return 1;
                }

                // Numero di segnali graficati
                int pos = OSeries.Count;

                // Si possono graficare un numero massimo di segnali
                int limit = 7;
                if (pos == limit)
                {
                    return 1;
                }

                int yIndex = GetIndexYAxis(measureUnit, name);

                OSeries.Insert(pos, new LineSeries<DateTimePoint>()
                {
                    Values = oValues,

                    Name = name,

                    // use the line smoothness property to control the curve
                    // it goes from 0 to 1
                    // where 0 is a straight line and 1 the most curved
                    LineSmoothness = 0,
                    //dimension of circle that define points

                    GeometrySize = 10,

                    GeometryFill = new SolidColorPaint(new SKColor(150, 150, 150, 150)),

                    ScalesYAt = yIndex
                });

                return OSeries.Count;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        /// <summary>
        /// Rimuove un segnale dal grafico e se necessario anche l'asse Y associata
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int RemoveSeries(String name)
        {
            try
            {
                OSeries.Remove(OSeries.Single(x => x.Name == name));

                int pos = OYManger.IndexOf(OYManger.Single(x => x.SignalNameList.Contains(name)));

                OYManger[pos].SignalNameList.Remove(name);

                if (OYManger[pos].SignalNameList.Count == 0)
                {
                    OYAxes.Remove(OYAxes[pos]);
                    OYManger.RemoveAt(pos);
                }

                return OSeries.Count;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        /// <summary>
        /// Rimuove tutti i segnali del grafico
        /// </summary>
        public void RemoveAll()
        {
            OSeries.Clear();
            //TODO?
            //YAxes.Clear();
            //YManger.Clear();
            //XAxes.Clear();
        }

    }
}
