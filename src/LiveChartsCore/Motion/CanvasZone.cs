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

using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Painting;

namespace LiveChartsCore.Motion;

internal class CanvasZone
{
    public const int NoClip = -1;
    public const int DrawMargin = 0;
    public const int XCrosshair = 1;
    public const int YCrosshair = 2;

    public int StateId { get; set; } = -1;
    public LvcRectangle Clip { get; set; }
    private HashSet<Paint> PaintTasks { get; set; } = [];

    // ToDo: do not sort on every call.
    public IEnumerable<Paint> EnumerateTasks() =>
        PaintTasks.Where(x => x is not null && x != Paint.Default).OrderBy(x => x.ZIndex);

    public void AddTask(Paint task)
    {
        if (task is null || task == Paint.Default) return;
        _ = PaintTasks.Add(task);
    }

    public bool RemoveTask(Paint task) =>
        PaintTasks.Remove(task);

    public void ClearTasks() =>
        PaintTasks = [];

    public int CountTasks() =>
        PaintTasks.Count;
}
