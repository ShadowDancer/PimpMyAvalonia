using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PimpMyAvalonia.LanguageServer.Tests
{
    static class TestUtils
    {
        public static string GetApplicationRoot()
        {
            var wd = AppDomain.CurrentDomain.BaseDirectory;
            while (Path.GetFileName(wd) != "bin")
            {
                wd = Directory.GetParent(wd).FullName;
            }
            return Directory.GetParent(wd).FullName;
        }

    }
}
