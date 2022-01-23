using Davisol.Pdfium.Model.Shape;
using System;
using System.Collections.Generic;
using System.Text;

namespace Davisol.Pdfium.Model.Content
{
    public class PdfMatch
    {
        public string Text { get; }
        
        public PdfTextSpan TextSpan { get; }
        
        public int Page { get; }

        public IList<PdfRectangle> TextBounds { get; set; }

        public PdfMatch(string text, PdfTextSpan textSpan, int page)
        {
            Text = text;
            TextSpan = textSpan;
            Page = page;
        }
    }
}
