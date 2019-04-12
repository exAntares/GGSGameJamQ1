/*
MIT License

Copyright (c) 2017 Jeiel Aranal

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubjectNerd.PsdImporter.PsdParser;
using SubjectNerd.PsdImporter.Reconstructor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SubjectNerd.PsdImporter
{
	public class PsdImporter
	{
		private const string DOC_ROOT = "DOCUMENT_ROOT";

		private static string GetPsdFilepath(Object psdFile)
		{
			string filepath = AssetDatabase.GetAssetPath(psdFile);
			if (string.IsNullOrEmpty(filepath))
				return string.Empty;
			if (filepath.ToLower().EndsWith(".psd") == false)
				return string.Empty;
			return filepath;
		}

        private static IEnumerator ParseLayers(IPsdLayer[] layers, bool doYield, Action<PsdLayer, int[]> onLayer, Action onComplete, int[] parentIndex = null) {
            // Loop through layers in reverse so they are encountered in same order as Photoshop
            for (int i = layers.Length - 1; i >= 0; i--) {
                int[] layerIndex = parentIndex;
                if (layerIndex == null) {
                    layerIndex = new int[] { i };
                } else {
                    int lastIndex = layerIndex.Length;
                    Array.Resize(ref layerIndex, lastIndex + 1);
                    layerIndex[lastIndex] = i;
                }

                PsdLayer layer = layers[i] as PsdLayer;
                if (layer == null) {
                    continue;
                }

                onLayer?.Invoke(layer, layerIndex);
                if (doYield) {
                    yield return null;
                }

                if (layer.Childs.Length > 0) {
                    yield return EditorCoroutineRunner.StartCoroutine(ParseLayers(layer.Childs, doYield, onLayer, null, layerIndex));
                }
            }

            onComplete?.Invoke();
        }

        public static void BuildImportLayerData(Object file, ISpriteImportSettings importSettings,
                                                Action<ImportLayerData, DisplayLayerData> callback) {
            string filepath = GetPsdFilepath(file);
            if (string.IsNullOrEmpty(filepath)) {
                callback?.Invoke(null, null);
                return;
            }

            ImportLayerData docImportData = new ImportLayerData() {
                name = DOC_ROOT,
                indexId = new int[] { -1 },
                Childs = new List<ImportLayerData>()
            };

            DisplayLayerData docDisplayData = new DisplayLayerData() {
                indexId = new int[] { -1 },
                Childs = new List<DisplayLayerData>()
            };

            using (PsdDocument psd = PsdDocument.Create(filepath)) {
                EditorCoroutineRunner.StartCoroutine(
                    ParseLayers(psd.Childs, false,
                    onLayer: (layer, indexId) => {
                        // Walk down the index id to get the parent layers
                        // and build the full path
                        string fullPath = "";
                        ImportLayerData parentLayer = docImportData;
                        DisplayLayerData parentDisplay = docDisplayData;
                        if (indexId.Length > 1) {
                            for (int idIdx = 0; idIdx < indexId.Length - 1; idIdx++) {
                                int idx = indexId[idIdx];
                                parentLayer = parentLayer.Childs[idx];
                                parentDisplay = parentDisplay.Childs[idx];

                                if (string.IsNullOrEmpty(fullPath) == false)
                                    fullPath += "/";
                                fullPath += parentLayer.name;
                            }
                        }

                        if (string.IsNullOrEmpty(fullPath) == false) {
                            fullPath += "/";
                        }

                        fullPath += layer.Name;

                        ImportLayerData layerImportData = new ImportLayerData() {
                            name = layer.Name,
                            path = fullPath,
                            indexId = indexId,
                            import = layer.IsVisible,
                            useDefaults = true,
                            Alignment = importSettings.DefaultAlignment,
                            Pivot = importSettings.DefaultPivot,
                            ScaleFactor = importSettings.ScaleFactor,
                            Childs = new List<ImportLayerData>()
                        };

                        DisplayLayerData layerDisplayData = new DisplayLayerData() {
                            indexId = indexId,
                            isVisible = layer.IsVisible,
                            isGroup = layer.Childs.Length > 0,
                            isOpen = layer.IsFolderOpen
                        };

                        int layerIdx = indexId[indexId.Length - 1];

                        int maxLayers = layerIdx + 1;
                        while (parentLayer.Childs.Count < maxLayers)
                            parentLayer.Childs.Add(null);

                        parentLayer.Childs[layerIdx] = layerImportData;

                        while (parentDisplay.Childs.Count < maxLayers)
                            parentDisplay.Childs.Add(null);

                        parentDisplay.Childs[layerIdx] = layerDisplayData;
                    },
                    onComplete: () => {
                        callback?.Invoke(docImportData, docDisplayData);
                    })
                );
            }
        }

		private static PsdLayer GetPsdLayerByIndex(PsdDocument psdDoc, int[] layerIdx)
		{
			IPsdLayer target = psdDoc;
			foreach (int idx in layerIdx)
			{
				if (idx < 0 || idx >= target.Childs.Length)
					return null;
				target = target.Childs[idx];
			}

			PsdLayer layer = target as PsdLayer;
			return layer;
		}

		#region Layer Texture Generation
		public static Texture2D GetLayerTexture(Object psdFile, int[] layerIdx)
		{
			ImportLayerData setting = new ImportLayerData()
			{
				Alignment = SpriteAlignment.Center,
				Pivot = new Vector2(0.5f, 0.5f),
				ScaleFactor = ScaleFactor.Full,
                Childs = new List<ImportLayerData>(),
				import = true,
				indexId = layerIdx
			};
			return GetLayerTexture(psdFile, setting);
		}

		public static Texture2D GetLayerTexture(Object psdFile, ImportLayerData setting)
		{
			string filepath = GetPsdFilepath(psdFile);
			if (string.IsNullOrEmpty(filepath))
				return null;

			Texture2D texture = null;
			using (PsdDocument psd = PsdDocument.Create(filepath))
			{
				var layer = GetPsdLayerByIndex(psd, setting.indexId);
				texture = GetLayerTexture(psd, layer, setting.ScaleFactor);
			}
			return texture;
		}

		private static Texture2D GetLayerTexture(PsdDocument psdDoc, PsdLayer psdLayer, ScaleFactor scaleFactor)
		{
			if (psdLayer == null || psdLayer.IsGroup)
				return null;

			Texture2D layerTexture = GetTexture(psdLayer);
			if (scaleFactor != ScaleFactor.Full)
			{
				int mipMapLevel = scaleFactor == ScaleFactor.Half ? 1 : 2;
				layerTexture = ScaleTextureByMipmap(layerTexture, mipMapLevel);
			}
			return layerTexture;
		}

		private static Texture2D GetTexture(PsdLayer layer)
		{
			Texture2D texture = new Texture2D(layer.Width, layer.Height);
			Color32[] pixels = new Color32[layer.Width * layer.Height];

			Channel red = (from l in layer.Channels where l.Type == ChannelType.Red select l).First();
			Channel green = (from l in layer.Channels where l.Type == ChannelType.Green select l).First();
			Channel blue = (from l in layer.Channels where l.Type == ChannelType.Blue select l).First();
			Channel alpha = (from l in layer.Channels where l.Type == ChannelType.Alpha select l).FirstOrDefault();
			Channel mask = (from l in layer.Channels where l.Type == ChannelType.Mask select l).FirstOrDefault();

			for (int i = 0; i < pixels.Length; i++)
			{
				byte r = red.Data[i];
				byte g = green.Data[i];
				byte b = blue.Data[i];
				byte a = 255;

				if (alpha != null)
					a = alpha.Data[i];
				if (mask != null)
					a *= mask.Data[i];

				int mod = i % texture.width;
				int n = ((texture.width - mod - 1) + i) - mod;
				pixels[pixels.Length - n - 1] = new Color32(r, g, b, a);
			}

			texture.SetPixels32(pixels);
			texture.Apply();
			return texture;
		}

		private static Texture2D ScaleTextureByMipmap(Texture2D tex, int mipLevel)
		{
			if (mipLevel < 0 || mipLevel > 2)
				return null;
			int width = Mathf.RoundToInt(tex.width / (mipLevel * 2));
			int height = Mathf.RoundToInt(tex.height / (mipLevel * 2));

			// Scaling down by abusing mip maps
			Texture2D resized = new Texture2D(width, height);
			resized.SetPixels32(tex.GetPixels32(mipLevel));
			resized.Apply();
			return resized;
		}
        #endregion

        #region Layer Asset Import
        public static void ImportLayersUI(Object psdFile, ImportUserData importSettings, List<int[]> layerIndices) {
            int total = layerIndices.Count;
            Action<int, ImportLayerData> layerCallback = (current, layer) => EditorCoroutineRunner.UpdateUI($"[{current}/{total}] Layer: {layer.name}", (float)current / total);
            IEnumerator importCoroutine = ImportCoroutine(psdFile, importSettings.TargetDirectory, importSettings, layerIndices, layerCallback);
            EditorCoroutineRunner.StartCoroutineWithUI(importCoroutine, "Importing PSD Layers", true);
        }

        public static void ImportLayers(Object psdFile, ImportUserData importSettings, List<int[]> layerIndices, Action<List<Sprite>> callback = null) {
            EditorCoroutineRunner.StartCoroutine(ImportCoroutine(psdFile, importSettings.TargetDirectory, importSettings, layerIndices, completeCallback: callback));
        }

        private static IEnumerator ImportCoroutine(Object psdFile, string targetDirectory, ImportUserData importSettings,
                                                    List<int[]> layerIndices,
                                                    Action<int, ImportLayerData> layerCallback = null,
                                                    Action<List<Sprite>> completeCallback = null) {
            string filepath = GetPsdFilepath(psdFile);
            if (string.IsNullOrEmpty(filepath)) {
                completeCallback?.Invoke(null);
                yield break;
            }

            if (string.IsNullOrEmpty(targetDirectory)) {
                targetDirectory = filepath.Substring(0, filepath.LastIndexOf("/"));
            }

            // Get the texture importer for the PSD
            TextureImporter psdUnitySettings = (TextureImporter)AssetImporter.GetAtPath(filepath);
            TextureImporterSettings psdUnityImport = new TextureImporterSettings();
            psdUnitySettings.ReadTextureSettings(psdUnityImport);

            int importCurrent = 0;

            var sprites = new List<Sprite>();
            using (PsdDocument psd = PsdDocument.Create(filepath)) {
                var layersSettings = new List<ImportLayerData>();
                foreach (int[] layerIdx in layerIndices) {
                    ImportLayerData layerSettings = importSettings.GetLayerData(layerIdx);
                    if (layerSettings == null) {
                        continue;
                    }

                    layerCallback?.Invoke(importCurrent, layerSettings);
                    layersSettings.Add(layerSettings);
                }
                const float totalSteps = 4.0f;
                float currentStep = 0.0f;
                EditorCoroutineRunner.UpdateUI($"Getting psd layers", ++currentStep / totalSteps);
                yield return null;
                var psdLayers = GetPsdLayers(layersSettings, psd);
                EditorCoroutineRunner.UpdateUI($"CreateTextures2D", ++currentStep / totalSteps);
                yield return null;
                var textures = CreateTextures2D(psdLayers, psd, importSettings, psdUnityImport);
                EditorCoroutineRunner.UpdateUI($"GetTextureWithPathData", ++currentStep / totalSteps);
                yield return null;
                var texturesData = GetTextureWithPathData(textures, psdLayers.Select(x => x.psdLayer).ToArray(), importSettings.fileNaming, importSettings.groupMode, targetDirectory);
                EditorCoroutineRunner.UpdateUI($"CreateSpriteAssets", ++currentStep / totalSteps);
                yield return null;
                sprites.AddRange(CreateSpriteAssets(texturesData, layersSettings, psdUnityImport, importSettings.PackingTag));
            }
            completeCallback?.Invoke(sprites);
        }

        public struct PsdLayerWithScaleFactor {
            public PsdLayer psdLayer;
            public ScaleFactor scaleFactor;
        }

        private static IList<PsdLayerWithScaleFactor> GetPsdLayers(IList<ImportLayerData> layerSettings, PsdDocument psdDoc) {
            var result = new List<PsdLayerWithScaleFactor>();
            foreach (var layer in layerSettings) {
                if (layer == null) {
                    continue;
                }

                PsdLayer psdLayer = GetPsdLayerByIndex(psdDoc, layer.indexId);
                if (psdLayer.IsGroup) {
                    continue;
                }

                result.Add(new PsdLayerWithScaleFactor() { psdLayer = psdLayer, scaleFactor = layer.ScaleFactor });
            }
            return result;
        }

        private static IList<Texture2D> CreateTextures2D(IList<PsdLayerWithScaleFactor> psdLayer, PsdDocument psdDoc, ImportUserData importSettings, TextureImporterSettings psdUnityImport) {
            var result = new List<Texture2D>();
            foreach (var layer in psdLayer) {
                // Generate the texture
                Texture2D layerTexture = GetLayerTexture(psdDoc, layer.psdLayer, layer.scaleFactor);
                if (layerTexture == null) {
                    continue;
                }

                result.Add(layerTexture);
            }

            return result;
        }

        public struct Texture2DWithOutputPath {
            public Texture2D texture;
            public string layerPath;
            public string baseDirPath;
        }

        private static IList<Texture2DWithOutputPath> GetTextureWithPathData(IList<Texture2D> textures, IList<PsdLayer> layers, NamingConvention fileNaming, GroupMode groupMode, string baseDirPath) {
            var result = new List<Texture2DWithOutputPath>(textures.Count);
            textures = Texture2DUtils.TrimTextures(textures);
            for (int i = 0; i < textures.Count; i++) {
                var layerPath = PathUtils.GetLayerPath(layers[i], fileNaming, groupMode);
                result.Add(new Texture2DWithOutputPath() { texture = textures[i], baseDirPath = baseDirPath, layerPath = layerPath });
            }
            return result;
        }

        private static IList<Sprite> CreateSpriteAssets(IList<Texture2DWithOutputPath> spritesData, IList<ImportLayerData> layerSettings, TextureImporterSettings psdUnityImport, string packingTag) {
            var result = new List<Sprite>(spritesData.Count);
            for (int i = 0; i < spritesData.Count; i++) {
                Texture2DWithOutputPath spriteData = spritesData[i];
                var sprite = SaveAsset(spriteData.layerPath, psdUnityImport, spriteData.texture, spriteData.baseDirPath, packingTag, layerSettings[i]);
                result.Add(sprite);
            }
            return result;
        }

        private static Sprite CreateSpriteAsset(PsdDocument psdDoc, ImportUserData importSettings, ImportLayerData layerSettings, TextureImporterSettings psdUnityImport) {
            if (layerSettings == null) {
                return null;
            }

            PsdLayer psdLayer = GetPsdLayerByIndex(psdDoc, layerSettings.indexId);
            if (psdLayer.IsGroup) {
                return null;
            }

            // Generate the texture
            Texture2D layerTexture = GetLayerTexture(psdDoc, psdLayer, layerSettings.ScaleFactor);
            if (layerTexture == null) {
                return null;
            }

            var layerPath = PathUtils.GetLayerPath(psdLayer, importSettings.fileNaming, importSettings.groupMode);
            // Save the texture as an asset
            return SaveAsset(layerPath, psdUnityImport, layerTexture, importSettings.TargetDirectory, importSettings.PackingTag, layerSettings);
        }

        private static Sprite SaveAsset(string layerPath, TextureImporterSettings psdUnityImport, Texture2D texture, string baseDirPath, string packingTag, ImportLayerData layerSettings) {
            // Generate the file path for this layer
            string fileDir;
            string filepath = PathUtils.GetFilePath(layerPath, baseDirPath, out fileDir);

            // Create the folder if non existent
            if (AssetDatabase.IsValidFolder(fileDir) == false) {
                var subPaths = fileDir.Split('/');
                string parentFolder = subPaths[0];
                foreach (string folder in subPaths.Skip(1)) {
                    string targetFolder = string.Format("{0}/{1}", parentFolder, folder);
                    if (AssetDatabase.IsValidFolder(targetFolder) == false)
                        AssetDatabase.CreateFolder(parentFolder, folder);
                    parentFolder = targetFolder;
                }
            }

            // Write out the texture contents into the file
            AssetDatabase.CreateAsset(texture, filepath);
            byte[] buf = texture.EncodeToPNG();
            File.WriteAllBytes(filepath, buf);

            AssetDatabase.ImportAsset(filepath, ImportAssetOptions.ForceUpdate);
            Texture2D textureObj = AssetDatabase.LoadAssetAtPath<Texture2D>(filepath);

            // Get the texture importer for the asset
            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(filepath);
            // Read out the texture import settings so settings can be changed
            TextureImporterSettings texSetting = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(texSetting);

            float finalPPU = psdUnityImport.spritePixelsPerUnit;
            switch (layerSettings.ScaleFactor) {
                case ScaleFactor.Half:
                    finalPPU /= 2;
                    break;
                case ScaleFactor.Quarter:
                    finalPPU /= 4;
                    break;
            }

            // Change settings
            texSetting.spriteAlignment = (int)layerSettings.Alignment;
            texSetting.spritePivot = layerSettings.Pivot;
            texSetting.spritePixelsPerUnit = finalPPU;
            texSetting.filterMode = psdUnityImport.filterMode;
            texSetting.wrapMode = psdUnityImport.wrapMode;
            texSetting.textureType = TextureImporterType.Sprite;
            texSetting.spriteMode = (int)SpriteImportMode.Single;
            texSetting.mipmapEnabled = false;
            texSetting.alphaIsTransparency = true;
            texSetting.npotScale = TextureImporterNPOTScale.None;
            // Set the rest of the texture settings
            textureImporter.spritePackingTag = packingTag;
            // Write in the texture import settings
            textureImporter.SetTextureSettings(texSetting);

            EditorUtility.SetDirty(textureObj);
            AssetDatabase.WriteImportSettingsIfDirty(filepath);
            AssetDatabase.ImportAsset(filepath, ImportAssetOptions.ForceUpdate);
            return (Sprite)AssetDatabase.LoadAssetAtPath(filepath, typeof(Sprite));
        }
		#endregion

		#region Reconstruction

		private static ReconstructData GetReconstructData(PsdDocument psdDoc, string psdPath, Vector2 documentPivot,
													ImportUserData importSettings, ImportLayerData reconstructRoot)
		{
			// Get the texture import setting of the PSD
			TextureImporter psdUnitySettings = (TextureImporter)AssetImporter.GetAtPath(psdPath);
			TextureImporterSettings psdUnityImport = new TextureImporterSettings();
			psdUnitySettings.ReadTextureSettings(psdUnityImport);

			Vector2 docSize = new Vector2(psdDoc.Width, psdDoc.Height);
			ReconstructData data = new ReconstructData(docSize, documentPivot, psdUnitySettings.spritePixelsPerUnit);

			reconstructRoot.Iterate(
				layerCallback: layer =>
				{
					if (layer.import == false)
						return;

					var psdLayer = GetPsdLayerByIndex(psdDoc, layer.indexId);

					Rect layerBounds = new Rect()
					{
						xMin = psdLayer.Left,
						xMax = psdLayer.Right,
						yMin = psdDoc.Height - psdLayer.Bottom,
						yMax = psdDoc.Height - psdLayer.Top
					};
					data.layerBoundsIndex.Add(layer.indexId, layerBounds);

					string layerDir;
                    string layerPath = PathUtils.GetLayerPath(psdLayer, importSettings.fileNaming, importSettings.groupMode);
                    string spritePath = PathUtils.GetFilePath(layerPath, importSettings.TargetDirectory, out layerDir);
					Sprite layerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

					if (layerSprite == null)
						layerSprite = CreateSpriteAsset(psdDoc, importSettings, layer, psdUnityImport);

					Vector2 spriteAnchor = Vector2.zero;

					if (layerSprite != null)
					{
						TextureImporter layerImporter = (TextureImporter)AssetImporter.GetAtPath(spritePath);
						TextureImporterSettings layerSettings = new TextureImporterSettings();
						layerImporter.ReadTextureSettings(layerSettings);
						
						if (layerSettings.spriteAlignment == (int) SpriteAlignment.Custom)
							spriteAnchor = layerSettings.spritePivot;
						else
							spriteAnchor = AlignmentToPivot((SpriteAlignment) layerSettings.spriteAlignment);
					}
					data.AddSprite(layer.indexId, layerSprite, spriteAnchor);
				},
				canEnterGroup: checkGroup => checkGroup.import
			);

			return data;
		}

		public static void Reconstruct(Object psdFile, ImportUserData importSettings,
										ImportLayerData reconstructRoot, Vector2 documentPivot,
										IReconstructor reconstructor)
		{
			string psdPath = GetPsdFilepath(psdFile);
			if (string.IsNullOrEmpty(psdPath))
				return;
			
			using (var psdDoc = PsdDocument.Create(psdPath))
			{
				ReconstructData data = GetReconstructData(psdDoc, psdPath,
												documentPivot, importSettings,
												reconstructRoot);
				
				var GO = reconstructor.Reconstruct(reconstructRoot, data, Selection.activeGameObject);
				if (GO != null)
				{
					EditorGUIUtility.PingObject(GO);
					Selection.activeGameObject = GO;
				}
			}
		}

		public static Vector2 AlignmentToPivot(SpriteAlignment spriteAlignment)
		{
			Vector2 pivot = Vector2.zero;
			switch (spriteAlignment)
			{
				case SpriteAlignment.TopLeft:
				case SpriteAlignment.TopCenter:
				case SpriteAlignment.TopRight:
					pivot.y = 1f;
					break;
				case SpriteAlignment.LeftCenter:
				case SpriteAlignment.Center:
				case SpriteAlignment.RightCenter:
					pivot.y = 0.5f;
					break;
				case SpriteAlignment.BottomLeft:
				case SpriteAlignment.BottomCenter:
				case SpriteAlignment.BottomRight:
					pivot.y = 0f;
					break;
			}
			switch (spriteAlignment)
			{
				case SpriteAlignment.TopLeft:
				case SpriteAlignment.LeftCenter:
				case SpriteAlignment.BottomLeft:
					pivot.x = 0f;
					break;
				case SpriteAlignment.TopCenter:
				case SpriteAlignment.Center:
				case SpriteAlignment.BottomCenter:
					pivot.x = 0.5f;
					break;
				case SpriteAlignment.TopRight:
				case SpriteAlignment.RightCenter:
				case SpriteAlignment.BottomRight:
					pivot.x = 1f;
					break;
			}
			return pivot;
		}
		#endregion
	}
}