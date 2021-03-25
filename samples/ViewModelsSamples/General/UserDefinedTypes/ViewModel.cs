using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;

namespace ViewModelsSamples.General.UserDefinedTypes
{
    public class Dog
    {
        public double Age { get; set; }
    }

    public class DogAverageAge
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class ViewModel
    {
        public ViewModel()
        {
            // in this case we have an array of the Dog class
            // we need to compare the Age property of every dog in our array
            // you normally should configure every intance you require to use only when your application starts.

            // by default LiveCharts already knows how to map the types:
            // short, int, long, float, double, decimal, short?, int?, long?, float?, double?, decimal?,
            // WeightedPoint, WeightedPointF, ObservablePoint, ObservablePointF, OversableValue and ObservableValueF
            // for more info see:
            // https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/LiveChartsCore/LiveChartsSettings.cs#L122
            LiveCharts.Configure(config =>

                // The HasMap<T>() method helps us to define a map from a type T to a point in our chart
                config
                    .HasMap<Dog>((dog, point) =>
                    {
                        // in this lambda function we take an intance of the Dog class (see dog parameter)
                        // and the point in the chart for that instance (see point parameter)
                        // LiveCharts will call this method for every instance of our Dog class,
                        // now we need to populate the point coordinates from our Dog intance to our point

                        // in this case we will use the Age property as our primary value (normally the Y coordinate)
                        point.PrimaryValue = (float)dog.Age;

                        // then the secondary value (normally the X coordinate)
                        // will be the index of the given dog class in our array
                        point.SecondaryValue = point.Context.Index;
                    })

                    // lets also set a mapper for the DogAverageAge class
                    .HasMap<DogAverageAge>((dogAverageAge, point) =>
                    {
                        // in this case we are ignoring the point paramenter
                        // every coordinate will be provided by our dogAverageAge instance
                        point.PrimaryValue = (float)dogAverageAge.Y;
                        point.SecondaryValue = (float)dogAverageAge.X;
                    })
                );
        }

        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new ColumnSeries<Dog>
            {
                Values = new List<Dog>
                {
                    new Dog { Age = 4 },
                    new Dog { Age = 6 },
                    new Dog { Age = 2 },
                    new Dog { Age = 8 },
                    new Dog { Age = 3 },
                    new Dog { Age = 4 }
                }
            },

            new LineSeries<DogAverageAge>
            {
                Values = new List<DogAverageAge>
                {
                    new DogAverageAge { X = 0, Y = (4 + 6 + 2) / 3d },
                    new DogAverageAge { X = 5, Y = (8 + 3 + 4) / 3d }
                },
                Fill = null
            }
        };
    }
}
