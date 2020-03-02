using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessNote.DataGathering
{
    public class ProcessItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProcessStartInfo Start { get; internal set; }
        public Process Process { get; set; }
        public ProcessThreadCollection Threads { get; set; }
        public List<string> Comments = new List<string>();

        private DateTime StartTime { get; set; }
        private TimeSpan RunningTime { get; set; }

        private long Memory { get; set; }

        public string CPUUsage { get; set; }

        public void SetThreads()
        {
            Threads = Process.Threads;
        }

        public void SetCPUUsage()
        {
            Task CPUCalculate = new Task(() =>
            {
                PerformanceCounter performanceCounter = new PerformanceCounter("Process", "% Processor Time", Process.ProcessName);
                performanceCounter.NextValue();
                Thread.Sleep(1000);
                float Usage = performanceCounter.NextValue();
                try
                {
                    CPUUsage = Math.Round(Usage, 1).ToString() + " %";
                }
                catch (Win32Exception)
                {
                    CPUUsage = "";
                }

            });
            CPUCalculate.Start();
        }

        public void SetMemory()
        {
            try
            {
                Memory = Process.WorkingSet64;
            }
            catch (Win32Exception)
            {
                Memory = 0;
            }
            catch (InvalidOperationException)
            {
                Memory = 0;
            }
        }

        public void SetRunningtTime()
        {
            try
            {
                StartTime = Process.StartTime;
                RunningTime = DateTime.Now - StartTime;
            }
            catch (Win32Exception)
            {
                StartTime = DateTime.Now;
                RunningTime = new TimeSpan(1, 12, 23, 62);
            }
            catch (InvalidOperationException)
            {
                RunningTime = new TimeSpan(0, 0, 0, 0);
            }
        }

        public string GetRunningTime()
        {
            return $"{RunningTime}";
        }

        public string GetStartTime()
        {
            return $"{ StartTime }";
        }

        public string ConvertBytesToMB()
        {
            double result = Math.Round(Memory / 1024f / 1024f, 2);
            return $"{ result } mb";
        }

        public ProcessThreadCollection GetThreads()
        {
            return Threads;
        }
    }
}
