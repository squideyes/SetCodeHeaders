using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SetCodeHeaders
{
    public static class MiscHelpers
    {
        private static readonly Regex csHeaderRegex =
            new(@"^\s*(//.*[\r\n]?)*\s*", RegexOptions.Compiled);

        private static readonly Regex xmlHeaderRegex =
            new(@"^\s*<!--.*?-->\s*", RegexOptions.Compiled | RegexOptions.Singleline);

        public static bool HasValidExtension(this string value) =>
            Enum.TryParse(Path.GetExtension(value).Substring(1), true, out FileKind _);

        public static FileKind ToFileKind(this string value) =>
            Path.GetExtension(value).Substring(1).ToEnumValue<FileKind>();

        public static List<string> ToLines(this string text)
        {
            var reader = new StringReader(text);

            var lines = new List<string>();

            string line;

            while ((line = reader.ReadLine()) != null)
                lines.Add(line);

            return lines;
        }

        public static CaretPosition MoveCaretTo(this ITextView textView, int position) =>
            textView.Caret.MoveTo(new SnapshotPoint(textView.TextSnapshot, position));

        public static void SetText(this ITextView textView, params string[] lines) =>
            textView.TextBuffer.SetText(lines);

        public static void SetText(this ITextBuffer buffer, params string[] lines)
        {
            var text = string.Join(Environment.NewLine, lines);

            var edit = buffer.CreateEdit(EditOptions.DefaultMinimalChange, 0, null);

            edit.Replace(new Span(0, buffer.CurrentSnapshot.Length), text);

            edit.Apply();
        }

        private static T ToEnumValue<T>(this string value) =>
            (T)Enum.Parse(typeof(T), value, true);

        public static List<string> TrimEmptyLines(this List<string> value)
        {
            static bool HasContent(string line)
            {
                if (string.IsNullOrEmpty(line))
                    return false;

                foreach (char c in line)
                {
                    if (c != ' ' && c != '\t')
                        return true;
                }

                return false;
            }

            return value.SkipWhile(e => !HasContent(e))
                .Reverse()
                .SkipWhile(e => !HasContent(e))
                .Reverse()
                .ToList();
        }

        public static string WithoutHeader(this string text, FileKind fileKind)
        {
            string WithoutHeader(Regex regex)
            {
                var matches = regex.Matches(text);

                if (matches.Count > 0)
                    text = regex.Replace(text, "");

                return text;
            }

            if (fileKind == FileKind.CS)
                return WithoutHeader(csHeaderRegex);
            else
                return WithoutHeader(xmlHeaderRegex);
        }
    }
}
