using System.Numerics;
using Vortice.Mathematics;

namespace Vortice;

public readonly struct VertexPositionColor(in Vector3 position, in Color4 color)
{
    public static readonly unsafe uint SizeInBytes = (uint)sizeof(VertexPositionColor);
    public readonly Vector3 Position = position;
    public readonly Color4 Color = color;
}
