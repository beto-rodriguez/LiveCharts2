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

using System.Globalization;
using System;

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
    /// Parses a hexadecimal color string.
    /// </summary>
    /// <param name="hexString"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static LvcColor Parse(string hexString)
    {
        return !TryParse(hexString, out var result)
            ? throw new ArgumentException("Invalid hexadecimal color string.", "hexString")
            : result;
    }

    /// <summary>
    /// Tries to parses a hexadecimal color string.
    /// </summary>
    /// <param name="hexString"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static bool TryParse(string hexString, out LvcColor color)
    {
        if (string.IsNullOrWhiteSpace(hexString))
        {
            color = Empty;
            return false;
        }

        hexString = hexString.Trim();
        var num = (hexString[0] == '#') ? 1 : 0;
        var num2 = hexString.Length - num;

        switch (num2)
        {
            case 3:
            case 4:
                {
                    byte result2;
                    if (num2 == 4)
                    {
                        if (!byte.TryParse(string.Concat(new string(hexString[num2 - 4 + num], 2)), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result2))
                        {
                            color = Empty;
                            return false;
                        }
                    }
                    else
                    {
                        result2 = byte.MaxValue;
                    }

                    if (!byte.TryParse(new string(hexString[num2 - 3 + num], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result3) || !byte.TryParse(new string(hexString[num2 - 2 + num], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result4) || !byte.TryParse(new string(hexString[num2 - 1 + num], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result5))
                    {
                        color = Empty;
                        return false;
                    }

                    color = new LvcColor(result3, result4, result5, result2);
                    return true;
                }
            case 6:
            case 8:
                {
                    if (!uint.TryParse(hexString.Substring(num), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
                    {
                        color = Empty;
                        return false;
                    }

                    var a = num2 == 6 ? (byte)255 : (byte)((result >> 24) & 255);
                    var r = (byte)((result >> 16) & 255);
                    var g = (byte)((result >> 8) & 255);
                    var b = (byte)(result & 255);

                    color = new LvcColor(r, g, b, a);

                    return true;
                }
            default:
                color = Empty;
                return false;
        }
    }

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
    public static LvcColor FromRGB(byte red, byte green, byte blue) =>
        new(red, green, blue);

    /// <summary>
    /// Creates a new instance of the <see cref="LvcColor"/> class with the given components.
    /// </summary>
    /// <param name="alpha">The alpha channel component from 0 to 255.</param>
    /// <param name="red">The red component from 0 to 255.</param>
    /// <param name="green">The green component from 0 to 255.</param>
    /// <param name="blue">The blue component from 0 to 255.</param>
    /// <returns></returns>
    public static LvcColor FromArgb(byte alpha, byte red, byte green, byte blue) =>
        new(red, green, blue, alpha);

    /// <summary>
    /// Creates a new instance of the <see cref="LvcColor"/> class with the given components.
    /// </summary>
    /// <param name="alpha">The alpha channel component from 0 to 255.</param>
    /// <param name="color">The red color.</param>
    /// <returns></returns>
    public static LvcColor FromArgb(byte alpha, LvcColor color) =>
        new(color.R, color.G, color.B, alpha);
}
