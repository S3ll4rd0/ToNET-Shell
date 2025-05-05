namespace Shell._WinForms
{
    public partial class MainForm : Form
    {
        private MonitorController _monitorController;

        private string[] _cachedCmds;
        private string _userLine;
        private string _cachedSt;
        private string _input;
        private string _user;
        private string _domn;
        private string _path;
        private int _userln;
        private int _domnln;
        private int _pathln;

        private string[] args;

        public MainForm()
        {
            InitializeComponent();
            _monitorController = new(this.Monitor);

            _cachedCmds = new string[20];
            _user = $"{Environment.UserName}";
            _domn = $"{Environment.MachineName}";
            _path = $"{Directory.GetCurrentDirectory()}";
            _userln = _user.Length + 1;
            _domnln = _domn.Length + 1;
            _pathln = _path.Length + 1;

            //ShowMonitor();
            ShowUserLine();
            //ShowShellHeader();
            // 43 tamaño mitad - 86 total
            //Shell.Text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ-ABCDEFGHIJKLMNOPQRSTUVWXYZ-ABCDEFGHIJKLMNOPQRSTUVWXYZ-ABCDE";
            //MessageBox.Show(Shell.Text.Length + "");
        }

        private void ShowShellHeader()
        {
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

        private void Shell_TextChanged(object sender, EventArgs e)
        {
            _input = Shell.Text[_userLine.Length..];
            if (_input.Length > 0 && _input.Contains(' '))
                args = _input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
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
            else if (e.KeyCode == Keys.Space)
            {
                //MessageBox.Show("Espacio pulsado");
            }
        }

        private void ShowUserLine()
        {
            _userLine = $"{_user}@{_domn} {_path} >>> ";
            Shell.Text = _userLine;
            // Pinta el usuario
            ApplyShellStyle(0, _userln - 1, Color.GreenYellow);
            // Pinta la arroba
            ApplyShellStyle(_userln - 1, 1, Color.DeepPink);
            // Pinta el dominio
            ApplyShellStyle(_userln, _domnln, Color.LimeGreen);
            // Pinta la ruta
            ApplyShellStyle(_userln + _domnln, _pathln, Color.Coral);
            // Pinta las flechas del final
            ApplyShellStyle(_userln + _domnln + _pathln, 4, Color.BlueViolet);
            // Devuelve el resto del texto al color original
            Shell.SelectionStart = Shell.Text.Length;
            Shell.SelectionLength = 0;
            Shell.SelectionColor = Color.Green;
        }

        private void ApplyShellStyle(int start, int length, Color color)
        {
            Shell.Select(start, length);
            Shell.SelectionColor = color;
        }
    }
}
