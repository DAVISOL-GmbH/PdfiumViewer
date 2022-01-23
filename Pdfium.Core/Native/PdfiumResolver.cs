using System;
using System.Collections.Generic;
using System.Text;

namespace Davisol.Pdfium.Native
{
    public class PdfiumResolver
    {
        public static event PdfiumResolveEventHandler Resolve;

        private static void OnResolve(PdfiumResolveEventArgs e)
        {
            Resolve?.Invoke(null, e);
        }

        public static string GetPdfiumFileName()
        {
            var e = new PdfiumResolveEventArgs();
            OnResolve(e);
            return e.PdfiumFileName;
        }
    }
}
