using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiskCleaner {
    public partial class MainForm {
        protected string Core_ObtainTemplateText(string path) {
            string text = File.ReadAllText(path);
            return text;
        }

        private Dictionary<string, string> templateVars = new Dictionary<string, string>();

        protected void Core_ParseTheTemplate() {
            if (templateItems != null || templateText == null) { return; }

            List<string> lines = new List<string>();
            {
                string template = templateText;
                using (StringReader reader = new StringReader(template)) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        lines.Add(line);
                    }
                }
            }

            templateVars.Clear();
            templateItems = TemplateItem.processLines(lines, templateVars);

            if (Debugger.debugLevelThreshold > 1) {
                StringBuilder report = new StringBuilder();
                foreach (TemplateItem item in templateItems) {
                    report.AppendFormat("{0}{1}", item.ToString(), System.Environment.NewLine);
                }
                Debugger.PrintWithLevel(1, report.ToString());
            }
        }

        protected Dictionary<string, long> Core_LookUpLargeDirs() {
            // Build the dir list
            HashSet<string> dirSet = new HashSet<string>();
            Stack<string> dirs = new Stack<string>();
            dirs.Push("C:\\");

            while(dirs.Count > 0) {
                string dir = dirs.Pop();
                dirSet.Add(dir);
                try {
                    DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                    DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                    foreach (DirectoryInfo fi in directoryInfos) {
                        dirs.Push(fi.FullName);
                    }
                }
                catch (System.UnauthorizedAccessException e) {
                    // Debugger.Print("Skipped {0}: {1}", dir, e.ToString());
                }
            }

            List<string> dirList = new List<string>(dirSet);
            dirList.Sort(delegate (string s1, string s2) {
                if(s1.StartsWith(s2)) { return -1; }
                if(s2.StartsWith(s1)) { return 1; }
                return s1.CompareTo(s2);
            });

            // Collect directory sizes
            Dictionary<string, long> dirToSize = new Dictionary<string, long>();
            foreach (string dir in dirList) {
                try {
                    DirectoryInfo directoryInfo = new DirectoryInfo(dir);

                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    long size = 0;
                    foreach (FileInfo fileInfo in fileInfos) {
                        size += fileInfo.Length;
                    }
                    DirectoryInfo[] dirInfos = directoryInfo.GetDirectories();
                    foreach (DirectoryInfo dirInfo in dirInfos) {
                        string dirInfoFullName = dirInfo.FullName;
                        size += dirToSize.ContainsKey(dirInfoFullName) ? dirToSize[dirInfoFullName] : 0;
                    }
                    dirToSize[dir] = size;
                }
                catch(System.UnauthorizedAccessException) {
                }
            }

            string lastPickedDir = null;
            foreach (string dir in dirList) {
                if(dirToSize.ContainsKey(dir) && dirToSize[dir] <= 100*1024*1024) { // Trim all smaller than 300 MB
                    dirToSize.Remove(dir);
                }
                if(lastPickedDir != null && lastPickedDir.StartsWith(dir)) {
                    dirToSize.Remove(dir);
                }
                lastPickedDir = dir;
            }

            return dirToSize;
        }
    }
}