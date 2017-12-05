using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ResourcesSupport
{
    public static class StringBuilderExtension
    {
        private static readonly char IndentChar = ' ';
        private static readonly int IndentLength = 4;

        public static void AppendLineFormat(this StringBuilder builder, string format, params object[] arg0)
        {
            builder.AppendFormat(format + "\n", arg0);
        }

        public static void AppendUsing(this StringBuilder builder, int indent, params string[] usings)
        {
            if (usings == null)
            {
                return;
            }

            var indentString = GetIndentString(indent);
            foreach (var editUsing in usings)
            {
                if (string.IsNullOrEmpty(editUsing))
                {
                    return;
                }

                builder.AppendLineFormat("{0}using {1};", indentString, editUsing);
            }
        }

        public static void AppendUsing(this StringBuilder builder, int indent, params IUsings[] usings)
        {
            foreach (var editUsings in usings)
            {
                builder.AppendUsing(indent, editUsings.usings);
            }
        }

        public static void AppendClass(this StringBuilder builder, string name, int indent = 0, string modifier = "", params string[] summaryArray)
        {
            builder.AppendSummary(indent, summaryArray);
            
            var indentString = GetIndentString(indent);
            builder.AppendLineFormat("{0}public {1}class {2}", indentString, string.IsNullOrEmpty(modifier) ? "" : modifier + " ", name);
            builder.AppendLine(indentString + "{");
        }

        public static void AppendEnum(this StringBuilder builder, string name, IEnumerable<string> elements, int indent = 0, string elementPrefix = "", string elementSuffix = "", params string[] summaryArray)
        {
            builder.AppendSummary(indent, summaryArray);

            var indentString = GetIndentString(indent);
            builder.AppendLine(indentString + "public enum " + name);
            builder.AppendLine(indentString + "{");
            builder.AppendLine(Join(elements, indent + 1, elementPrefix, elementSuffix));
            builder.AppendLine(indentString + "}");
        }

        public static void AppendSummary(this StringBuilder builder, int Indent = 0, params string[] summaryArray)
        {
            if (summaryArray == null || summaryArray.Length == 0)
            {
                return;
            }
            
            var prefix = GetIndentString(Indent) + "/// ";

            builder.AppendLine(prefix + "<summary>");
            foreach (var summary in summaryArray)
            {
                builder.AppendLine(prefix + summary);
            }
            builder.AppendLine(prefix + "</summary>");
        }

        public static string Join(IEnumerable<string> elements, int indent = 0, string elementPrefix = "", string elementSuffix = "", string separator = ",\n")
        {
            var prefix = GetIndentString(indent) + elementPrefix;
            return string.Join(separator, elements.Select(element => prefix + element + elementSuffix).ToArray());
        }

        public static string GetIndentString(int indent)
        {
            return new string(IndentChar, indent * IndentLength);
        }
    }
}