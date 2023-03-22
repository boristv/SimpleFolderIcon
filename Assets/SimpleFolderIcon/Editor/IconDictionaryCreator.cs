using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimpleFolderIcon.Editor
{
    public class IconDictionaryCreator : AssetPostprocessor
    {
        private const string AssetName = "SimpleFolderIcon";
        internal static Dictionary<string, Texture> IconDictionary;

        private static string _iconsPath;

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (!ContainsIconAsset(importedAssets) &&
                !ContainsIconAsset(deletedAssets) &&
                !ContainsIconAsset(movedAssets) &&
                !ContainsIconAsset(movedFromAssetPaths))
            {
                return;
            }

            BuildDictionary();
        }

        private static bool ContainsIconAsset(string[] assets)
        {
            foreach (string str in assets)
            {
                if (ReplaceSeparatorChar(Path.GetDirectoryName(str)) == _iconsPath)
                {
                    return true;
                }
            }
            return false;
        }

        private static string ReplaceSeparatorChar(string path)
        {
            return path.Replace("\\", "/");
        }

        internal static void BuildDictionary()
        {
            _iconsPath = UpdateIconsPath();
            
            var dictionary = new Dictionary<string, Texture>();
            
            var dir = new DirectoryInfo(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/" + _iconsPath);
            FileInfo[] info = dir.GetFiles("*.png");
            foreach(FileInfo f in info)
            {
                var texture = (Texture)AssetDatabase.LoadAssetAtPath($"{_iconsPath}/{f.Name}", typeof(Texture2D));
                dictionary.Add(Path.GetFileNameWithoutExtension(f.Name),texture);
            }

            IconDictionary = dictionary;
        }
        
        private static string UpdateIconsPath()
        {
            var folders = new List<string>();
            GetAllSubfolders("Assets", ref folders);
            var path = folders.First(name => name.Substring((name.LastIndexOf('/') + 1)) == AssetName);
            path += "/Icons";
            return path;
        }

        private static void GetAllSubfolders(string startFolder, ref List<string> folders)
        {
            folders.Add(startFolder);
            var subFolders = AssetDatabase.GetSubFolders(startFolder);
            foreach (var subFolder in subFolders)
            {
                GetAllSubfolders(subFolder, ref folders);
            }
        }
    }
}
