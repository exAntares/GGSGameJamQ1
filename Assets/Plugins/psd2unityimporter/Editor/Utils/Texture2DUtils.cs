using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubjectNerd.PsdImporter {
    public static class Texture2DUtils {
        public struct TrimmedColorData {
            public Color[] pixels;
            public int width;
            public int height;
        }

        public static IList<Texture2D> TrimTextures(IList<Texture2D> textures) {
            return GetTrimmedTexture2D(GetTrimmedColorData(textures));
        }

        public static IList<Texture2D> GetTrimmedTexture2D(TrimmedColorData[] trimData) {
            var result = new Texture2D[trimData.Length];
            for (int i = 0; i < trimData.Length; i++) {
                result[i] = new Texture2D(trimData[i].width, trimData[i].height);
                result[i].SetPixels(trimData[i].pixels);
                result[i].Apply();
            }
            return result;
        }

        public static TrimmedColorData[] GetTrimmedColorData(IList<Texture2D> textures) {
            var result = new TrimmedColorData[textures.Count];
            var allTasks = new Rect[textures.Count];
            for (int i = 0; i < textures.Count; i++) {
                result[i].height = textures[i].height;
                result[i].width = textures[i].width;
                result[i].pixels = textures[i].GetPixels();
            }

            ParallelQuery<Rect> bounds = result
                .AsParallel()
                .AsOrdered()
                .Select(trimData => {
                    int width = trimData.width;
                    int height = trimData.height;
                    return GetTrimBounds(trimData.pixels, width, height);
                });

            var j = 0;
            foreach (Rect rect in bounds) {
                Texture2D currentTexture = textures[j];
                result[j].width = (int)rect.width;
                result[j].height = (int)rect.height;
                result[j].pixels = currentTexture.GetPixels((int)rect.x, (int)rect.y, result[j].width, result[j].height);
                j++;
            }

            return result;
        }

        public static Rect GetTrimBounds(Color[] pixels, int width, int height) {
            int leftBound = GetLeftBound(pixels, width, height);
            int rightBound = GetRightBound(pixels, width, height);
            int topBound = GetTopBound(pixels, width, height);
            int bottomBound = GetBottomBound(pixels, width, height);
            var result = new Rect(leftBound, bottomBound, (rightBound + 1) - leftBound, (topBound + 1) - bottomBound);
            return result;
        }

        public static int GetLeftBound(Color[] pixels, int width, int height) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    var pixel = pixels[y * width + x];
                    if (pixel.a > 0) {
                        return x;
                    }
                }
            }
            return 0;
        }

        public static int GetRightBound(Color[] pixels, int width, int height) {
            for (int x = width - 1; x >= 0; --x) {
                for (int y = 0; y < height; y++) {
                    var pixel = pixels[y * width + x];
                    if (pixel.a > 0) {
                        return x;
                    }
                }
            }
            return width;
        }

        public static int GetBottomBound(Color[] pixels, int width, int height) {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    var pixel = pixels[y * width + x];
                    if (pixel.a > 0) {
                        return y;
                    }
                }
            }
            return 0;
        }

        public static int GetTopBound(Color[] pixels, int width, int height) {
            for (int y = height - 1; y >= 0; --y) {
                for (int x = 0; x < width; x++) {
                    var pixel = pixels[y * width + x];
                    if (pixel.a > 0) {
                        return y;
                    }
                }
            }
            return 0;
        }
    }
}
