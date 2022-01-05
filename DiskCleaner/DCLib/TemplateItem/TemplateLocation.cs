using System.Collections.Generic;
using System.Text;

namespace DiskCleaner {
    /**
     * Class that reprents a Location deletion template. This should
     * be either a file or a directory.
     **/
    public class TemplateLocation : TemplateItem {
        public enum location_types {
            DIRECTORY,
            FILE
        }

        private location_types type;

        public TemplateLocation(string line, string path, location_types type, Dictionary<string, string> options)
           : base(line, path, options) {
            this.path = path;
            this.type = type;
            this.Compile();
        }

        override public void Compile() {
            files = new List<TargetFile>();
            directories = new List<TargetDirectory>();

            if (this.type == location_types.FILE) {
                files.Add(new TargetFile(this.path));
            }
            else if (this.type == location_types.DIRECTORY) {
                LoadAllFromADirectory(this.path, files, directories);
            }
        }

        override public string Name() {
            StringBuilder name = this.NameSB();
            return name.ToString();
        }

        // Returns a .Name() StringBuilder, which should save some
        // time when building string further on top of the template's
        // .Name()
        private StringBuilder NameSB() {
            StringBuilder name = new StringBuilder();
            name.AppendFormat("Location({0}): {1}", type == location_types.DIRECTORY ? "DIR" : "FILE", path);
            return name;
        }

        override public string ToString() {
            StringBuilder report = this.NameSB();
            foreach (TargetFile file in this.files) {
                report.AppendFormat(" {0}\n", file.ToString());
            }
            foreach (TargetDirectory dir in directories) {
                report.AppendFormat(" {0}\n", dir.ToString());
            }
            return report.ToString();
        }
    }
}