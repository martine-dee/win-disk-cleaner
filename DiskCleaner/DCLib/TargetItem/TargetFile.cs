using System.IO;

namespace DiskCleaner {

    /**
     * Class for a target file
     **/
    public class TargetFile : TargetItem {
        public TargetFile(string path) : base(path) { }

        override public long AssessSize() {
            FileInfo fi = new FileInfo(path);
            return fi.Exists ? fi.Length : 0;
        }

        override public bool Delete() {
            return TheDeleter.DeleteFile(path);
        }

        override public string ToString() { return path; }
    }
}