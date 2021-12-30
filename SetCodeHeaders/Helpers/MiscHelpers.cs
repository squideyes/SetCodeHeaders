using Microsoft.VisualStudio.Shell.Interop;
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
        private static readonly Regex solutionNameRegex =
            new("(?<=').*?(?=')", RegexOptions.Compiled);

        public static bool IsCsFile(this string value) => Path.GetExtension(value)
            .Equals(".cs", StringComparison.OrdinalIgnoreCase);

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

        public static List<string> WithoutHeader(this List<string> oldLines)
        {
            var newLines = new List<string>(oldLines);

            while (newLines.Count > 0 && string.IsNullOrWhiteSpace(newLines.First()))
                newLines.RemoveAt(0);

            while (newLines.Count > 0 && newLines.First().StartsWith("//"))
                newLines.RemoveAt(0);

            while (newLines.Count > 0 && string.IsNullOrWhiteSpace(newLines.First()))
                newLines.RemoveAt(0);

            while (newLines.Count > 0 && string.IsNullOrWhiteSpace(newLines.Last()))
                newLines.RemoveAt(newLines.Count - 1);

            return newLines;
        }

        public static string ToNameOnly(this Solution solution) =>
            solutionNameRegex.Match(solution.Name).Value;

        public static void ShowMessageBox(this AsyncPackage package, string message,
            MessageBoxKind kind = MessageBoxKind.Warning)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var icon = kind switch
            {
                MessageBoxKind.Info => OLEMSGICON.OLEMSGICON_INFO,
                MessageBoxKind.Warning => OLEMSGICON.OLEMSGICON_WARNING,
                _ => throw new ArgumentOutOfRangeException(nameof(kind))
            };

            VsShellUtilities.ShowMessageBox(package, message, "SetCodeHeaders", icon,
                OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        public static List<T> Flatten<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> f) =>
            e.SelectMany(c => f(c).Flatten(f)).Concat(e).ToList();
    }
}