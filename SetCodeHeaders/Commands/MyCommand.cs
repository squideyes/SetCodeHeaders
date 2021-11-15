// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com) 
// 
// This file is part of SetCodeHeaders
// 
// The use of this source code is licensed under the terms 
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SetCodeHeaders
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var solution = await VS.Solutions.GetCurrentSolutionAsync();

            var projects = (await VS.Solutions.GetAllProjectsAsync())?.ToList();

            if (!projects.Any())
            {
                Package.ShowMessageBox(
                    $"The \"{solution.ToNameOnly()}\" solution does not contain a project!");

                return;
            }

            var fullPath = GetHeaderTextFullPath(solution);

            var (hasLines, lines) = GetHeaderLines(fullPath);

            if (!hasLines)
            {
                Package.ShowMessageBox(
                    $"The \"{fullPath}\" file does not contain header-text!");

                return;
            }

            var fileNames = new List<string>();

            foreach (var project in projects)
            {
                var files = project.Children.Flatten(c => c.Children)
                    .Where(i => i.Type == SolutionItemType.PhysicalFile);

                foreach (var file in files)
                {
                    if (file.FullPath.HasValidExtension())
                        fileNames.Add(file.FullPath);
                }
            }

            if (fileNames.Count == 0)
            {
                Package.ShowMessageBox(
                     $"The \"{solution.ToNameOnly()}\" solution does not contain updatable files!");

                return;
            }

            int updated = 0;
            int skipped = 0;

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
                    {
                        skipped++;

                        continue;
                    }

                    using var edit = docView.TextBuffer.CreateEdit();

                    edit.Replace(0,
                        docView.TextBuffer.CurrentSnapshot.Length, newText);

                    edit.Apply();

                    if (docView?.TextView == null)
                    {
                        skipped++;

                        continue;
                    }

                    docView.TextView.MoveCaretTo(0);

                    updated++;
                }
                else
                {
                    var oldText = File.ReadAllText(fileName);

                    var newText = GetHeaderText(fileKind, lines)
                        + oldText.WithoutHeader(fileKind);

                    if (newText == oldText)
                    {
                        skipped++;

                        continue;
                    }

                    File.WriteAllText(fileName, newText);

                    updated++;
                }
            }

            MiscHelpers.ShowMessageBox(Package,
                $"{updated:N0} \"{solution.ToNameOnly()}\" files were updated ({skipped:N0} were skipped).",
                MessageBoxKind.Info);
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

        private string GetHeaderTextFullPath(Solution solution)
        {
            var folder = Path.GetDirectoryName(solution.FullPath);

            var fileName = Path.GetFileNameWithoutExtension(solution.FullPath);

            return Path.Combine(folder, fileName + ".sln.headertext");
        }

        private (bool, List<string>) GetHeaderLines(string fullPath)
        {
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

