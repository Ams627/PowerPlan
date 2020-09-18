using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;


namespace PowerPlan
{
    public static class PlanHelper
    {
        [DllImport("PowrProf.dll")]
        private static extern UInt32 PowerEnumerate(IntPtr RootPowerKey, IntPtr SchemeGuid, IntPtr SubGroupOfPowerSettingGuid, UInt32 AcessFlags, UInt32 Index, ref Guid Buffer, ref UInt32 BufferSize);

        [DllImport("PowrProf.dll")]
        private static extern UInt32 PowerEnumerate(IntPtr RootPowerKey, ref Guid SchemeGuid, IntPtr SubGroupOfPowerSettingGuid, UInt32 AcessFlags, UInt32 Index, ref Guid Buffer, ref UInt32 BufferSize);

        [DllImport("PowrProf.dll")]
        private static extern UInt32 PowerEnumerate(IntPtr RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingGuid, UInt32 AcessFlags, UInt32 Index, ref Guid Buffer, ref UInt32 BufferSize);

        [DllImport("PowrProf.dll")]
        private static extern UInt32 PowerReadFriendlyName(IntPtr RootPowerKey, ref Guid SchemeGuid, IntPtr SubGroupOfPowerSettingGuid, IntPtr PowerSettingGuid, IntPtr Buffer, ref UInt32 BufferSize);

        [DllImport("PowrProf.dll")]
        private static extern UInt32 PowerReadFriendlyName(IntPtr RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingGuid, IntPtr PowerSettingGuid, IntPtr Buffer, ref UInt32 BufferSize);

        [DllImport("PowrProf.dll")]
        private static extern UInt32 PowerReadFriendlyName(IntPtr RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingGuid, ref Guid PowerSettingGuid, IntPtr Buffer, ref UInt32 BufferSize);

        [DllImport("PowrProf.dll")]
        private static extern UInt32 PowerSetActiveScheme(IntPtr UserRootPowerKey,  ref Guid SchemeGuid);

        [DllImport("PowrProf.dll")]
        private static extern UInt32 PowerGetActiveScheme(IntPtr UserRootPowerKey, ref IntPtr SchemeGuid);


        [DllImport("powrprof.dll")]
        private static extern uint PowerReadACValue(IntPtr RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingGuid, ref Guid PowerSettingGuid, ref int Type, IntPtr Buffer, ref uint BufferSize);

        [DllImport("powrprof.dll")]
        private static extern UInt32 PowerReadSettingAttributes(ref Guid subGroupGuid, ref Guid PowerSettingGuid);


        public enum AccessFlags : uint
        {
            ACCESS_SCHEME = 16,
            ACCESS_SUBGROUP = 17,
            ACCESS_INDIVIDUAL_SETTING = 18
        }

        public static IEnumerable<Guid> GetPlans()
        {
            var schemeGuid = Guid.Empty;

            uint sizeSchemeGuid = (uint)Marshal.SizeOf(typeof(Guid));
            uint schemeIndex = 0;

            while (PowerEnumerate(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, (uint)AccessFlags.ACCESS_SCHEME, schemeIndex, ref schemeGuid, ref sizeSchemeGuid) == 0)
            {
                yield return schemeGuid;
                schemeIndex++;
            }
        }

        public static IEnumerable<Guid> GetSettingsGroups(Guid plan)
        {
            var schemeGuid = Guid.Empty;

            uint sizeSchemeGuid = (uint)Marshal.SizeOf(typeof(Guid));
            uint schemeIndex = 0;

            while (PowerEnumerate(IntPtr.Zero, ref plan, IntPtr.Zero, (uint)AccessFlags.ACCESS_SUBGROUP, schemeIndex, ref schemeGuid, ref sizeSchemeGuid) == 0)
            {
                yield return schemeGuid;
                schemeIndex++;
            }
        }

