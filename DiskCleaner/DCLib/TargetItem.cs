namespace DiskCleaner {
    /**
     * A class that represents a target item for deletion. It should
     * be either a file or a directory
     **/
    public abstract class TargetItem {
        protected string path;

        // The C-Tor
        public TargetItem(string path) { this.path = path; }

        public string GetPath() { return path; }

        // Asses the resource size (in bytes)
        abstract public long AssessSize();

        // Delete the resource
        abstract public bool Delete();
    }
}