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

using System.Runtime.CompilerServices;

#if DEBUG

[assembly: InternalsVisibleTo("LiveChartsBackersPackage")]

#endif

#if RELEASE

[assembly: AssemblyKeyFile("./../../../../../../nupkgs/keypair.snk")]
[assembly: InternalsVisibleTo("LiveChartsBackersPackage, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5845b22b111de8f9504daf136f975d790af5361b9612a3dea48fc3480087efa128adbfe0267ee4f894d8985e8b9c55f6734896e192fa647e142e1aadf281131d2329b345e456bbb6ff5b3ef4bf8da574022d0e9618a68c9241a61350bb505b6ae6a49f6c3d45be7aa76f5646574d40870c22d648d57765e040cac6e8b21c3d3")]

#endif

