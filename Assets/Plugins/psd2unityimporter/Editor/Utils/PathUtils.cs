using SubjectNerd.PsdImporter.PsdParser;
using System.IO;
using System.Linq;

namespace SubjectNerd.PsdImporter {
    public static class PathUtils {
        public static string GetFilePath(string filename, string baseDirPath, out string outDirectoryPath) {
            // Sanitize directory
            baseDirPath = SanitizeString(baseDirPath, Path.GetInvalidPathChars());

            // Sanitize filename
            filename = SanitizeString(filename, Path.GetInvalidFileNameChars());

            string filepath = string.Format("{0}/{1}", baseDirPath, filename);
            outDirectoryPath = baseDirPath;
            return filepath;
        }

        private static string SanitizeString(string text, char[] cleanChars) {
            text = string.Join("_", text.Split(cleanChars));
            text = new string(text.Select(c => {
                if (char.IsWhiteSpace(c))
                    return '_';
                return c;
            }).ToArray());
            return text;
        }

        public static string GetLayerPath(PsdLayer layer, NamingConvention fileNaming, GroupMode groupMode) {
            string filename = $"{layer.Name}.png";
            string folder = string.Empty;
            if (fileNaming != NamingConvention.LayerNameOnly) {
                bool isDir = fileNaming == NamingConvention.CreateGroupFolders;
                var docLayer = layer.Document as IPsdLayer;
                var parent = layer.Parent;
                while (parent != null && parent.Equals(docLayer) == false) {
                    if (isDir) {
                        if (string.IsNullOrEmpty(folder))
                            folder = parent.Name;
                        else
                            folder = string.Format("{0}/{1}", parent.Name, folder);
                    } else {
                        filename = string.Format("{0}_{1}", parent.Name, filename);
                    }
                    parent = parent.Parent;
                    if (groupMode == GroupMode.ParentOnly) {
                        break;
                    }
                }
            }

            return filename;
        }
    }
}