using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Devices;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace Shell._WinForms
{
    public partial class MainForm : Form
    {
        private readonly PerformanceCounter _receiveCounter;
        private readonly PerformanceCounter _sendCounter;
        private readonly PerformanceCounter _cpuCounter;
        private readonly PerformanceCounter _ramCounter;
        private readonly DriveInfo[] _drives;
        private static System.Timers.Timer? timer;

        private const string DOWNARROW = "\u2191";
        private const string UPARROW = "\u2193";
        private string _monitorText;
        private string _cpumonitor;
        private string _rammonitor;
        private string _upmonitor;
        private string _downmonitor;
        private int _monitorBlocks;
        private int _cpu;
        private int _ram;
        private int _up;
        private int _down;

        private string[] _cachedCmds;
        private string _cachedSt;
        private string _user;
        private string _domn;
        private string _path;
        private int _userln;
        private int _domnln;
        private int _pathln;

        public MainForm()
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _cpuCounter.NextValue();
            _ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            _ramCounter.NextValue();
            _sendCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", "Realtek USB GbE Family Controller");
            _sendCounter.NextValue();
            _receiveCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", "Realtek USB GbE Family Controller");
            _receiveCounter.NextValue();
            _drives = DriveInfo.GetDrives();

            _cachedCmds = new string[20];
            _user = $"{Environment.UserName}";
            _domn = $"{Environment.MachineName}";
            _path = $"{Directory.GetCurrentDirectory()}";
            _userln = _user.Length + 1;
            _domnln = _domn.Length + 1;
            _pathln = _path.Length + 1;

            InitializeComponent();
            //ShowMonitor();
            //ShowUserLine();
            //ShowShellHeader();
            // 43 tamaño mitad - 86 total
            //Shell.Text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ-ABCDEFGHIJKLMNOPQRSTUVWXYZ-ABCDEFGHIJKLMNOPQRSTUVWXYZ-ABCDE";
            //MessageBox.Show(Shell.Text.Length + "");

            Thread.Sleep(1000);
            timer = new System.Timers.Timer(2000);
            timer.Elapsed += UpdateMonitorValues;
            timer.AutoReset = true;
            timer.Start();
        }

        private void UpdateMonitorValues(object sender, ElapsedEventArgs e)
        {
            _monitorText = string.Empty;
            _monitorBlocks = 0;

            ReadComputerValues();
            ReadDiskValues();

            if (Monitor.InvokeRequired)
                Monitor.Invoke(new Action(() => UpdateMonitorText()));
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
            _upmonitor = $"  NET {UPARROW}   {GetUsageBar(_up)} {_up:00}% ";
            _downmonitor = $"  NET {DOWNARROW}   {GetUsageBar(_down)} {_down:00}% ";
            
            _monitorText = $"{_cpumonitor}{_rammonitor}\n{_upmonitor}{_downmonitor}\n";
            _monitorBlocks = 4;
        }

        private void ReadDiskValues()
        {
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
                    continue;
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
            Monitor.Clear();
            Monitor.Text = _monitorText;

            int start = 0;
            for (int i = 0; i < _monitorBlocks; i++)
            {
                ApplyMonitorStyle(start, 10, Color.Orange);
                ApplyMonitorStyle(start + 10, 1, Color.Pink);

                string text = Monitor.Text.Substring(start + 11, 25);
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
            Monitor.Select(start, length);
            Monitor.SelectionColor = color;
        }

        private void Shell_TextChanged(object sender, EventArgs e)
        {

        }

        private void Shell_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                //MessageBox.Show("Flecha arriba pulsada");
            }
            else if (e.KeyCode == Keys.Enter)
            {
                //MessageBox.Show("Enter pulsado");
                //e.Handled = true; // Opcional: evita que se agregue una nueva línea
            }
            else if (e.KeyCode == Keys.Down)
            {
                //MessageBox.Show("Flecha abajo pulsada");
            }
        }

        private void ShowMonitor()
        {
            string A = "  CPU     [AAAA BBBB CCCC DDDD EEEE ] 26% |";
            string B = "| RAM     [AAAA BBBB CCCC DDDD EEEE ] 75%  ";
            string C = "  DISK C  [AAAA BBBB CCCC DDDD EEEE ] 06% |";
            string D = "| DISK E  [AAAA BBBB CCCC DDDD EEEE ] 32%  ";
            string E = "  NET     [AAAA BBBB CCCC DDDD EEEE ] 87% |";
            string F = "|                                          ";
            Monitor.Text = A + B + Environment.NewLine + C + D + Environment.NewLine + E + F;

            Monitor.Select(0, 10);
            Monitor.SelectionColor = Color.Orange;

            Monitor.Select(10, 1);
            Monitor.SelectionColor = Color.Pink;

            Monitor.Select(11, 25);
            Monitor.SelectionColor = Color.Green;

            Monitor.Select(36, 2);
            Monitor.SelectionColor = Color.Pink;
                
            Monitor.Select(38, 2);
            Monitor.SelectionColor = Color.Cyan;

            Monitor.Select(40, 3);
            Monitor.SelectionColor = Color.YellowGreen;

        }

        private void ShowShellHeader()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            // Disco
            //var diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            //diskCounter.NextValue();
            //// RAM
            //var ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            //ramCounter.NextValue();

            DriveInfo drive = new DriveInfo("C");
            long totalDisk = drive.TotalSize;
            long freeDisk = drive.AvailableFreeSpace;
            long udiskUsage = totalDisk - freeDisk;
            float diskUsage = (float)(((udiskUsage / 1024) / 1024) / 1024);
            //Console.WriteLine($"Disco usado: {usedDisk / (1024 * 1024 * 1024)} GB de {totalDisk / (1024 * 1024 * 1024)} GB");

            var ci = new ComputerInfo();
            ulong totalRam = ci.TotalPhysicalMemory;
            ulong availableRam = ci.AvailablePhysicalMemory;
            ulong uramUsage = totalRam - availableRam;
            float ramUsage = (float)((uramUsage / 1024) / 1024);
            //Console.WriteLine($"RAM usada: {usedRam / (1024 * 1024)} MB de {totalRam / (1024 * 1024)} MB");

            //var netCounter = new PerformanceCounter("Network Interface", "Bytes Total/sec", "Realtek USB GbE Family Controller");
            Thread.Sleep(1000);

            float cpuUsage = cpuCounter.NextValue();
            //float ramUsage = ramCounter.NextValue();
            //float diskUsage = diskCounter.NextValue();
            //float netUsage = netCounter.NextValue();

            ShowBar(cpuUsage);
            ShowBar(ramUsage);
            ShowBar(diskUsage);

            //Console.WriteLine($"CPU: {cpuUsage:F2} %");
            //Console.WriteLine($"RAM: {ramUsage:F2} %");
            //Console.WriteLine($"Disco: {diskUsage:F2} %");
            //Console.WriteLine($"Red: {netUsage / 1024:F2} KB/s");

            /*
            // Sistema operativo
            Console.WriteLine($"Sistema Operativo: {Environment.OSVersion}");
            // Usuario
            Console.WriteLine($"Usuario: {Environment.UserName}");
            // Nombre de máquina
            Console.WriteLine($"Nombre de máquina: {Environment.MachineName}");
            // Procesador
            using (var searcher = new ManagementObjectSearcher("select * from Win32_Processor"))
            {
                foreach (var item in searcher.Get())
                    Console.WriteLine($"Procesador: {item["Name"]}");
            }
            // RAM
            using (var searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem"))
            {
                foreach (var item in searcher.Get())
                    Console.WriteLine($"RAM: {Math.Round(Convert.ToDouble(item["TotalPhysicalMemory"]) / (1024 * 1024 * 1024), 2)} GB");
            }
            // Disco duro
            using (var searcher = new ManagementObjectSearcher("select * from Win32_LogicalDisk where DriveType=3"))
            {
                foreach (var item in searcher.Get())
                    Console.WriteLine($"Disco: {item["DeviceID"]} - {Math.Round(Convert.ToDouble(item["Size"]) / (1024 * 1024 * 1024), 2)} GB");
            }
            // Tarjeta gráfica
            using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
            {
                foreach (var item in searcher.Get())
                    Console.WriteLine($"Tarjeta gráfica: {item["Name"]}");
            }
            // Red
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    Console.WriteLine($"Interfaz: {ni.Name}");
                    Console.WriteLine($"  Tipo: {ni.NetworkInterfaceType}");
                    Console.WriteLine($"  Velocidad: {ni.Speed / 1_000_000} Mbps");
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            Console.WriteLine($"  IP: {ip.Address}");
                    }
                }
            }
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) &&
                    !ni.Name.ToLower().Contains("vmware") &&
                    !ni.Name.ToLower().Contains("virtual") &&
                    !ni.Name.ToLower().Contains("loopback") &&
                    !ni.Name.ToLower().Contains("vethernet") &&
                    !ni.Name.ToLower().Contains("wsl") &&
                    !ni.Name.ToLower().Contains("hyper-v")
                    )
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                            !ip.Address.ToString().StartsWith("127."))
                        {
                            Console.WriteLine($"Interfaz real: {ni.Name}");
                            Console.WriteLine($"  IP: {ip.Address}");
                            Console.WriteLine($"  Velocidad: {ni.Speed / 1_000_000} Mbps");
                        }
                    }
                }
            }
            */
        }

        private void ShowBar(float usage)
        {
            string t = "";
            int bars = (int)usage / 2 +1;
            for (int i = 0; i < 50; i++)
            {
                if (i < bars)
                    t += "/";
                else
                    t += "·";
            }

            Shell.Text = $"[{t}]";
            Shell.Select(0, 1);
            Shell.SelectionColor = Color.Orange;

            Shell.Select(1, bars);
            Shell.SelectionColor = Color.Green;

            Shell.Select(bars + 1, 51 - bars);
            Shell.SelectionColor = Color.Gray;

            Shell.Select(51, 1);
            Shell.SelectionColor = Color.Orange;
        }

        private void ShowUserLine()
        {
            Shell.Text = $"{_user}@{_domn} {_path} >>> ";
            // Pinta el usuario
            Shell.Select(0, _userln - 1);
            Shell.SelectionColor = Color.Orange;
            // Pinta la arroba
            Shell.Select(_userln -1, 1);
            Shell.SelectionColor = Color.DeepPink;
            // Pinta el dominio
            Shell.Select(_userln, _domnln);
            Shell.SelectionColor = Color.Orange;
            // Pinta la ruta
            Shell.Select(_userln + _domnln, _pathln);
            Shell.SelectionColor = Color.Coral;
            // Pinta las flechas del final
            Shell.Select(_userln + _domnln + _pathln, 4);
            Shell.SelectionColor = Color.BlueViolet;
            // Devuelve el resto del texto al color original
            Shell.SelectionStart = Shell.Text.Length;
            Shell.SelectionLength = 0;

            Shell.SelectionColor = Color.Green;
        }
    }
}
