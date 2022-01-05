using System.IO;

namespace DiskCleaner {
    /**
     * The minimal Utility for deleting single files and empty
     * directories
     **/
    public static class TheDeleter {
        private static int unauthorizedAccessExceptionCount = 0;
        private static int ioExceptionCount = 0;

        public static void ClearTheStats() {
            unauthorizedAccessExceptionCount = 0;
            ioExceptionCount = 0;
        }

        public static int StatsUnauthorizedAccessExceptionCount() {
            return unauthorizedAccessExceptionCount;
        }

        public static int StatsIoExceptionsCount() {
            return ioExceptionCount;
        }

        public static bool DeleteFile(string path) {
            if (File.Exists(path)) {
                try { File.Delete(path); }
                catch (IOException ioException) {
                    Debugger.PrintWithLevel(5, ioException.ToString());
                    ioExceptionCount++;
                    return false;
                }
                catch (System.UnauthorizedAccessException unauthorizedAccessException) {
                    Debugger.PrintWithLevel(5, unauthorizedAccessException.ToString());
                    unauthorizedAccessExceptionCount++;
                    return false;
                }
                return !File.Exists(path);
            }
            return false;
        }

        public static bool DeleteEmptyDirectory(string path) {
            if (Directory.Exists(path)) {
                try { Directory.Delete(path); }
                catch (IOException ioException) {
                    Debugger.PrintWithLevel(5, ioException.ToString());
                    ioExceptionCount++;
                    return false;
                }
                catch (System.UnauthorizedAccessException unauthorizedAccessException) {
                    Debugger.PrintWithLevel(5, unauthorizedAccessException.ToString());
                    unauthorizedAccessExceptionCount++;
                    return false;
                }
                return !Directory.Exists(path);
            }
            return false;
        }
    }
}