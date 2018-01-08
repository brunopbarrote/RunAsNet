using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RunAsNet
{
    public static class ApiLogon
    {
        [Flags]
        enum LogonFlags
        {
            LOGON_WITH_PROFILE = 0x00000001,
            LOGON_NETCREDENTIALS_ONLY = 0x00000002
        }

        [Flags]
        enum CreationFlags
        {
            CREATE_SUSPENDED = 0x00000004,
            CREATE_NEW_CONSOLE = 0x00000010,
            CREATE_NEW_PROCESS_GROUP = 0x00000200,
            CREATE_UNICODE_ENVIRONMENT = 0x00000400,
            CREATE_SEPARATE_WOW_VDM = 0x00000800,
            CREATE_DEFAULT_ERROR_MODE = 0x04000000,
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ProcessInfo
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct StartupInfo
        {
            public int cb;
            public string reserved1;
            public string desktop;
            public string title;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public ushort wShowWindow;
            public short reserved2;
            public int reserved3;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        static extern bool CreateProcessWithLogonW(
            string principal,
            string authority,
            string password,
            LogonFlags logonFlags,
            string appName,
            string cmdLine,
            CreationFlags creationFlags,
            IntPtr environmentBlock,
            string currentDirectory,
            ref StartupInfo startupInfo,
            out ProcessInfo processInfo);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr h);

        static LogonCredential CreateLogonCredential()
        {
            var regEx = new Regex("^(/|-)(?<name>\\w+)(?:\\:(?<value>.+)$|\\:$|$)", RegexOptions.Compiled);

            var logonCredential = new LogonCredential();

            foreach (string arg in Environment.GetCommandLineArgs())
            {
                var namedArg = regEx.Match(arg);

                if (namedArg.Success)
                {
                    switch (namedArg.Groups["name"].ToString().ToLower())
                    {
                        case "user":

                            var user = namedArg.Groups["value"].ToString();

                            logonCredential.Domain = user.Split('\\').GetValue(0).ToString();

                            logonCredential.Username = user.Split('\\').GetValue(1).ToString();

                            break;
                        case "pass":

                            logonCredential.Password = namedArg.Groups["value"].ToString();

                            break;
                        case "command":

                            logonCredential.Command = namedArg.Groups["value"].ToString();

                            break;
                        default:
                            break;
                    }
                }
            }

            return logonCredential;
        }

        public static bool CreateProcessWithLogonWNetOnly()
        {
            var logonCredential = CreateLogonCredential();

            if (!logonCredential.IsValid())
            {
                throw new OperationCanceledException("Invalid command line arguments.");
            }

            var pi = new ProcessInfo();

            var si = new StartupInfo
            {
                cb = Marshal.SizeOf(typeof(StartupInfo))
            };

            bool retval = CreateProcessWithLogonW(
                    logonCredential.Username,
                    logonCredential.Domain,
                    logonCredential.Password,
                    LogonFlags.LOGON_NETCREDENTIALS_ONLY,
                    null,
                    logonCredential.Command,
                    CreationFlags.CREATE_NEW_CONSOLE,
                    IntPtr.Zero,
                    null,
                    ref si,
                    out pi
                );

            if (!retval)
            {
                MessageBox.Show(String.Format("{0}", new Win32Exception(Marshal.GetLastWin32Error()).Message));

                return false;
            }
            else
            {
                CloseHandle(pi.hProcess);

                CloseHandle(pi.hThread);

                return true;
            }
        }
    }
}
