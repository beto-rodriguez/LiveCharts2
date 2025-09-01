// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// https://github.com/amerkoleci/Vortice.Windows

using System;
using System.Runtime.InteropServices;
using SharpGen.Runtime;

namespace Vortice;

public enum ShaderStage
{
    Vertex,
    Pixel,
    Compute
}

public static class ShaderCompiler
{
    public static byte[]? Compile(string source, ShaderStage stage, string entryPoint = "", string fileName = "")
    {
        if (string.IsNullOrEmpty(entryPoint))
        {
            entryPoint = GetDefaultEntryPoint(stage);
        }

        uint flags = 0;
        string shaderProfile = $"{GetShaderProfile(stage)}_5_0";
        Result hr = D3DCompiler.D3DCompile(
            source,
            source.Length,
            fileName,
            null,
            0,
            entryPoint,
            shaderProfile,
            flags,
            0,
            out IDxcBlob blob,
            out IDxcBlob? errorMsgs);

        if (hr.Failure)
        {
            if (errorMsgs != null)
            {
                var errorText = GetStringFromBlob(errorMsgs);
            }
        }
        else
        {
            return GetBytesFromBlob(blob);
        }

        return null;
    }

    private static string GetDefaultEntryPoint(ShaderStage stage)
    {
        switch (stage)
        {
            case ShaderStage.Vertex:
                return "VSMain";
            //case ShaderStage.Hull:
            //    return "HSMain";
            //case ShaderStage.Domain:
            //    return "DSMain";
            //case ShaderStage.Geometry:
            //    return "GSMain";
            case ShaderStage.Pixel:
                return "PSMain";
            case ShaderStage.Compute:
                return "CSMain";
            default:
                return string.Empty;
        }
    }

    private static string GetShaderProfile(ShaderStage stage)
    {
        switch (stage)
        {
            case ShaderStage.Vertex:
                return "vs";
            //case ShaderStage.Hull:
            //    return "hs";
            //case ShaderStage.Domain:
            //    return "ds";
            //case ShaderStage.Geometry:
            //    return "gs";
            case ShaderStage.Pixel:
                return "ps";
            case ShaderStage.Compute:
                return "cs";
            default:
                return string.Empty;
        }
    }

    private static unsafe string? GetStringFromBlob(IDxcBlob blob)
    {
        return Marshal.PtrToStringAnsi((IntPtr)blob.GetBufferPointer());
    }

    private static unsafe byte[] GetBytesFromBlob(IDxcBlob blob)
    {
        byte* pMem = (byte*)blob.GetBufferPointer();
        uint size = blob.GetBufferSize();
        byte[] result = new byte[size];
        fixed (byte* pTarget = result)
        {
            for (uint i = 0; i < size; ++i)
            {
                pTarget[i] = pMem[i];
            }
        }

        return result;
    }

    [ComImport]
    [Guid("8BA5FB08-5195-40e2-AC58-0D989C3A0102")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IDxcBlob
    {
        [PreserveSig]
        unsafe char* GetBufferPointer();

        [PreserveSig]
        uint GetBufferSize();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct D3D_SHADER_MACRO
    {
        [MarshalAs(UnmanagedType.LPStr)] string Name;
        [MarshalAs(UnmanagedType.LPStr)] string Definition;
    }

    private static class D3DCompiler
    {
        [DllImport("d3dcompiler_47.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public extern static Result D3DCompile(
            [MarshalAs(UnmanagedType.LPStr)] string srcData, int srcDataSize,
            [MarshalAs(UnmanagedType.LPStr)] string sourceName,
            [MarshalAs(UnmanagedType.LPArray)] D3D_SHADER_MACRO[] defines,
            int pInclude,
            [MarshalAs(UnmanagedType.LPStr)] string entryPoint,
            [MarshalAs(UnmanagedType.LPStr)] string target,
            uint Flags1,
            uint Flags2,
            out IDxcBlob code, out IDxcBlob? errorMsgs);
    }
}
