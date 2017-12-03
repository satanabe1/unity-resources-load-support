using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace ResourcesSupport
{

    /// <summary>
    /// Resources.Loadをサポートするクラスを作成するクラス
    /// </summary>
    public class ResourcesLoaderCreater
    {
        /// <summary>
        /// 生成するクラス名
        /// </summary>
        private static readonly string CreateClassName = "ResourcesLoader";

        /// <summary>
        /// 記述するEnum名
        /// </summary>
        private static readonly string EditEnumName = "AssetName";

        /// <summary>
        /// 記述するパス配列名
        /// </summary>
        private static readonly string EditPathArrayName = "AssetPaths";

        /// <summary>
        /// 記述するメソッド名
        /// </summary>
        private static readonly string EditMethodName = "Load";

        /// <summary>
        /// 無視する拡張子
        /// </summary>
        private static readonly string[] IgnoreExtensions = new string[]
        {
            ".meta"
        };

        [MenuItem("ResourcesSupport/Create Loader")]
        public static void Create()
        {
            var builder = new StringBuilder();

            // Using定義
            builder.AppendLine("using UnityEngine;");

            // クラス定義開始
            builder.AppendLine("public static class " + CreateClassName);
            builder.AppendLine("{");
            {
                // Assets内にあるResourcesフォルダの全てのパスを取得
                var resourcesPaths = Directory.GetDirectories("Assets", "Resources", SearchOption.AllDirectories);

                // Resourcesフォルダ内のファイルパスを取得
                var removeWord = "Resources/";
                var removeWordLength = removeWord.Length;
                var filePaths = GetFilePaths(resourcesPaths, IgnoreExtensions).Select(path =>
                    {
                        var startIndex = path.IndexOf(removeWord) + removeWordLength;
                        var length = path.Length - startIndex - Path.GetExtension(path).Length;
                        return path.Substring(startIndex, length);
                    });

                // ファイルパスからファイル名取得
                var fileNames = filePaths.Select(path => Path.GetFileNameWithoutExtension(path));

                // Enum定義開始
                builder.AppendLine("public enum " + EditEnumName);
                builder.AppendLine("{");
                {
                    builder.AppendLine(string.Join(",\n", fileNames.ToArray()));
                }
                builder.AppendLine("}");

                // ファイルパス配列定義開始
                builder.AppendLine(string.Format("private static readonly string[] {0} = new string[]", EditPathArrayName));
                builder.AppendLine("{");
                {
                    builder.AppendLine(string.Join(",\n", filePaths.Select(path => "\"" + path + "\"").ToArray()));
                }
                builder.AppendLine("};");

                builder.AppendLine(string.Format("public static T {0}<T>({1} name) where T : UnityEngine.Object", EditMethodName, EditEnumName));
                builder.AppendLine("{");
                {
                    builder.AppendLine(string.Format("return Resources.Load<T>({0}[(int){1}]);", EditPathArrayName, "name"));
                }
                builder.AppendLine("}");
            }
            builder.AppendLine("}");

            // スクリプト作成
            File.WriteAllText("Assets/" + CreateClassName + ".cs", builder.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
        }

        /// <summary>
        /// ファイルパスを取得
        /// </summary>
        private static IEnumerable<string> GetFilePaths(IEnumerable<string> paths, IEnumerable<string> ignoreExtensions)
        {
            var filePathList = new List<string>();
            foreach (var path in paths)
            {
                var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                filePathList.AddRange(files);
            }

            return filePathList
            .Where(path => !ignoreExtensions.Contains(Path.GetExtension(path))) // 無視する拡張子のファイルをフィルタリング
            .Select(path => path.Replace("\\", "/"));                           // ファイルパスの「￥」を「/」に変換（WindowsとMacの差を吸収するため）
        }
    }
}
