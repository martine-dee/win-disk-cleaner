using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DiskCleaner {

    /**
     * A naively written debug helper
     **/
    public static class Debugger {
        // The UI element to write the log output to
        public static RichTextBox rtDebug = null;
        // The log file to write the log output to in absence of rtDebug
        public static string logPath = "defaultlog.log";
        // The StreamWriter for the file above
        private static StreamWriter sw = null;


        /**
         * The debug level at which debug output should be written.
         * How does it work? The .Print(text, level) will result in
         * printed text if level is <= debugLevelThreshold
         **/
        public static int debugLevelThreshold = 0;

        // Write debug output at the given debug level
        public static void PrintWithLevel(int level, string line, params object[] paramList) {
            if (level > debugLevelThreshold) { return; }

            string lineFormatted = paramList.Length > 0 ? String.Format(line, paramList) : line;

            if (rtDebug == null) {
                if (sw != null || !String.IsNullOrWhiteSpace(logPath)) {
                    if (sw == null) {
                        sw = File.AppendText(logPath);
                        sw.WriteLine("\n\n++++++++++++++++++++++++++++++++++++++\nRun on {0}\n++++++++++++++++++++++++++++++++++++++", (new DateTime()).ToString());
                    }
                    sw.WriteLine(lineFormatted);
                    sw.Flush();
                }
                return;
            }
            rtDebug.Text += lineFormatted + "\n";
            rtDebug.SelectionStart = rtDebug.Text.Length;
            rtDebug.ScrollToCaret();
        }

        // Write debug output at the debug level = 0
        public static void Print(string line, params object[] paramList) {
            Debugger.PrintWithLevel(0, line, paramList);
        }

        private static HashSet<string> seenLines = new HashSet<string>();

        public static void PrintUnique(string line, params object[] paramList) {
            if(!seenLines.Contains(line)) {
                seenLines.Add(line);
                Print(line, paramList);
            }
        }

        public static void PrintUniqueWithKey(string line, string key, params object[] paramList) {
            if (!seenLines.Contains(key)) {
                seenLines.Add(key);
                Print(line, paramList);
            }
        }

        // Clear the UI component for debug output
        public static void Clear() {
            if (rtDebug != null) { rtDebug.Text = ""; }
        }

        /**
         * This section is for printing file size
         **/

        public static readonly long KB_SIZE = 1024;
        public static readonly long MB_SIZE = KB_SIZE * 1024;
        public static readonly long GB_SIZE = MB_SIZE * 1024;

        public static string FileSizeToString(long size) {
            if (size > GB_SIZE) {
                return String.Format("{0:0.##} GB", size * 1.0 / GB_SIZE);
            }
            if (size > MB_SIZE) {
                return String.Format("{0:0.##} MB", size * 1.0 / MB_SIZE);
            }
            if (size > KB_SIZE) {
                return String.Format("{0:0.##} kB", size * 1.0 / KB_SIZE);
            }

            return String.Format("{0:0.##} bytes", size);
        }
    }
}
