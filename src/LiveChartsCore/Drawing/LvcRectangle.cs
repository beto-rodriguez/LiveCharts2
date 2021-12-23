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

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines  a rectangle.
/// </summary>
public struct LvcRectangle
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LvcRectangle"/> struct.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="size"></param>
    public LvcRectangle(LvcPoint location, LvcSize size)
    {
        Location = location;
        Size = size;
        IsEmpty = false;
    }

    private LvcRectangle(bool empty)
    {
        Location = new LvcPoint();
        Size = new LvcSize();
        IsEmpty = empty;
    }

    /// <summary>
    /// Gets an empty rectangle instance.
    /// </summary>
    public static LvcRectangle Empty = new(true);

    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    public LvcPoint Location { get; set; }

    /// <summary>
    /// Gets the X location coordinate.
    /// </summary>
    public float X => Location.X;

    /// <summary>
    /// Gets the Y location coordinate.
    /// </summary>
    public float Y => Location.Y;

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    public LvcSize Size { get; set; }

    /// <summary>
    /// Gets the width.
    /// </summary>
    public float Width => Size.Width;

    /// <summary>
    /// Gets the height.
    /// </summary>
    public float Height => Size.Height;

    /// <summary>
    /// Gets or sets whether the instance is empty.
    /// </summary>
    private bool IsEmpty { get; set; }

    /// <summary>
    /// Determines whether the instance is equals to the given instance.
    /// </summary>
    /// <param name="obj">The instance to compare to.</param>
    /// <returns>The comparision result.</returns>
    public override bool Equals(object? obj)
    {
        return obj is LvcRectangle rectangle
            && ((IsEmpty && rectangle.IsEmpty) ||
                (Location == rectangle.Location && Size == rectangle.Size));
    }

    /// <summary>
    /// Gets the object hash code.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = 574998336;
        hashCode = hashCode * -1521134295 + Location.GetHashCode();
        hashCode = hashCode * -1521134295 + Size.GetHashCode();
        hashCode = hashCode * -1521134295 + IsEmpty.GetHashCode();
        return hashCode;
    }

    /// <summary>
    /// Compares 2 <see cref="LvcRectangle"/> instances.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(LvcRectangle left, LvcRectangle right) => left.Equals(right);

    /// <summary>
    /// Compares 2 <see cref="LvcRectangle"/> instances.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(LvcRectangle left, LvcRectangle right) => !(left == right);
}
