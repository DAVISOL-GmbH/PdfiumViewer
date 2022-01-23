using Davisol.Pdfium.Model.Bookmark;
using System;

namespace Davisol.Pdfium.IO
{
    public interface IPdfFile : IDisposable
    {
        IntPtr DocumentPointer { get; }

        IntPtr FormPointer { get; }

        PdfBookmarkCollection Bookmarks { get; }
    }
}