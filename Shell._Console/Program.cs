using System;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;

class Program
{
    //private static ConsoleStyle _console;
    //static void Main()
    //{
    //    ConsoleStyle.LoadAsciiImage();
    //}

    static void Main()
    {
        var category = new PerformanceCounterCategory("Network Interface");
        string[] instanceNames = category.GetInstanceNames();
        foreach (var name in instanceNames)
        {
            Console.WriteLine(name);
        }

        category = new PerformanceCounterCategory("Network Interface");
        string[] counters = category.GetCounters("NOMBRE_DE_LA_INTERFAZ").Select(c => c.CounterName).ToArray();
        foreach (var name in counters)
            Console.WriteLine(name);

        // CPU
        var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        cpuCounter.NextValue();
        // Disco
        var diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
        diskCounter.NextValue();
        // RAM
        var ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        // Red (pon el nombre exacto de tu interfaz de red)
        string networkInterface = "Realtek USB GbE Family Controller"; // Cambia esto por el nombre de tu interfaz real
        var netCounter = new PerformanceCounter("Network Interface", "Bytes Total/sec", networkInterface);
        netCounter.NextValue();

        // Espera un segundo para obtener valores reales
        Thread.Sleep(1000);

        float cpuUsage = cpuCounter.NextValue();
        float ramUsage = ramCounter.NextValue();
        float diskUsage = diskCounter.NextValue();
        float netUsage = netCounter.NextValue();

        Console.WriteLine($"CPU: {cpuUsage:F2} %");
        Console.WriteLine($"RAM: {ramUsage:F2} %");
        Console.WriteLine($"Disco: {diskUsage:F2} %");
        Console.WriteLine($"Red: {netUsage / 1024:F2} KB/s");


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
    }
}