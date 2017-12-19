using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;

namespace ResourcesSupport
{
    public class ResourcesSupportPostprocessor : AssetPostprocessor
    {
        private static readonly string ResourcesWord = "Resources/";

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Any(p => p.Contains(ResourcesWord)) ||
                deletedAssets.Any(p => p.Contains(ResourcesWord)) ||
                movedAssets.Any(p => p.Contains(ResourcesWord)) ||
                movedFromAssetPaths.Any(p => p.Contains(ResourcesWord)))
            {
                foreach (var path in FindAssetPaths(typeof(ResourcesLoaderSetting)))
                {
                    var setting = AssetDatabase.LoadAssetAtPath<ResourcesLoaderSetting>(path);
                    if (setting.isAuto)
                    {
                        ResourcesLoaderCreater.Create(setting);
                        break;
                    }
                }
            }
        }

        public static IEnumerable<string> FindAssetPaths(Type type)
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:" + type.Name);

            foreach (var guid in guids)
            {
                yield return AssetDatabase.GUIDToAssetPath(guid);
            }
        }
    }
}