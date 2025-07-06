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

namespace LiveChartsCore.Generators;

/// <summary>
/// Represents a pair of parameters used to configure a command or operation.
/// </summary>
/// <remarks>This class is designed to encapsulate two related parameters, allowing them to be passed together as
/// a single object. It is generic, enabling flexibility in the types of parameters used.</remarks>
/// <typeparam name="T1">The type of the first parameter.</typeparam>
/// <typeparam name="T2">The type of the second parameter.</typeparam>
/// <param name="parameter1"></param>
/// <param name="parameter2"></param>
public class CommandParameters<T1, T2>(T1 parameter1, T2 parameter2)
{
    /// <summary/>
    public T1 Parameter1 { get; set; } = parameter1;

    /// <summary/>
    public T2 Parameter2 { get; set; } = parameter2;
}

/// <summary>
/// Represents a set of parameters for a command, encapsulating three values of potentially different types.
/// </summary>
/// <remarks>This class is designed to hold three related values that can be passed together as parameters to a
/// command or operation. It is generic, allowing flexibility in the types of the parameters.</remarks>
/// <typeparam name="T1">The type of the first parameter.</typeparam>
/// <typeparam name="T2">The type of the second parameter.</typeparam>
/// <typeparam name="T3">The type of the third parameter.</typeparam>
/// <param name="parameter1"></param>
/// <param name="parameter2"></param>
/// <param name="parameter3"></param>
public class CommandParameters<T1, T2, T3>(T1 parameter1, T2 parameter2, T3 parameter3)
{
    /// <summary/>
    public T1 Parameter1 { get; set; } = parameter1;

    /// <summary/>
    public T2 Parameter2 { get; set; } = parameter2;

    /// <summary/>
    public T3 Parameter3 { get; set; } = parameter3;
}

/// <summary>
/// Represents a container for a set of four command parameters, each of a potentially different type.
/// </summary>
/// <remarks>This class is designed to hold and provide access to four related parameters, which can be of
/// different types. It is commonly used in scenarios where multiple values need to be grouped together and passed as a
/// single unit.</remarks>
/// <typeparam name="T1">The type of the first parameter.</typeparam>
/// <typeparam name="T2">The type of the second parameter.</typeparam>
/// <typeparam name="T3">The type of the third parameter.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter.</typeparam>
/// <param name="parameter1"></param>
/// <param name="parameter2"></param>
/// <param name="parameter3"></param>
/// <param name="parameter4"></param>
public class CommandParameters<T1, T2, T3, T4>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
{
    /// <summary/>
    public T1 Parameter1 { get; set; } = parameter1;
    /// <summary/>
    public T2 Parameter2 { get; set; } = parameter2;
    /// <summary/>
    public T3 Parameter3 { get; set; } = parameter3;
    /// <summary/>
    public T4 Parameter4 { get; set; } = parameter4;
}

/// <summary/>
public static class CommandParameters
{
    /// <summary/>
    public static CommandParameters<T1, T2> Create<T1, T2>(T1 first, T2 second)
        => new(first, second);

    /// <summary/>
    public static CommandParameters<T1, T2, T3> Create<T1, T2, T3>(T1 first, T2 second, T3 third)
        => new(first, second, third);

    /// <summary/>
    public static CommandParameters<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
        => new(first, second, third, fourth);
}
