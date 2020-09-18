namespace PowerPlan
{
    /// <summary>
    /// Yes, this was copied from Microsoft.Win32!
    /// </summary>
    public enum RegistryValueKind
    {
        //
        // Summary:
        //     No data type.
        None = -1,
        //
        // Summary:
        //     An unsupported registry data type. For example, the Microsoft Win32 API registry
        //     data type REG_RESOURCE_LIST is unsupported. Use this value to specify that the
        //     Microsoft.Win32.RegistryKey.SetValue(System.String,System.Object) method should
        //     determine the appropriate registry data type when storing a name/value pair.
        Unknown = 0,
        //
        // Summary:
        //     A null-terminated string. This value is equivalent to the Win32 API registry
        //     data type REG_SZ.
        String = 1,
        //
        // Summary:
        //     A null-terminated string that contains unexpanded references to environment variables,
        //     such as %PATH%, that are expanded when the value is retrieved. This value is
        //     equivalent to the Win32 API registry data type REG_EXPAND_SZ.
        ExpandString = 2,
        //
        // Summary:
        //     Binary data in any form. This value is equivalent to the Win32 API registry data
        //     type REG_BINARY.
        Binary = 3,
        //
        // Summary:
        //     A 32-bit binary number. This value is equivalent to the Win32 API registry data
        //     type REG_DWORD.
        DWord = 4,
        //
        // Summary:
        //     An array of null-terminated strings, terminated by two null characters. This
        //     value is equivalent to the Win32 API registry data type REG_MULTI_SZ.
        MultiString = 7,
        //
        // Summary:
        //     A 64-bit binary number. This value is equivalent to the Win32 API registry data
        //     type REG_QWORD.
        QWord = 11
    }
}
