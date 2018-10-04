using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MineSweeper.ViewModel
{
    public class StopwatchViewModel :
        INotifyPropertyChanged
    {
        private static readonly TimeSpan UPDATE_INTERVEL =
            TimeSpan.FromSeconds(1);

        private Stopwatch Stopwatch;

        private Timer ElaspedUpdater;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Elasped => Convert.ToInt32(Stopwatch.Elapsed.TotalSeconds).ToString("0000");

        public StopwatchViewModel()
        {
            Stopwatch = new Stopwatch();
            ElaspedUpdater = new Timer()
            {
                Interval = UPDATE_INTERVEL.TotalMilliseconds
            };
            ElaspedUpdater.Elapsed += OnElaspedUpdate;
        }

        public void Start()
        {
            Stopwatch.Start();
            ElaspedUpdater.Start();
        }

        public void Stop()
        {
            Stopwatch.Stop();
            ElaspedUpdater.Stop();
        }

        public void Reset()
        {
            Stopwatch.Reset();
            ElaspedUpdater.Stop();
            OnElaspedUpdate(default, default);
        }

        private void OnElaspedUpdate(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(Elasped));
        }

        private void OnPropertyChanged([CallerMemberName]string name = "")
        {
            if(!CachedNames.TryGetValue(name, out var arg))
            {
                arg = new PropertyChangedEventArgs(name);
            }
            PropertyChanged?.Invoke(this, arg);
        }

        private static readonly IReadOnlyDictionary<string, PropertyChangedEventArgs> CachedNames =
            new Dictionary<string, PropertyChangedEventArgs>()
            {
                {nameof(Elasped), new PropertyChangedEventArgs(nameof(Elasped)) },
            };
    }
}
