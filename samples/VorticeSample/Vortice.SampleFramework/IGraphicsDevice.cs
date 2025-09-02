// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// https://github.com/amerkoleci/Vortice.Windows

using System;
using System.Runtime.CompilerServices;

namespace Vortice;

public interface IGraphicsDevice : IDisposable
{
    bool DrawFrame(Action<int, int> draw, [CallerMemberName] string? frameName = null);
}
