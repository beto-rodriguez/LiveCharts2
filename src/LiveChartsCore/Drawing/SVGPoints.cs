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
/// A collections of SVG paths.
/// </summary>
public static class SVGPoints
{
    // icons from https://www.svgrepo.com/

    /// <summary>
    /// A diamond.
    /// </summary>
    public static string Diamond =>
        "M7.92950166,15.644 L3.34050166,8.796 C2.88750166,8.342 2.88650166,7.608 3.33750166,7.155 " +
        "L7.90350166,0.323 C8.35450166,-0.13 9.08950166,-0.128 9.54550166,0.325 L14.0865017,7.175 " +
        "C14.5405017,7.628 14.5415017,8.363 14.0895017,8.816 L9.57150166,15.646 C9.57150166,15.646 " +
        "8.38450166,16.097 7.92950166,15.644 Z";

    /// <summary>
    /// A gem.
    /// </summary>
    public static string Gem =>
        "M11.7831 0H3.21691L0.0712554 5.24275C-0.0349835 5.41982 -0.0214476 5.64398 0.105326 5.80697L7.10533 " +
        "14.807C7.20005 14.9288 7.34571 15 7.5 15C7.6543 15 7.79995 14.9288 7.89468 14.807L14.8947 " +
        "5.80697C15.0215 5.64398 15.035 5.41982 14.9287 5.24275L11.7831 0Z";

    /// <summary>
    /// A star.
    /// </summary>
    public static string Star =>
        "M11.0137 2.76683C11.379 1.89022 12.6208 1.89022 12.9861 2.76683L14.9102 7.38462C15.0654 " +
        "7.75726 15.4295 8 15.8332 8H20.893C21.8234 8 22.2893 9.12483 21.6314 9.78268L17.5391 13.875C17.2823 " +
        "14.1318 17.185 14.5076 17.2847 14.8568L18.9076 20.5369C19.1816 21.496 18.1122 22.2767 17.2822 " +
        "21.7234L12.5546 18.5716C12.2187 18.3477 11.7811 18.3477 11.4452 18.5717L6.72544 21.7182C5.89284 " +
        "22.2732 4.81988 21.49 5.09479 20.5279L6.71509 14.8568C6.81486 14.5076 6.71747 14.1318 6.46068 " +
        "13.875L2.38859 9.8029C1.72328 9.13758 2.19448 8 3.13538 8H8.16658C8.57028 8 8.93438 7.75726 9.08965 " +
        "7.38462L11.0137 2.76683Z";

    /// <summary>
    /// A heart.
    /// </summary>
    public static string Heart =>
        "M5.36129 3.46995C6.03579 3.16081 6.76287 3 7.50002 3C8.23718 3 8.96425 3.16081 9.63875 3.46995C10.3129 " +
        "3.77893 10.9185 4.22861 11.4239 4.78788C11.7322 5.12902 12.2678 5.12902 12.5761 4.78788C13.5979 3.65726 " +
        "15.0068 3.00001 16.5 3.00001C17.9932 3.00001 19.4021 3.65726 20.4239 4.78788C21.4427 5.91515 22 7.42425 " +
        "22 8.9792C22 10.5342 21.4427 12.0433 20.4239 13.1705L14.2257 20.0287C13.0346 21.3467 10.9654 21.3467 9.77429 " +
        "20.0287L3.57613 13.1705C3.07086 12.6115 2.67474 11.9531 2.40602 11.2353C2.13731 10.5175 2 9.75113 2 8.9792C2 " +
        "8.20728 2.13731 7.44094 2.40602 6.72315C2.67474 6.00531 3.07086 5.34694 3.57613 4.78788C4.08157 4.22861 " +
        "4.68716 3.77893 5.36129 3.46995Z";

    /// <summary>
    /// A pin.
    /// </summary>
    public static string Pin =>
        "M174,5248.219 C172.895,5248.219 172,5247.324 172,5246.219 C172,5245.114 172.895,5244.219 174,5244.219 " +
        "C175.105,5244.219 176,5245.114 176,5246.219 C176,5247.324 175.105,5248.219 174,5248.219 M174,5239 " +
        "C170.134,5239 167,5242.134 167,5246 C167,5249.866 174,5259 174,5259 C174,5259 181,5249.866 181,5246 " +
        "C181,5242.134 177.866,5239 174,5239";

    /// <summary>
    /// A square.
    /// </summary>
    public static string Square =>
        "M4.281 3h16.437A1.281 1.281 0 0 1 22 4.281v16.437A1.282 1.282 0 0 1 20.718 22H4.282A1.282 1.282 0 0 1 3 " +
        "20.718V4.281A1.281 1.281 0 0 1 4.281 3z";

    /// <summary>
    /// A circle.
    /// </summary>
    public static string Circle =>
        "M2 12C2 6.47715 6.47715 2 12 2C17.5228 2 22 6.47715 22 12C22 17.5228 17.5228 22 12 22C6.47715 22 2 17.5228 " +
        "2 12Z";

    /// <summary>
    /// A pentagon.
    /// </summary>
    public static string Pentagon =>
        "M10.1259 2.21864C11.2216 1.34212 12.7784 1.34212 13.8741 2.21864L21.5041 8.32262C22.5088 9.12637 22.8891 " +
        "10.4813 22.4494 11.6905L19.4185 20.0252C18.9874 21.2108 17.8607 22 16.5992 22H7.40087C6.13935 22 5.01261 " +
        "21.2108 4.58149 20.0252L1.55067 11.6905C1.11097 10.4813 1.49127 9.12637 2.49596 8.32262L10.1259 2.21864Z";

    /// <summary>
    /// A cross mark.
    /// </summary>
    public static string Cross =>
        "M12,2A10,10,0,1,0,22,12,10,10,0,0,0,12,2Zm3.71,12.29a1,1,0,0,1,0,1.42,1,1,0,0,1-1.42,0L12,13.42,9.71,15.71" +
        "a1,1,0,0,1-1.42,0,1,1,0,0,1,0-1.42L10.58,12,8.29,9.71A1,1,0,0,1,9.71,8.29L12,10.58l2.29-2.29" +
        "a1,1,0,0,1,1.42,1.42L13.42,12Z";

    /// <summary>
    /// A check mark.
    /// </summary>
    public static string Check =>
        "M2 12C2 6.47715 6.47715 2 12 2C17.5228 2 22 6.47715 22 12C22 17.5228 17.5228 22 12 22C6.47715 22 2 17.5228 " +
        "2 12ZM15.7071 9.29289C16.0976 9.68342 16.0976 10.3166 15.7071 10.7071L12.0243 14.3899C11.4586 14.9556 " +
        "10.5414 14.9556 9.97568 14.3899L8.29289 12.7071C7.90237 12.3166 7.90237 11.6834 8.29289 11.2929C8.68342 " +
        "10.9024 9.31658 10.9024 9.70711 11.2929L11 12.5858L14.2929 9.29289C14.6834 8.90237 15.3166 8.90237 15.7071 " +
        "9.29289Z";
}
