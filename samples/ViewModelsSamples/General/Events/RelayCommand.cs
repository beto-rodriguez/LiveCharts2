using LiveChartsCore.Kernel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace ViewModelsSamples.General.Events
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var points = (IEnumerable<ChartPoint>)parameter;

            // the command passes a collection of the point that were triggered by the pointer down event.
            foreach (var point in points)
            {
                Trace.WriteLine(point.Context.DataSource);
            }
        }
    }
}
