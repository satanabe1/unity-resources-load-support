﻿using System.Text;
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
        /// 記述するEnum名
        /// </summary>
        private static readonly string EnumNameFormat = "{0}Name";

        /// <summary>
        /// 記述するパス配列名
        /// </summary>
        private static readonly string PathArrayNameFormat = "{0}Paths";

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

        public static void Create(ResourcesLoaderSetting setting)
        {
            var builder = new StringBuilder();

            // インデントの深さ
            int indent = 0;

            // Using記述
            var usingsList = new List<IUsings>();
            usingsList.Add(setting);
            usingsList.AddRange(setting.parameters);

            var usingNames = new HashSet<string>();
            foreach (var usings in usingsList)
            {
                foreach (var names in usings.usings)
                {
                    foreach(var name in names) 
                    {
                        usingNames.Add(names);
                    }
                }
            }
            builder.AppendUsing(indent, usingNames.ToArray());

            // クラス記述開始
            builder.AppendClass(setting.createClassName, indent, "static", "Resources.Loadをラップしたクラスです", "※自動生成されたクラスです");
            indent++;
            {
                // Assets内にあるResourcesフォルダの全てのパスを取得
                var resourcesPaths = Directory.GetDirectories("Assets", "Resources", SearchOption.AllDirectories);

                // Resourcesフォルダ内のファイルパスを取得
                var filePaths = GetFilePaths(resourcesPaths, IgnoreExtensions);

                var removeWord = "Resources/";
                var removeWordLength = removeWord.Length;
               
                // parameterで指定された取得処理を記述する
                foreach (var parameter in setting.parameters)
                {
                    // 指定した拡張子のファイルパスを取得
                    var paths = FindByExtension(filePaths, parameter.targetExtensions).Select(path =>
                        {
                            var startIndex = path.IndexOf(removeWord) + removeWordLength;
                            var length = path.Length - startIndex - Path.GetExtension(path).Length;
                            return path.Substring(startIndex, length);
                        });

                    // ファイルパスからファイル名を取得
                    var fileNames = paths.Select(path => Path.GetFileNameWithoutExtension(path));

                    // Enum記述開始
                    var editEnumName = string.Format(EnumNameFormat, parameter.typeName);
                    builder.AppendEnum(editEnumName, fileNames, indent);

                    // パス配列記述開始
                    var editPathArrayName = string.Format(PathArrayNameFormat, parameter.typeName);
                    builder.AppendLineFormat("{0}private static readonly string[] {1} = new string[]", StringBuilderExtension.GetIndentString(indent), editPathArrayName);
                    builder.AppendLine(StringBuilderExtension.GetIndentString(indent) + "{");
                    indent++;
                    {
                        builder.AppendLine(StringBuilderExtension.Join(paths, indent, "\"", "\""));
                    }
                    indent--;
                    builder.AppendLine(StringBuilderExtension.GetIndentString(indent) + "};");

                    // 取得関数記述開始
                    var argumentName = "name";
                    builder.AppendLineFormat("{0}public static {1} {2}({3} {4})", StringBuilderExtension.GetIndentString(indent), parameter.typeName, EditMethodName, editEnumName, argumentName);
                    builder.AppendLine(StringBuilderExtension.GetIndentString(indent) + "{");
                    indent++;
                    {
                        builder.AppendLineFormat("{0}return Resources.Load<{1}>({2}[(int){3}]);", StringBuilderExtension.GetIndentString(indent), parameter.typeName, editPathArrayName, argumentName);
                    }
                    indent--;
                    builder.AppendLine(StringBuilderExtension.GetIndentString(indent) + "}");
                }
            }
            indent--;
            builder.AppendLine(StringBuilderExtension.GetIndentString(indent) + "}");

            // スクリプト作成
            var createPath = string.IsNullOrEmpty(setting.createPath) ? "Assets" : Path.Combine("Assets", setting.createPath);
            File.WriteAllText(Path.Combine(createPath, setting.createClassName + ".cs"), builder.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
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

        /// <summary>
        /// 指定した拡張子のファイルパスを検索
        /// </summary>
        private static IEnumerable<string> FindByExtension(IEnumerable<string> paths, IEnumerable<string> extensions)
        {
            var builder = new StringBuilder();
            foreach (var extension in extensions)
            {
                if (builder.Length == 0)
                {
                    builder.Append(@".*\.(");
                }
                else
                {
                    builder.Append("|");
                }
                
                builder.Append(extension);
            }
            builder.Append(")");

            var regex = new Regex(@builder.ToString(), RegexOptions.IgnoreCase);

            return paths.Where(path => regex.IsMatch(path));
        }
    }
}
