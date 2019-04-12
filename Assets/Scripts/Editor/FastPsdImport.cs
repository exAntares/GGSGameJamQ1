using SubjectNerd.PsdImporter;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HalfBlind {
    public static class FastPsdImport {
        private const string MENU_ASSET_IMPORT = "Assets/Import PSD Layers (FAST)";

        [MenuItem(MENU_ASSET_IMPORT)]
        private static void ImportPsdAsset() {
            Object[] selectionArray = Selection.objects;
            if (selectionArray.Length < 1) {
                return;
            }

            for (int i = 0; i < selectionArray.Length; i++) {
                var file = selectionArray[i];
                var path = AssetDatabase.GetAssetPath(file);
                if (path.ToLower().EndsWith(".psd")) {
                    ImportPsd(file);
                    return;
                }
            }
        }

        private static void ImportPsd(Object importFile) {
            var importSettings = new ImportUserData {
                fileNaming = NamingConvention.CreateGroupFolders,
                groupMode = GroupMode.FullPath,
                TargetDirectory = "Assets/DropboxOutbox"
            };

            PsdImporter.BuildImportLayerData(importFile, importSettings, (importedLayerData, displayData) => {
                importSettings.DocRoot = importedLayerData;
                List<int[]> importLayersList = importSettings.DocRoot.Childs
                .Select(x => x.indexId)
                .ToList();

                PsdImporter.ImportLayersUI(importFile, importSettings, importLayersList);
            });
        }
    }
}
