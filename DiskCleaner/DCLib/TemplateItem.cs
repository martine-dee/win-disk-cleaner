using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace DiskCleaner {

    /**
     * A template item. Each object of this type represents one entry
     * from the deletion template. See example of a deletion template
     * in this projects' ./Template/*
     * 
     * Currently, this class has two child classes:
     *  - TemplateLocation, for a single file or directory
     *  - TemplateSearch, for a search over a directory
     **/
    public abstract class TemplateItem {
        // Given
        protected string sourceTemplateLine;
        protected string path;
        private long assessedSizeCached = 0;
        protected Dictionary<string, string> options;

        // Inferred data (through the template compilation via .Compile()
        protected List<TargetFile> files;
        protected List<TargetDirectory> directories;

        // The C-Tor
        public TemplateItem(string sourceTemplateLine, string path, Dictionary<string, string> options) {
            this.sourceTemplateLine = sourceTemplateLine;
            this.path = path;
            this.options = options;
        }

        // Determine which files and directories are covered by this template
        abstract public void Compile();

        // Tells if there are any files covered by this template
        public bool hasFiles() { return files.Count > 0; }

        // Assess the size of the covered data
        public long AssessSize() {
            long size = 0;
            foreach (TargetFile file in files) {
                size += file.AssessSize();
            }
            assessedSizeCached = size;
            return size;
        }

        public bool hasSize() {
            if(assessedSizeCached > 0) {
                return true;
            }
            if(files.Count > 0) {
                foreach(TargetFile tf in files) {
                    if(tf.AssessSize() > 0) {
                        return true;
                    }
                }
            }
            return false;
        }

        public long AssessedSizeCached() {
            return assessedSizeCached;
        }

        public int AssessedFileCount() {
            return files.Count;
        }

        public int AssessedDirectoryCount() {
            return directories.Count;
        }

        /**
         * This method tells if the item should be checked by default in
         * the UI. The default is yes (true), and otherwise can be speci-
         * fied in the deletion template as
         *
         *   checked: no
         **/
        public bool IsChecked() {
            if (options != null && options.ContainsKey("checked")) {
                string val = options["checked"];
                if (!String.IsNullOrEmpty(val) && val.Equals("no")) {
                    return false;
                }
            }
            return true;
        }

        private bool? isCulled = null;
        public bool IsCulled() {
            if (isCulled == null) {
                if (options != null && options.ContainsKey("culled")) {
                    string val = options["culled"];
                    if (!String.IsNullOrEmpty(val) && val.Equals("yes")) {
                        isCulled = true;
                        return true;
                    }
                }
                isCulled = false;
                return false;
            }
            return (bool) isCulled;
        }

        private bool? isDisabled = null;
        public bool IsDisabled() {
            if (isDisabled == null) {
                if (options != null && options.ContainsKey("disabled")) {
                    string val = options["disabled"];
                    if (!String.IsNullOrEmpty(val) && val.Equals("yes")) {
                        isDisabled = true;
                        return true;
                    }
                }
                isDisabled = false;
                return false;
            }
            return (bool) isDisabled;
        }

        // Generate a template identifier
        abstract public string Name();

        // Delete all files and directories covered by this template
        public void DeleteAll() {
            foreach (TargetFile file in files) {
                if (!file.Delete()) { Debugger.PrintWithLevel(1, "Could not delete file: {0}", file.ToString()); }
            }
            foreach (TargetDirectory dir in directories) {
                if (!dir.Delete()) { Debugger.PrintWithLevel(1, "Could not delete dir: {0}", dir.ToString()); }
            }
        }

        /**
		 * The template parser section (static code)
		 **/
        // For parsing the "Location: path" lines
        private static readonly Regex parseLocation = new Regex(@"[Ll]ocation:\s*(.+)\s*");
        // For parsing the "Search: path" lines
        private static readonly Regex parseSearch = new Regex(@"[Ss]earch:\s*(.+)\s*");
        // Starts with a whitespace?
        private static readonly Regex startsWithWS = new Regex(@"^\s");
        // For case-insensitive search
        private static readonly CultureInfo ci = new CultureInfo("en-US");

        // Processes lines from a deletion template
        public static List<TemplateItem> processLines(List<string> lines, Dictionary<string, string> vars) {
            List<TemplateItem> templateItems = new List<TemplateItem>();

            int pos = 0;
            while (pos < lines.Count) {
                string line = lines[pos];
                List<string> optionLines = null;
                Dictionary<string, string> options = null;

                // Process the line with its optionLines
                if (line.StartsWith("#")) // A comment
                {
                    pos++;
                    continue;
                }

                if(line.StartsWith("$")) // A var
                {
                    int equalsPos = line.IndexOf("=");
                    if(equalsPos != -1)
                    {
                        string varname = line.Substring(1, equalsPos - 1).Trim();
                        if(vars.ContainsKey(varname))
                        {
                            Debugger.Print("The varname '{0}' is already defined. Skipping. Line {1}: '{2}'", varname, pos, line);
                        }
                        else
                        {
                            string value = line.Substring(equalsPos + 1).Trim();
                            vars.Add(varname, value);
                        }
                    }
                    else
                    {
                        Debugger.Print("Found 'debug_threshold:' without the option 'level'. Please add the 'level: value' as an option in the next line.");
                        continue;
                    }
                    pos++;
                    continue;
                }

                // Load additional lines, if present. Any starting with
                // spaces will be joined with this ones.
                if (pos + 1 < lines.Count && startsWithWS.IsMatch(lines[pos + 1])) {
                    pos++;
                    optionLines = new List<string>();
                    while (pos < lines.Count && startsWithWS.IsMatch(lines[pos])) {
                        optionLines.Add(lines[pos]);
                        pos++;
                    }

                    // Rewind one position back, unless the reading is at the last
                    // line, which belonged to the optionLines. In that case, everything
                    // was just read.
                    if (pos != lines.Count || !startsWithWS.IsMatch(lines[pos - 1])) {
                        pos--;
                    }
                }
                if (optionLines != null) {
                    options = TemplateItem.processOptionLines(optionLines);
                }

                if (line.StartsWith("location:", true, ci)) {
                    TemplateItem newItem = processALocation(line, options);
                    if (newItem != null && newItem.hasSize()) {
                        templateItems.Add(newItem);
                    }
                }
                else if (line.StartsWith("search:", true, ci)) {
                    if (!options.ContainsKey("regex")) {
                        Debugger.Print("The option 'regex' is mandatory for a 'search:' item. At: '{0}'", line);
                        continue;
                    }
                    string regex = options["regex"];
                    if (!regex.StartsWith("\"") || !regex.EndsWith("\"")) {
                        Debugger.Print("The 'regex' value must start and end with a double quote (\"). At: '{0}'", line);
                        continue;
                    }

                    List<TemplateItem> items = TemplateItem.processASearch(line, options);
                    if (items != null &&  items.Count > 0) {
                        foreach(TemplateItem item in items) {
                            if (item.hasSize()) {
                                templateItems.Add(item);
                            }
                        }
                    }
                }
                else if (line.StartsWith("debug_threshold", true, ci)) {
                    if (!options.ContainsKey("level")) {
                        Debugger.Print("Found 'debug_threshold:' without the option 'level'. Please add the 'level: value' as an option in the next line.");
                        continue;
                    }

                    try {
                        int newLevel = int.Parse(options["level"]);
                        Debugger.debugLevelThreshold = newLevel;
                    }
                    catch(Exception e) {
                        Debugger.Print(
                            "Coudln't set new debug threshold: {0}",
                            e == null
                                ? "exception=null"
                                : e.ToString()
                        );
                    }
                }
                pos++;
            }

            return templateItems;
        }

        private static char[] unwantedSpaces = { ' ', '\t' };

        private static Dictionary<string, string> processOptionLines(List<string> optionLines) {
            Dictionary<string, string> options = new Dictionary<string, string>();
            foreach (string optionLine in optionLines) {
                if (String.IsNullOrEmpty(optionLine)) {
                    continue;
                }
                int separatorPos = optionLine.IndexOf(":");
                if (separatorPos != -1) {
                    string optionName = optionLine.Substring(0, separatorPos);
                    optionName = optionName.Trim(unwantedSpaces);
                    string optionContent = optionLine.Substring(separatorPos + 1);
                    optionContent = optionContent.Trim(unwantedSpaces);
                    options.Add(optionName, optionContent);
                }
                else {
                    Debugger.Print("Invalid option line: {0}", optionLine);
                }
            }
            return options;
        }

        // Extracts path from the given template line. Beware, it is important
        // to obtain paths through this method, as it is supposed to handle
        // all parameters that could be present in the template.
        public static string ExtractLocation(String line, Regex regex) {
            Match matches = regex.Match(line);
            if (matches.Groups.Count == 2) {
                string location = matches.Groups[1].Value;
                if (location != null) {
                    location = Environment.ExpandEnvironmentVariables(location);
                }
                return location;
            }
            return null;
        }

        // Recursively deep-searches the given dirPath, as to collect all
        // present files and directories
        public static void LoadAllFromADirectory(string dirPath, List<TargetFile> fileTargets, List<TargetDirectory> dirTargets) {
            Stack<string> dirs = new Stack<string>();
            dirs.Push(dirPath);

            while (dirs.Count > 0) {
                string dir = dirs.Pop();
                if(!Directory.Exists(dir)) { continue; }

                dirTargets.Add(new TargetDirectory(dir));

                try {
                    string[] foundDirs = Directory.GetDirectories(dir);
                    if (foundDirs.Length > 0) {
                        foreach (string foundDir in foundDirs) {
                            dirs.Push(foundDir);
                        }
                    }

                    string[] foundFiles = Directory.GetFiles(dir);
                    foreach (string foundFile in foundFiles) {
                        fileTargets.Add(new TargetFile(foundFile));
                    }
                }
                catch(UnauthorizedAccessException e) {
                    Debugger.Print("Skipped {0}: {1}", dir, e.ToString());
                }
            }
        }

        // Proceses a Search template
        private static List<TemplateItem> processASearch(string line, Dictionary<string, string> options) {
            string searchLocation = ExtractLocation(line, parseSearch);
            TemplateSearch ts = new TemplateSearch(line, searchLocation, options);
            List<TemplateItem> list = ts.GetTemplateLocations();
            return list;
        }

        // Processes a Location template
        private static TemplateItem processALocation(string line, Dictionary<string, string> options) {
            TemplateItem templateItem = null;
            string location = ExtractLocation(line, parseLocation);
            if (location != null) {
                if (File.Exists(location)) {
                    templateItem = new TemplateLocation(line, location, TemplateLocation.location_types.FILE, options);
                }
                else if (Directory.Exists(location)) {
                    templateItem = new TemplateLocation(line, location, TemplateLocation.location_types.DIRECTORY, options);
                }
                // else Doesn't exist
            }
            return templateItem;
        }
    }
}