namespace DiskCleaner {

    /**
     * Class for a target directory
     **/
    public class TargetDirectory : TargetItem {
        public TargetDirectory(string path) : base(path) { }

        override public long AssessSize() { return 0; }

        override public bool Delete() {
            return TheDeleter.DeleteEmptyDirectory(path);
        }

        override public string ToString() { return path; }
    }
}