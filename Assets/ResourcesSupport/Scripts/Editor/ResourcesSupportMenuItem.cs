using UnityEditor;
using UnityEngine;
using System.IO;

namespace ResourcesSupport
{
    public class ResourcesLoaderMenuItem
    {
        private const string Prefix = "ResourcesSupport/";

        [MenuItem(Prefix + "Open Create Loader Window")]
        public static void CreateResourcesLoader()
        {
            ResourcesLoaderCreaterWindow.Open();
        }

        [MenuItem(Prefix + "Create Setting")]
        public static void CreateResourcesLoaderSetting()
        {
            CreateScriptableObject<ResourcesLoaderSetting>("Assets");
        }

        [MenuItem(Prefix + "Create Load Parameter")]
        public static void CreateLoadParameter()
        {
            CreateScriptableObject<LoadParameter>("Assets");
        }

        private static void CreateScriptableObject<T>(string dir) where T : ScriptableObject
        {
            var obj = ScriptableObject.CreateInstance<T>();
            ProjectWindowUtil.CreateAsset(obj, GetCreateScriptableObjectPath<T>(dir));
            AssetDatabase.Refresh();
        }

        private static string GetCreateScriptableObjectPath<T>(string dir)
        {
            var pathWithoutExtension = Path.Combine(dir, typeof(T).Name);
            var path = string.Format("{0}.asset", pathWithoutExtension);
            return AssetDatabase.GenerateUniqueAssetPath (path);
        }
    }
}