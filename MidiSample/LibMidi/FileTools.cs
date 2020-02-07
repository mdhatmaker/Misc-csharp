using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace LibMidi
{
    public class FileTools
    {
        public static string GetParent(string path, uint levels = 1)
        {
            DirectoryInfo dinfo = Directory.GetParent(path);
            for (int i = 1; i < levels; ++i)
                dinfo = Directory.GetParent(dinfo.FullName);
            return dinfo.FullName;
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    } // end of class FileTools

} // end of namespace LibMidi