        public static IEnumerable<Guid> GetSettings(Guid plan, Guid group)
        {
            var schemeGuid = Guid.Empty;

            uint sizeSchemeGuid = (uint)Marshal.SizeOf(typeof(Guid));
            uint schemeIndex = 0;

            while (PowerEnumerate(IntPtr.Zero, ref plan, ref group, (uint)AccessFlags.ACCESS_INDIVIDUAL_SETTING, schemeIndex, ref schemeGuid, ref sizeSchemeGuid) == 0)
            {
                yield return schemeGuid;
                schemeIndex++;
            }
        }


        public static void SetPlan(Guid guid)
        {
            PowerSetActiveScheme(IntPtr.Zero, ref guid);
        }

        public static Guid GetCurrentPlan()
        {
            Guid guid = Guid.Empty;
            IntPtr pGuid = IntPtr.Zero;
            PowerGetActiveScheme(IntPtr.Zero, ref pGuid);

            guid = (Guid)Marshal.PtrToStructure(pGuid, typeof(Guid));

            return guid;
        }


        public static (string name, UInt32 error) ReadFriendlyName(Guid schemeGuid, Guid? subGroup = null, Guid? setting = null)
        {
            UInt32 result;
            uint sizeName = 1024 * 10;
            IntPtr pSizeName = Marshal.AllocHGlobal((int)sizeName);

            try
            {
                if (!subGroup.HasValue && !setting.HasValue)
                {
                    result = PowerReadFriendlyName(IntPtr.Zero, ref schemeGuid, IntPtr.Zero, IntPtr.Zero, pSizeName, ref sizeName);
                    if (result != 0)
                    {
                        return ("", result);
                    }
                }
                else if (subGroup.HasValue && !setting.HasValue)
                {
                    var vSubGroup = subGroup.Value;
                    result = PowerReadFriendlyName(IntPtr.Zero, ref schemeGuid, ref vSubGroup, IntPtr.Zero, pSizeName, ref sizeName);
                    if (result != 0)
                    {
                        return ("", result);
                    }
                }
                else if (subGroup.HasValue && setting.HasValue)
                {
                    var vSubGroup = subGroup.Value;
                    var vSetting = setting.Value;
                    result = PowerReadFriendlyName(IntPtr.Zero, ref schemeGuid, ref vSubGroup, ref vSetting, pSizeName, ref sizeName);
                    if (result != 0)
                    {
                        return ("", result);
                    }
                }

                string name = Marshal.PtrToStringUni(pSizeName);
                return (name, 0);
            }
            finally
            {
                Marshal.FreeHGlobal(pSizeName);
            }
        }

        public static object GetAcValue(Guid plan, Guid subgroup, Guid value)
        {
            uint sizeName = 1024;
            IntPtr pSizeName = Marshal.AllocHGlobal((int)sizeName);

            try
            {
                var str = value.ToString();
                int type = (int)RegistryValueKind.DWord;

                UInt32 result = PowerReadACValue(IntPtr.Zero, ref plan, ref subgroup, ref value, ref type, pSizeName, ref sizeName);
                var rtype = (RegistryValueKind)type;

                switch (rtype)
                {
                    case RegistryValueKind.String:
                        return (string)Marshal.PtrToStringUni(pSizeName);
                    case RegistryValueKind.ExpandString:
                        return null;
                    case RegistryValueKind.Binary:
                        var arr = new Byte[sizeName];
                        Marshal.Copy(pSizeName, arr, 0, (int)sizeName);
                        return (byte[])arr;
                    case RegistryValueKind.DWord:
                        return (UInt32)Marshal.ReadInt32(pSizeName);
                    case RegistryValueKind.MultiString:
                        return null;
                    case RegistryValueKind.QWord:
                        return (UInt64)Marshal.ReadInt64(pSizeName);
                    default:
                        return null;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pSizeName);
            }
        }

        public static bool IsHidden(Guid subgroup, Guid value)
        {
            return (PowerReadSettingAttributes(ref subgroup, ref value) & 1) != 0;
        }
    }
}