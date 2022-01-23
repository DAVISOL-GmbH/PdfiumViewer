using Davisol.Pdfium.IO;
using Davisol.Pdfium.Model.Document;
using Davisol.Pdfium.Model.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Davisol.Pdfium
{
    public static class PdfiumExtensions
    {
        public static T CreateService<T>(this IPdfDocument document, Func<IPdfDocument, IPdfFile, T> createServiceFunc)
        {
            if (document is null || createServiceFunc is null || !(document is PdfDocument pdfDocument))
                return default;

            return createServiceFunc(pdfDocument, pdfDocument.File);
        }

        public static IList<PdfRectangle> DistinctBounds(this IList<PdfRectangle> allBounds)
        {
            if (!(allBounds?.Any() ?? false))
                return allBounds;

            var result = new List<PdfRectangle>();

            var left = 0f;
            var right = 0f;


            return result.ToArray();
        }
    }
}
