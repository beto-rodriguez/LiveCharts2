// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// https://github.com/amerkoleci/Vortice.Windows

using System.Runtime.InteropServices;
using SharpGen.Runtime;
using Vortice.Mathematics;

namespace VorticeSample;

public class ColorDrawingEffect : ComObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorDrawingEffect"/> class.
    /// </summary>
    /// <param name="color">The color.</param>
    public ColorDrawingEffect(in Color4 color)
    {
        Color = color;
        NativePointer = Marshal.GetIUnknownForObject(this);
    }

    /// <summary>
    /// Gets the color.
    /// </summary>
    /// <value>The color.</value>
    public Color4 Color { get; }
}
