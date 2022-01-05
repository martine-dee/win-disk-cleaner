using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DiskCleaner {

    /**
     * Class for holding the Search deletion templates
     **/
    public class TemplateSearch : TemplateItem {
        public TemplateSearch(string line, string path, Dictionary<string, string> options)
            : base(line, path, options) { Compile(); }

        override public void Compile() {
            // Extract the regex
            if (!options.ContainsKey("regex")) {
                return;
            }
            string regexString = options["regex"];
            regexString = regexString.Substring(1, regexString.Length - 2);
            Regex regex = new Regex(regexString);

            files = new List<TargetFile>();
            directories = new List<TargetDirectory>();

            List<string> raw_file_paths = new List<string>();
            List<string> raw_dir_paths = new List<string>();
            if (path != null) {
                if (File.Exists(path)) {
                    files.Add(new TargetFile(path));
                }
                else if (Directory.Exists(path)) {
                    // Gather all files, but only in this directory!
                    string[] local_files = Directory.GetFiles(path);
                    if (local_files.Length > 0) {
                        raw_file_paths.AddRange(local_files);
                    }

                    string[] local_dirs = Directory.GetDirectories(path);
                    if (local_dirs.Length > 0) {
                        raw_dir_paths.AddRange(local_dirs);
                    }
                }
            }

            if (raw_file_paths.Count > 0 || raw_dir_paths.Count > 0) {
                foreach (string raw_file_path in raw_file_paths) {
                    if (regex.IsMatch(raw_file_path)) {
                        files.Add(new TargetFile(raw_file_path));
                    }
                }
                foreach (string raw_dir_path in raw_dir_paths) {
                    if (regex.IsMatch(raw_dir_path)) {
                        LoadAllFromADirectory(raw_dir_path, files, directories);
                    }
                }
            }
        }

        private static bool GetTemplateLocationsIsDirEligible(string basePath, string candidatePath) {
            int countBase = basePath.Split('\\').Length;
            int countCandidate = candidatePath.Split('\\').Length;
            return countCandidate == countBase || countCandidate == countBase + 1;
        }

        private static bool GetTemplateLocationsIsFileEligible(string basePath, string candidatePath) {
            int countBase = basePath.Split('\\').Length;
            int countCandidate = candidatePath.Split('\\').Length;
            return countCandidate == countBase;
        }

        public List<TemplateItem> GetTemplateLocations() {
            List<TemplateItem> list = new List<TemplateItem>();

            foreach (TargetDirectory td in directories) {
                string tdPath = td.GetPath();
                if (GetTemplateLocationsIsDirEligible(this.path, tdPath)) {
                    list.Add(new TemplateLocation(null, tdPath, TemplateLocation.location_types.DIRECTORY, options));
                }
            }

            if(list.Count == 0) {
                foreach (TargetFile tf in files) {
                    string tfPath = tf.GetPath();
                    if (GetTemplateLocationsIsFileEligible(this.path, tfPath)) {
                        list.Add(new TemplateLocation(null, tfPath, TemplateLocation.location_types.FILE, options));
                    }
                }
            }

            return list;
        }

        override public string Name() {
            StringBuilder name = new StringBuilder();
            name.AppendFormat("Search ({0}): {1}\n", files.Count, path);
            for (int i = 0; i < 5; i++) {
                if (i >= files.Count) {
                    break;
                }
                name.AppendFormat(" - ({0}) {1}\n", Debugger.FileSizeToString(files[i].AssessSize()), files[i].ToString());
            }
            if (files.Count > 5) {
                name.Append(" ...\n");
            }
            return name.ToString();
        }
    }
}