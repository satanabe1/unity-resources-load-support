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

            // インデントの深さ
            int indent = 0;

            // Using記述
            builder.AppendUsing(indent, "UnityEngine");

            // クラス記述開始
            builder.AppendClass(CreateClassName, indent, "static", "Resources.Loadをラップしたクラスです", "※自動生成されたクラスです");
            indent++;
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

                // ファイルパスからファイル名を取得
                var fileNames = filePaths.Select(path => Path.GetFileNameWithoutExtension(path));

                // Enum記述開始
                builder.AppendEnum(EditEnumName, fileNames, indent);

                // パス配列記述開始
                builder.AppendLineFormat("{0}private static readonly string[] {1} = new string[]", StringBuilderExtension.GetIndentString(indent), EditPathArrayName);
                builder.AppendLine(StringBuilderExtension.GetIndentString(indent) + "{");
                indent++;
                {
                    builder.AppendLine(StringBuilderExtension.Join(filePaths, indent, "\"", "\""));
                }
                indent--;
                builder.AppendLine(StringBuilderExtension.GetIndentString(indent) + "};");

                // 取得関数記述開始
                builder.AppendLineFormat("{0}public static T {1}<T>({2} name) where T : UnityEngine.Object", StringBuilderExtension.GetIndentString(indent), EditMethodName, EditEnumName);
                builder.AppendLine(StringBuilderExtension.GetIndentString(indent) + "{");
                indent++;
                {
                    builder.AppendLineFormat("{0}return Resources.Load<T>({1}[(int){2}]);", StringBuilderExtension.GetIndentString(indent), EditPathArrayName, "name");
                }
                indent--;
                builder.AppendLine(StringBuilderExtension.GetIndentString(indent) + "}");
            }
            indent--;
            builder.AppendLine(StringBuilderExtension.GetIndentString(indent) + "}");

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
