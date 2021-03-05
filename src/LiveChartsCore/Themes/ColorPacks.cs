// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Drawing;

namespace LiveChartsCore
{
    public static class ColorPacks
    {
        public static Color[] FluentDesign => new Color[]
        {
            RGB(116,77,169),
            RGB(231,72,86),
            RGB(1,133,116),
            RGB(0,153,188),
            RGB(191,0,119),
            RGB(255,140,0),
            RGB(194,57,179),
            RGB(76,74,72),
            RGB(0,183,195)
        };

        public static Color[] MaterialDesign500 => new Color[]
        {
            RGB(33,150,243),    // blue
            RGB(244,67,54),     // red
            RGB(139,195,74),    // light green
            RGB(0,188,212),     // cyan
            RGB(63,81,181),     // indigo
            RGB(255,193,7),     // ambar
            RGB(0,150,136),     // teal
            RGB(233,30,99),     // pink
            RGB(96,125,139),    // blue gray
        };

        public static Color[] MaterialDesign200 => new Color[]
        {
            RGB(144,202,249),    // blue
            RGB(239,154,154),     // red
            RGB(197,225,165),    // light green
            RGB(128,222,234),     // cyan
            RGB(159,168,218),     // indigo
            RGB(255,224,130),     // ambar
            RGB(128,203,196),     // teal
            RGB(244,143,177),     // pink
            RGB(176,190,197),    // blue gray
        };

        public static Color[] MaterialDesign800 => new Color[]
        {
            RGB(21,101,192),    // blue
            RGB(198,40,40),     // red
            RGB(85,139,47),    // light green
            RGB(0,131,143),     // cyan
            RGB(40,53,147),     // indigo
            RGB(255,143,0),     // ambar
            RGB(0,105,92),     // teal
            RGB(173,20,87),     // pink
            RGB(55,71,79),    // blue gray
        };

        public static Color[] Blues => new Color[]
        {
            RGB(13,71,161),
            RGB(25,118,210),
            RGB(100,181,246),
            RGB(187,222,251)
        };

        public static Color[] Reds => new Color[]
        {
            RGB(183,28,28),
            RGB(211,47,47),
            RGB(244,67,54),
            RGB(229,115,115)
        };

        public static Color[] Greens => new Color[]
        {
            RGB(51,105,30),
            RGB(104,159,56),
            RGB(76,175,80),
            RGB(174,213,129)
        };

        public static Color[] Grays => new Color[]
        {
            RGB(38,50,56),
            RGB(69,90,100),
            RGB(96,125,139),
            RGB(144,164,174)
        };

        public static Color[] Energy => new Color[]
        {
            RGB(17,29,94),
            RGB(199,0,57),
            RGB(243,113,33),
            RGB(192,226,24)
        };

        public static Color[] Nature => new Color[]
        {
            RGB(133,96,63),
            RGB(158,117,64),
            RGB(189,147,84),
            RGB(227,209,138)
        };


        public static Color[] Cloudy => new Color[]
        {
            RGB(176,136,249),
            RGB(190,220,250),
            RGB(152,172,248),
            RGB(218,159,249)
        };

        public static Color[] Candy => new Color[]
        {
            RGB(97,85,166),
            RGB(166,133,226),
            RGB(255,171,225),
            RGB(255,230,230)
        };

        public static Color[] Fishy => new Color[]
        {
            RGB(255,146,146),
            RGB(255,180,180),
            RGB(255,220,220),
            RGB(255,232,232)
        };

        public static Color[] LateSummer => new Color[]
        {
            RGB(130,38,89),
            RGB(179,65,128),
            RGB(227,107,174),
            RGB(248,161,209)
        };

        public static Color RGB(byte r, byte g, byte b) => Color.FromArgb(255, r, g, b);
    }
}