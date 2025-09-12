using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Layouts;
using LiveChartsCore.Vortice.Drawing.Geometries;

namespace LiveChartsCore.Vortice.Drawing.Layouts;

///<inheritdoc cref="BaseContainer{TShape, TDrawingContext}"/>
public class Container : Container<RoundedRectangleGeometry>
{ }

///<inheritdoc cref="BaseContainer{TShape, TDrawingContext}"/>
/// <summary>
/// Initializes a new instance of the <see cref="Container{TShape}"/> class.
/// </summary>
public class Container<TShape> : BaseContainer<TShape, VorticeDrawingContext>
    where TShape : BoundedDrawnGeometry, IDrawnElement<VorticeDrawingContext>, new()
{ }
