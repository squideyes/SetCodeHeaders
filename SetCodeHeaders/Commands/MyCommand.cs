using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SetCodeHeaders
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        private readonly List<string> fileNames = new();

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var projects = (await VS.Solutions.GetAllProjectsAsync())?.ToList();

            if (!projects.Any())
                return;

            var (hasLines, lines) = GetHeaderLines(projects);

            if (!hasLines)
                return;

            foreach (var project in projects)
            {
                foreach (var file in project.Children.Where(
                    c => c.Type == SolutionItemType.PhysicalFile))
                {
                    if (file.FullPath.HasValidExtension())
                        fileNames.Add(file.FullPath);
                }
            }

            if (fileNames.Count == 0)
                return;

            foreach (var fileName in fileNames)
            {
                var fileKind = fileName.ToFileKind();

                if (await VS.Documents.IsOpenAsync(fileName))
                {
                    var docView = await VS.Documents.GetDocumentViewAsync(fileName);

                    var sb = new StringBuilder();

                    foreach (var line in docView.TextBuffer.CurrentSnapshot.Lines)
                        sb.AppendLine(line.GetText());

                    var oldText = sb.ToString();

                    var newText = GetHeaderText(fileKind, lines)
                        + oldText.WithoutHeader(fileKind);

                    if (newText == oldText)
                        continue;

                    using var edit = docView.TextBuffer.CreateEdit();

                    edit.Replace(0, 
                        docView.TextBuffer.CurrentSnapshot.Length, newText);

                    edit.Apply();

                    if (docView?.TextView == null)
                        continue;

                    docView.TextView.MoveCaretTo(0);
                }
                else
                {
                    var oldText = File.ReadAllText(fileName);

                    var newText = GetHeaderText(fileKind, lines) 
                        + oldText.WithoutHeader(fileKind);

                    if (newText == oldText)
                        continue;

                    File.WriteAllText(fileName, newText);
                }
            }
        }

        private string GetHeaderText(FileKind fileKind, List<string> lines)
        {
            var sb = new StringBuilder();

            switch (fileKind)
            {
                case FileKind.CS:
                    foreach (var line in lines)
                        sb.AppendLine("// " + line);
                    sb.AppendLine();
                    break;
                case FileKind.CONFIG:
                case FileKind.XAML:
                case FileKind.XML:
                case FileKind.XSD:
                    sb.AppendLine("<!-- ");
                    foreach (var line in lines)
                        sb.AppendLine(line);
                    sb.AppendLine("-->");
                    sb.AppendLine();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileKind));
            }

            return sb.ToString();
        }

        private (bool, List<string>) GetHeaderLines(List<Project> projects)
        {
            static Solution GetSolution(SolutionItem item)
            {
                while (item.Parent != null)
                    item = item.Parent;

                return item as Solution;
            }

            var solution = GetSolution(projects.First());

            var folder = Path.GetDirectoryName(solution.FullPath);

            var fileName = Path.GetFileNameWithoutExtension(solution.FullPath);

            var fullPath = Path.Combine(folder, fileName + ".sln.headertext");

            if (!File.Exists(fullPath))
                return (false, null);

            var text = File.ReadAllText(fullPath);

            if (text.Length == 0)
                return (false, null);

            var lines = text.ToLines();

            if (lines.All(l => string.IsNullOrWhiteSpace(l)))
                return (false, null);

            return (true, lines.TrimEmptyLines());
        }
    }
}
