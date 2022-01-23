using Davisol.Pdfium.Model.Document;
using System;
using System.Linq;

namespace Pdfium.Console.NetCore
{
    internal class Program
    {
        private const string FilePath = @"C:\TEMP\testdoc.pdf";

        private const string Needle = @"findme";

        static void Main(string[] args)
        {
            var pdfDocument = PdfDocument.Load(FilePath);

            var matches = pdfDocument.Search(Needle, false, false, true);

            var matchIndex = 1;
            foreach (var match in matches.Items)
            {
                System.Console.WriteLine($"Match {matchIndex++} on page {match.Page} within {match.Text}");
                if (match.TextBounds?.Any() ?? false)
                {
                    foreach (var textBounds in match.TextBounds)
                    {
                        System.Console.WriteLine($"  -> element is positioned at {textBounds.Bounds.Left}:{textBounds.Bounds.Top} with a size of {textBounds.Bounds.Width}:{textBounds.Bounds.Height}");
                    }
                }
            }
        }
    }
}
