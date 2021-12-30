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

            var (hasLines, headerLines) = GetHeaderLines(fullPath);

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
                    if (file.FullPath.IsCsFile())
                        fileNames.Add(file.FullPath);
                }
            }

            if (fileNames.Count == 0)
            {
                Package.ShowMessageBox(
                     $"The \"{solution.ToNameOnly()}\" solution does not contain .CS files!");

                return;
            }

            int updated = 0;
            int skipped = 0;

            bool GetNewLines(List<string> oldLines, out List<string> newLines)
            {
                newLines = new List<string>();

                newLines.AddRange(headerLines);
                newLines.Add("");
                newLines.AddRange(oldLines.WithoutHeader());

                var changed = !oldLines.SequenceEqual(newLines);

                if (!changed)
                    skipped++;

                return changed;
            };

            foreach (var fileName in fileNames)
            {
                if (await VS.Documents.IsOpenAsync(fileName))
                {
                    var docView = await VS.Documents
                        .GetDocumentViewAsync(fileName);

                    var oldLines = docView.TextBuffer.CurrentSnapshot
                        .Lines.Select(l => l.GetText()).ToList();

                    if (!GetNewLines(oldLines, out List<string> newLines))
                        continue;

                    var newText = string.Join("\r\n", newLines);

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

                    if (string.IsNullOrEmpty(oldText))
                    {
                        skipped++;

                        continue;
                    }

                    var oldLines = oldText.ToLines();

                    if (!GetNewLines(oldLines, out List<string> newLines))
                        continue;

                    var newText = string.Join("\r\n", newLines);

                    File.WriteAllText(fileName, newText);

                    updated++;
                }
            }

            MiscHelpers.ShowMessageBox(Package,
                $"{updated:N0} \"{solution.ToNameOnly()}\" files were updated ({skipped:N0} were skipped).",
                MessageBoxKind.Info);
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

            var result = new List<string>();

            while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines.First()))
                lines.RemoveAt(0);

            while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines.Last()))
                lines.RemoveAt(lines.Count - 1);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    result.Add("//");
                else
                    result.Add("// " + line.TrimEnd());
            }

            return (true, result);
        }
    }
}