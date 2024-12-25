using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public partial class SpineExportWindow : EditorWindow
{
    public class TexturePackingSetting
    {
        [JsonProperty("class")]
        public string type = "export-json";
        public string name = "JSON";
        public bool open = false;
        public string extension = ".json";
        public string format = "JSON";
        public bool prettyPrint = false;
        public bool nonessential = true;
        public bool cleanUp = false;

        public PackAtlasSetting packAtlas = null;

        public string packSource = "attachments";
        public string packTarget = "perskeleton";
        public bool warnings = true;

        public TexturePackingSetting(int maxWidth, int maxHeight)
        {
            packAtlas = new PackAtlasSetting(maxWidth, maxHeight);
        }
    }

    public class PackAtlasSetting
    {
        public bool stripWhitespaceX = true;
        public bool stripWhitespaceY = true;
        public bool rotation = true;
        public bool alias = true;
        public bool ignoreBlankImages = false;
        public int alphaThreshold = 3;
        public int minWidth = 16;
        public int minHeight = 16;
        public int maxWidth = 2048;
        public int maxHeight = 2048;
        public bool pot = true;
        public bool multipleOfFour = false;
        public bool square = false;
        public string outputFormat = "png";
        public float jpegQuality = 0.9f;
        public bool premultiplyAlpha = true;
        public bool bleed = false;
        public float[] scale = { 1f };
        public string[] scaleSuffix = { "" };
        public string[] scaleResampling = { "bicubic" };
        public int paddingX = 2;
        public int paddingY = 2;
        public bool edgePadding = true;
        public bool duplicatePadding = true;
        public string filterMin = "Linear";
        public string filterMag = "Linear";
        public string wrapX = "ClampToEdge";
        public string wrapY = "ClampToEdge";
        public string format = "RGBA8888";
        public string atlasExtension = ".atlas.txt";
        public bool combineSubdirectories = false;
        public bool flattenPaths = false;
        public bool useIndexes = false;
        public bool debug = false;
        public bool fast = false;
        public bool limitMemory = true;
        public bool currentProject = true;
        public string packing = "polygons";
        public bool silent = false;
        public bool ignore = false;
        public int bleedIterations = 2;

        public PackAtlasSetting(int maxWidth, int maxHeight)
        {
            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;
        }
    }
}
