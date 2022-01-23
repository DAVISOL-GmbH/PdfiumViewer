using System;
using System.Collections.Generic;
using System.Text;

namespace Davisol.Pdfium.Model.Bookmark
{
    public class PdfBookmark
    {
        public string Title { get; set; }
        public int PageIndex { get; set; }

        public PdfBookmarkCollection Children { get; }

        public PdfBookmark()
        {
            Children = new PdfBookmarkCollection();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
