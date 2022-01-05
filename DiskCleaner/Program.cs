using System;
using System.Windows.Forms;

namespace DiskCleaner {
    static class Program {
        /// <summary>
        /// Win 10 Tools: Disk Cleaner
        /// A tool for cleaning unnecessary data on Windows 10. It works
        /// off*.txt lists of:
        /// - File/Directory locations
        /// - Search templates(written as C# code)
        /// 
        /// See an example of such a template in ./Template/
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
