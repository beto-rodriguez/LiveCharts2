using LiveChartsCore.Drawing;

namespace LiveChartsCore.Context
{
    public delegate void TransitionsSetterDelegate<T>(T visual, Animation chartAnimation);
    public delegate void ChartPointMapperDelegate<T>(ChartPoint<T> point, T model, ChartPointContext pointContext);
}
