using LiveChartsCore.Drawing;

namespace LiveChartsCore.Context
{
    public delegate void TransitionsSetterDelegate<T>(T visual, Animation chartAnimation);

    public delegate void ChartPointMapperDelegate<TModel>(IChartPoint point, TModel model, IChartPointContext pointContext);
}
