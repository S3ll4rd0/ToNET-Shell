using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Shell._WinForms
{
    internal class MonitorController
    {
        private readonly PerformanceCounter _receiveCounter;
        private readonly PerformanceCounter _sendCounter;
        private readonly PerformanceCounter _cpuCounter;
        private readonly PerformanceCounter _ramCounter;
        private readonly RichTextBox _monitor;
        private DriveInfo[] _newDrives;
        private DriveInfo[] _drives;
        private static System.Timers.Timer? timer;

        private const string DOWNARROW = "\u2191";
        private const string UPARROW = "\u2193";
        private string _monitorText = string.Empty;
        private string _cpumonitor = string.Empty;
        private string _rammonitor = string.Empty;
        private string _upmonitor = string.Empty;
        private string _downmonitor = string.Empty;
        private int _monitorBlocks = 0;
        private int _cpu = 0;
        private int _ram = 0;
        private int _up = 0;
        private int _down = 0;

        public MonitorController(RichTextBox monitor)
        {
            _monitor = monitor;

            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _cpuCounter.NextValue();
            _ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            _ramCounter.NextValue();
            _sendCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", "Realtek USB GbE Family Controller");
            _sendCounter.NextValue();
            _receiveCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", "Realtek USB GbE Family Controller");
            _receiveCounter.NextValue();
            _drives = DriveInfo.GetDrives();

            Thread.Sleep(1000);
            UpdateMonitorValues(null, null);
            timer = new System.Timers.Timer(2000);
            timer.Elapsed += UpdateMonitorValues;
            timer.AutoReset = true;
            timer.Start();
        }

        private void UpdateMonitorValues(object? sender, ElapsedEventArgs? e)
        {
            _monitorText = string.Empty;
            _monitorBlocks = 0;

            ReadComputerValues();
            ReadDiskValues();

            if (_monitor.InvokeRequired)
                _monitor.Invoke(new Action(() => UpdateMonitorText()));
            else
                UpdateMonitorText();
        }

        private void ReadComputerValues()
        {
            _cpu = Convert.ToInt32(_cpuCounter.NextValue());
            _ram = Convert.ToInt32(_ramCounter.NextValue());
            _up = Convert.ToInt32(_sendCounter.NextValue()) / 1024;
            _down = Convert.ToInt32(_receiveCounter.NextValue()) / 1024;

            _cpumonitor = $"  CPU     {GetUsageBar(_cpu)} {_cpu:00}% ";
            _rammonitor = $"  RAM     {GetUsageBar(_ram)} {_ram:00}% ";
            _upmonitor  = $"  NET {UPARROW}   {GetUsageBar(_up)} {_up:00}% ";
            _downmonitor = $"  NET {DOWNARROW}   {GetUsageBar(_down)} {_down:00}% ";

            _monitorText = $"{_cpumonitor}{_rammonitor}\n{_upmonitor}{_downmonitor}\n";
            _monitorBlocks = 4;
        }

        private void ReadDiskValues()
        {
            bool error = false;

            for (int i = 0; i < _drives.Length; i++)
            {
                try
                {
                    DriveInfo drive = _drives[i];
                    string dName = drive.Name[..1];
                    float totalDisk = Convert.ToSingle(drive.TotalSize / 1024 / 1024 / 1024);
                    float freeDisk = Convert.ToSingle(drive.AvailableFreeSpace / 1024 / 1024 / 1024);
                    int usedDisk = Convert.ToInt32((totalDisk - freeDisk) * 100 / totalDisk);

                    if ((i % 2) == 0)
                        _monitorText += $"  UNIT {dName}  {GetUsageBar(usedDisk)} {usedDisk:00}% ";
                    else
                        _monitorText += $"  UNIT {dName}  {GetUsageBar(usedDisk)} {usedDisk:00}% \n";
                    _monitorBlocks++;
                }
                catch (Exception)
                {
                    _newDrives = _drives.Where((value, index) => index != i).ToArray();
                    error = true;
                    continue;
                }
                finally
                {
                    if (error)
                        _drives = _newDrives;
                }
            }
        }

        private static string GetUsageBar(int usage)
        {
            string bar = string.Empty;
            int bars = usage / 4 < 1 ? 1 : usage / 4;
            for (int i = 0; i < 25; i++)
                if (i < bars)
                    bar += "/";
                else
                    bar += "·";
            return $"[{bar}]";
        }

        private void UpdateMonitorText()
        {
            _monitor.Clear();
            _monitor.Text = _monitorText;

            int start = 0;
            for (int i = 0; i < _monitorBlocks; i++)
            {
                ApplyMonitorStyle(start, 10, Color.Orange);
                ApplyMonitorStyle(start + 10, 1, Color.Pink);

                string text = _monitor.Text.Substring(start + 11, 25);
                int bars = text.LastIndexOf('/') + 1;
                int dots = 25 - bars;

                ApplyMonitorStyle(start + 11, bars, Color.Red);
                ApplyMonitorStyle(start + 11 + bars, dots, Color.DarkGreen);
                ApplyMonitorStyle(start + 36, 1, Color.Pink);
                ApplyMonitorStyle(start + 37, 3, Color.Cyan);
                ApplyMonitorStyle(start + 40, 1, Color.YellowGreen);

                if (i % 2 == 0)
                    start += 42;
                else
                    start += 43;
            }
        }

        private void ApplyMonitorStyle(int start, int length, Color color)
        {
            _monitor.Select(start, length);
            _monitor.SelectionColor = color;
        }
    }
}
