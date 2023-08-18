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
/// Defines a color.
/// </summary>
public struct LvcColor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LvcColor"/> struct.
    /// </summary>
    /// <param name="red">The red component from 0 to 255.</param>
    /// <param name="green">The green component from 0 to 255.</param>
    /// <param name="blue">The blue component from 0 to 255.</param>
    /// <param name="alpha">The alpha channel component from 0 to 255.</param>
    public LvcColor(byte red, byte green, byte blue, byte alpha)
    {
        R = red;
        G = green;
        B = blue;
        A = alpha;
        IsEmpty = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LvcColor"/> struct.
    /// </summary>
    /// <param name="red">The red component from 0 to 255.</param>
    /// <param name="green">The green component from 0 to 255.</param>
    /// <param name="blue">The blue component from 0 to 255.</param>
    public LvcColor(byte red, byte green, byte blue) : this(red, green, blue, 255) { }

    internal LvcColor(bool isEmpty)
    {
        R = 255;
        G = 255;
        B = 255;
        A = 0;
        IsEmpty = isEmpty;
    }

    /// <summary>
    /// Gets an empty color.
    /// </summary>
    public static LvcColor Empty => new(true);

    /// <summary>
    /// Gets or sets the red component.
    /// </summary>
    public byte R { get; set; }

    /// <summary>
    /// Gets or sets the green component.
    /// </summary>
    public byte G { get; set; }

    /// <summary>
    /// Gets or sets the blue component.
    /// </summary>
    public byte B { get; set; }

    /// <summary>
    /// Gets or sets the alpha component.
    /// </summary>
    public byte A { get; set; }

    /// <summary>
    /// Gets or sets whether this color is empty.
    /// </summary>
    private bool IsEmpty { get; set; }

    /// <summary>
    /// Determines whether the instance is equals to the given instance.
    /// </summary>
    /// <param name="obj">The instance to compare to.</param>
    /// <returns>The comparision result.</returns>
    public override readonly bool Equals(object? obj)
    {
        return obj is LvcColor color &&
            R == color.R && G == color.G && B == color.B && A == color.A && IsEmpty == color.IsEmpty;
    }

    /// <summary>
    /// Gets the object hash code.
    /// </summary>
    /// <returns></returns>
    public override readonly int GetHashCode()
    {
        var hashCode = 1960784236;
        hashCode = hashCode * -1521134295 + R.GetHashCode();
        hashCode = hashCode * -1521134295 + G.GetHashCode();
        hashCode = hashCode * -1521134295 + B.GetHashCode();
        hashCode = hashCode * -1521134295 + A.GetHashCode();
        return hashCode;
    }

    /// <summary>
    /// Compares two <see cref="LvcColor"/> instances.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(LvcColor left, LvcColor right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="LvcColor"/> instances.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(LvcColor left, LvcColor right) => !(left == right);

    /// <summary>
    /// Creates a new instance of the <see cref="LvcColor"/> class with the given components.
    /// </summary>
    /// <param name="red">The red component from 0 to 255.</param>
    /// <param name="green">The green component from 0 to 255.</param>
    /// <param name="blue">The blue component from 0 to 255.</param>
    /// <returns></returns>
    public static LvcColor FromRGB(byte red, byte green, byte blue)
    {
        return new LvcColor(red, green, blue);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="LvcColor"/> class with the given components.
    /// </summary>
    /// <param name="alpha">The alpha channel component from 0 to 255.</param>
    /// <param name="red">The red component from 0 to 255.</param>
    /// <param name="green">The green component from 0 to 255.</param>
    /// <param name="blue">The blue component from 0 to 255.</param>
    /// <returns></returns>
    public static LvcColor FromArgb(byte alpha, byte red, byte green, byte blue)
    {
        return new LvcColor(red, green, blue, alpha);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="LvcColor"/> class with the given components.
    /// </summary>
    /// <param name="alpha">The alpha channel component from 0 to 255.</param>
    /// <param name="color">The red color.</param>
    /// <returns></returns>
    public static LvcColor FromArgb(byte alpha, LvcColor color)
    {
        return new LvcColor(color.R, color.G, color.B, alpha);
    }
}
