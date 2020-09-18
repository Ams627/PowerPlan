using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace PowerSetter
{
    class Program
    {
        [DllImport("PowrProf.dll")]
        private static extern UInt32 PowerSetActiveScheme(IntPtr UserRootPowerKey, ref Guid SchemeGuid);
        static void Main(string[] args)
        {
            var log = new Logger(@"c:\temp\PowerSetter\logs");
            
            if (!args.Any())
            {
                log.Log("You must specify a GUID on the command line - this is the power plan to continually set.");
                log.Log($"Exiting.");
                Environment.Exit(0);
            }

            if (!Guid.TryParse(args[0], out var guid))
            {
                log.Log("Invalid GUID specified on the command line.");
                log.Log($"Exiting.");
                Environment.Exit(0);
            }

            while (true)
            {
                var result = PowerSetActiveScheme(IntPtr.Zero, ref guid);
                if (result == 0)
                {
                    log.Log($"Power scheme set to {guid}");
                }
                else
                {
                    log.Log($"Failed to set power scheme set to {guid} - Win32 error code was {result}");
                }
                System.Threading.Thread.Sleep(3000);
            }
        }
    }
}
