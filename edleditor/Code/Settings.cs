using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace edleditor
{
    class Settings
    {
        public static string TVRecordsPath
        {
            get
            {
                try
                {
                    //get records folder
                    RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Media Center\Service\Recording");
                    return (string)key.GetValue("RecordPath");
                }
                catch
                {
                    return "";
                }
            }
        }
    }
}
