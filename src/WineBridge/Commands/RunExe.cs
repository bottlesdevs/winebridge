using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace WineBridge.Commands;

internal class RunExe : ICommand
{
    public void Execute(string[]? args)
    {
        if (args is null || args.Length == 0)
        {
            Console.WriteLine("Error. Usage: WineBridge.exe runExe [--shell] fullPathToExe [...]");
            return;
        }

        var useShell = false;
        var index = 0;

        if (args[0].Equals("--shell", StringComparison.OrdinalIgnoreCase))
        {
            useShell = true;
            index++;
        }

        if (index >= args.Length)
        {
            Console.WriteLine("Error. Usage: WineBridge.exe runExe [--shell] fullPathToExe [...]");
            return;
        }

        var raw = args[index]?.Trim().Trim('"', '\'') ?? string.Empty;

        var m = Regex.Match(
            raw,
            @"[A-Za-z]:\\[^:]+?\.exe",
            RegexOptions.IgnoreCase
        );

        if (!m.Success)
        {
            Console.WriteLine("Error: no valid Windows executable path found.");
            return;
        }

        var executable = m.Value;
        var cwd = Path.GetDirectoryName(executable) ?? string.Empty;

        var remaining = raw.Substring(m.Index + m.Length).Trim();
        var arguments = remaining;

        Console.WriteLine("[RunExe] Mode:     " + (useShell ? "shell" : "direct"));
        Console.WriteLine("[RunExe] FileName: " + executable);
        Console.WriteLine("[RunExe] CWD:      " + cwd);
        Console.WriteLine("[RunExe] Args:     " + arguments);

        try
        {
            if (useShell)
            {
                RunViaShell(executable, arguments, cwd);
            }
            else
            {
                RunDirect(executable, arguments, cwd);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error launching process: " + ex.Message);
        }
    }

    private static void RunDirect(string executable, string arguments, string cwd)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = cwd,
            UseShellExecute = false,
            Arguments = arguments
        };

        Process.Start(startInfo);
    }

    private static void RunViaShell(string executable, string arguments, string cwd)
    {
        var info = new SHELLEXECUTEINFOW
        {
            cbSize = Marshal.SizeOf<SHELLEXECUTEINFOW>(),
            fMask = SEE_MASK_NOCLOSEPROCESS,
            hwnd = IntPtr.Zero,
            lpVerb = "open",
            lpFile = executable,
            lpParameters = arguments,
            lpDirectory = string.IsNullOrWhiteSpace(cwd) ? null : cwd,
            nShow = SW_SHOWNORMAL,
            hInstApp = IntPtr.Zero,
            lpIDList = IntPtr.Zero,
            lpClass = null,
            hkeyClass = IntPtr.Zero,
            dwHotKey = 0,
            hIcon = IntPtr.Zero,
            hProcess = IntPtr.Zero
        };

        if (!ShellExecuteExW(ref info))
        {
            var err = Marshal.GetLastWin32Error();
            Console.WriteLine("ShellExecuteExW failed. Win32 error: " + err);
            return;
        }

        if (info.hProcess != IntPtr.Zero)
        {
            // TODO: this might be introduced with a ne flag, in case we need to wait for the procss to exit
        }
    }

    private const uint SEE_MASK_NOCLOSEPROCESS = 0x00000040;
    private const int SW_SHOWNORMAL = 1;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct SHELLEXECUTEINFOW
    {
        public int cbSize;
        public uint fMask;
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpVerb;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpFile;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpParameters;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpDirectory;
        public int nShow;
        public IntPtr hInstApp;
        public IntPtr lpIDList;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpClass;
        public IntPtr hkeyClass;
        public uint dwHotKey;
        public IntPtr hIcon;
        public IntPtr hProcess;
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShellExecuteExW(ref SHELLEXECUTEINFOW lpExecInfo);
}
