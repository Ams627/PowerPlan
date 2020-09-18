using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace PowerSetter
{
    internal class Logger
    {
        private string _dir;
        private readonly string _filename;

        public Logger(string dir)
        {
            _dir = dir;
            Directory.CreateDirectory(dir);
            var prog = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
            var pid = Process.GetCurrentProcess().Id;

            for (int i = 0; i < int.MaxValue; i++)
            {
                _filename = Path.Combine(dir, $"{prog}-{pid}-{i:D2}.log");
                if (!File.Exists(_filename))
                {
                    break;
                }
            }
            Log($"Program started (pid: {pid})");
            var elevate = IsAdministrator ? "This process is elevated." : "THIS PROCESS IS NOT ELEVATED";
            Log(elevate);
        }

        internal void Log(string s)
        {
            var now = DateTime.Now;
            File.AppendAllText(_filename, $"{now:yyyy-MM-dd HH:mm:ss.ff}|{s}\r\n");
        }

        private static bool IsAdministrator => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }
}