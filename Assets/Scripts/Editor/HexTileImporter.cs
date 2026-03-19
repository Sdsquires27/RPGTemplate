// Assets/Editor/HexTileImporter.cs
using UnityEngine;
using UnityEditor;

public class HexTileImporter : AssetPostprocessor
{
    // Only apply to sprites in your hex tile folder
    private const string HEX_TILE_PATH = "Assets/Sprites/HexTiles";
    private const float TARGET_UNITY_WIDTH = 0.88f; // your reference tile's width in Unity units

    void OnPreprocessTexture()
    {
        if (!assetPath.Contains(HEX_TILE_PATH)) return;

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.filterMode = FilterMode.Trilinear; // or Trilinear for extra smoothness
        importer.mipmapEnabled = false; // mipmaps cause blurring on 2D sprites
        importer.textureCompression = TextureImporterCompression.Uncompressed; // prevents compression artifacts
        importer.isReadable = true;

        int width, height;
        importer.GetSourceTextureWidthAndHeight(out width, out height);
        int correctPPU = Mathf.RoundToInt(width / TARGET_UNITY_WIDTH);
        importer.spritePixelsPerUnit = correctPPU;
    }
}