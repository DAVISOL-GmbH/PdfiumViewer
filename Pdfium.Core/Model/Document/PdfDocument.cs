using Davisol.Pdfium.IO;
using Davisol.Pdfium.Model.Bookmark;
using Davisol.Pdfium.Model.Content;
using Davisol.Pdfium.Model.Link;
using Davisol.Pdfium.Model.Shape;
using Davisol.Pdfium.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Davisol.Pdfium.Model.Document
{
    /// <summary>
    /// <see cref="IPdfDocument"/>.
    /// </summary>
    public class PdfDocument : IPdfDocument
    {
        private bool _disposed;
        private PdfFile _file;
        private readonly List<FloatSize> _pageSizes;


        #region Instanciation and intialization

        private PdfDocument(Stream stream, string password)
        {
            _file = new PdfFile(stream, password);

            _pageSizes = _file.GetPDFDocInfo();
            if (_pageSizes == null)
                throw new Win32Exception();

            PageSizes = new ReadOnlyCollection<FloatSize>(_pageSizes);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="path">Path to the PDF document.</param>
        public static PdfDocument Load(string path)
        {
            return Load(path, null);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="path">Path to the PDF document.</param>
        /// <param name="password">Password for the PDF document.</param>
        public static PdfDocument Load(string path, string password)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Load(System.IO.File.OpenRead(path), password);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided stream.
        /// </summary>
        /// <param name="stream">Stream for the PDF document.</param>
        public static PdfDocument Load(Stream stream)
        {
            return Load(stream, null);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided stream.
        /// </summary>
        /// <param name="stream">Stream for the PDF document.</param>
        /// <param name="password">Password for the PDF document.</param>
        public static PdfDocument Load(Stream stream, string password)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return new PdfDocument(stream, password);
        }

        #endregion

        internal PdfFile File => _file;

        /// <summary>
        /// <see cref="IPdfDocument.PageCount"/>
        /// </summary>
        public int PageCount => PageSizes.Count;

        /// <summary>
        /// <see cref="IPdfDocument.Bookmarks"/>
        /// </summary>
        public PdfBookmarkCollection Bookmarks => _file?.Bookmarks;

        /// <summary>
        /// <see cref="IPdfDocument.PageSizes"/>
        /// </summary>
        public IList<FloatSize> PageSizes { get; private set; }

        /// <summary>
        /// <see cref="IPdfDocument.Save(string)"/>
        /// </summary>
        public void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            using (var stream = System.IO.File.Create(path))
            {
                Save(stream);
            }
        }

        /// <summary>
        /// <see cref="IPdfDocument.Save(Stream)"/>
        /// </summary>
        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _file.Save(stream);
        }

        /// <summary>
        /// <see cref="IPdfDocument.Search(string, bool, bool, bool)"/>
        /// </summary>
        public PdfMatches Search(string text, bool matchCase, bool wholeWord, bool readBounds = false)
        {
            return Search(text, matchCase, wholeWord, 0, PageCount - 1, readBounds);
        }

        /// <summary>
        /// <see cref="IPdfDocument.Search(string, bool, bool, int, bool)"/>
        /// </summary>
        public PdfMatches Search(string text, bool matchCase, bool wholeWord, int page, bool readBounds = false)
        {
            return Search(text, matchCase, wholeWord, page, page, readBounds);
        }

        /// <summary>
        /// <see cref="IPdfDocument.Search(string, bool, bool, int, int, bool)"/>
        /// </summary>
        public PdfMatches Search(string text, bool matchCase, bool wholeWord, int startPage, int endPage, bool readBounds = false)
        {
            return _file?.Search(text, matchCase, wholeWord, startPage, endPage, readBounds);
        }

        /// <summary>
        /// <see cref="IPdfDocument.GetPdfText(int)"/>
        /// </summary>
        public string GetPdfText(int page)
        {
            return _file?.GetPdfText(page);
        }

        /// <summary>
        /// <see cref="IPdfDocument.GetPdfText(PdfTextSpan)"/>
        /// </summary>
        public string GetPdfText(PdfTextSpan textSpan)
        {
            return _file?.GetPdfText(textSpan);
        }

        /// <summary>
        /// <see cref="IPdfDocument.GetTextBounds(PdfTextSpan)"/>
        /// </summary>
        public IList<PdfRectangle> GetTextBounds(PdfTextSpan textSpan)
        {
            return _file?.GetTextBounds(textSpan);
        }

        /// <summary>
        /// <see cref="IPdfDocument.GetPageLinks(int, FloatSize)"/>
        /// </summary>
        public PdfPageLinks GetPageLinks(int page, FloatSize size)
        {
            return _file?.GetPageLinks(page, size);
        }

        /// <summary>
        /// <see cref="IPdfDocument.DeletePage(int)"/>
        /// </summary>
        public void DeletePage(int page)
        {
            _file.DeletePage(page);
            _pageSizes.RemoveAt(page);
        }

        /// <summary>
        /// <see cref="IPdfDocument.RotatePage(int, PdfRotation)"/>
        /// </summary>
        public void RotatePage(int page, PdfRotation rotation)
        {
            _file.RotatePage(page, rotation);
            _pageSizes[page] = _file.GetPDFDocInfo(page);
        }

        /// <summary>
        /// <see cref="IPdfDocument.GetInformation"/>
        /// </summary>
        public PdfInformation GetInformation()
        {
            return _file.GetInformation();
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <param name="disposing">Whether this method is called from Dispose.</param>
        protected void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_file != null)
                {
                    _file.Dispose();
                    _file = null;
                }

                _disposed = true;
            }
        }
    }
}
