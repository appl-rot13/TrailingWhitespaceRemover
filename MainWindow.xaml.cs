
namespace TrailingWhitespaceRemover
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;

    public partial class MainWindow
    {
        private static readonly Encoding FileEncoding = Encoding.GetEncoding("Shift-JIS");

        private static readonly string FileNewLine = Environment.NewLine;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            base.OnPreviewDragOver(e);

            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        protected override void OnPreviewDrop(DragEventArgs e)
        {
            base.OnPreviewDrop(e);

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (filePaths == null)
            {
                return;
            }

            foreach (var filePath in filePaths)
            {
                try
                {
                    RemoveTrailingWhitespace(filePath);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"{exception}");
                }
            }
        }

        private static void RemoveTrailingWhitespace(string readFilePath)
        {
            if (string.IsNullOrWhiteSpace(readFilePath))
            {
                return;
            }

            var writeFilePath = Path.Combine(
                Path.GetDirectoryName(readFilePath) ?? string.Empty,
                string.Format(
                    "{0}_Removed{1}",
                    Path.GetFileNameWithoutExtension(readFilePath),
                    Path.GetExtension(readFilePath)));

            var lines = ReadFile(readFilePath).Select(line => line.TrimEnd());
            WriteFile(writeFilePath, lines);
        }

        private static IEnumerable<string> ReadFile(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream, FileEncoding))
            {
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }
            }
        }

        private static void WriteFile(string filePath, IEnumerable<string> lines)
        {
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(stream, FileEncoding))
            {
                writer.NewLine = FileNewLine;

                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}
