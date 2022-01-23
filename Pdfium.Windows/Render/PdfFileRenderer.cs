using Davisol.Pdfium.IO;
using Davisol.Pdfium.Model.Document;
using Davisol.Pdfium.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davisol.Pdfium.Windows.Render
{
    internal class PdfFileRenderer : IDisposable
    {
        private readonly IPdfDocument _document;
        private readonly IPdfFile _file;
        private bool _disposed;

        private PdfFileRenderer(IPdfDocument document, IPdfFile file)
        {
            _document = document;
            _file = file;
        }

        internal static PdfFileRenderer ForDocument(IPdfDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return document.CreateService(CreateInstance);
        }

        private static PdfFileRenderer CreateInstance(IPdfDocument document, IPdfFile file)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            return new PdfFileRenderer(document, file);
        }



        public bool RenderPDFPageToDC(int pageNumber, IntPtr dc, int dpiX, int dpiY, int boundsOriginX, int boundsOriginY, int boundsWidth, int boundsHeight, NativeMethods.FPDF flags)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            using (var pageData = new PageData(_file, pageNumber))
            {
                NativeMethods.FPDF_RenderPage(dc, pageData.Page, boundsOriginX, boundsOriginY, boundsWidth, boundsHeight, 0, flags);
            }

            return true;
        }

        public bool RenderPDFPageToBitmap(int pageNumber, IntPtr bitmapHandle, int dpiX, int dpiY, int boundsOriginX, int boundsOriginY, int boundsWidth, int boundsHeight, int rotate, NativeMethods.FPDF flags, bool renderFormFill)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            using (var pageData = new PageData(_file, pageNumber))
            {
                if (renderFormFill)
                    flags &= ~NativeMethods.FPDF.ANNOT;

                NativeMethods.FPDF_RenderPageBitmap(bitmapHandle, pageData.Page, boundsOriginX, boundsOriginY, boundsWidth, boundsHeight, rotate, flags);

                if (renderFormFill)
                    NativeMethods.FPDF_FFLDraw(_file.FormPointer, bitmapHandle, pageData.Page, boundsOriginX, boundsOriginY, boundsWidth, boundsHeight, rotate, flags);
            }

            return true;
        }

        public Point PointFromPdf(int page, PointF point)
        {
            using (var pageData = new PageData(_file, page))
            {
                NativeMethods.FPDF_PageToDevice(
                    pageData.Page,
                    0,
                    0,
                    (int)pageData.Width,
                    (int)pageData.Height,
                    0,
                    point.X,
                    point.Y,
                    out var deviceX,
                    out var deviceY
                );

                return new Point(deviceX, deviceY);
            }
        }

        public Rectangle RectangleFromPdf(int page, RectangleF rect)
        {
            using (var pageData = new PageData(_file, page))
            {
                NativeMethods.FPDF_PageToDevice(
                    pageData.Page,
                    0,
                    0,
                    (int)pageData.Width,
                    (int)pageData.Height,
                    0,
                    rect.Left,
                    rect.Top,
                    out var deviceX1,
                    out var deviceY1
                );

                NativeMethods.FPDF_PageToDevice(
                    pageData.Page,
                    0,
                    0,
                    (int)pageData.Width,
                    (int)pageData.Height,
                    0,
                    rect.Right,
                    rect.Bottom,
                    out var deviceX2,
                    out var deviceY2
                );

                return new Rectangle(
                    deviceX1,
                    deviceY1,
                    deviceX2 - deviceX1,
                    deviceY2 - deviceY1
                );
            }
        }

        public PointF PointToPdf(int page, Point point)
        {
            using (var pageData = new PageData(_file, page))
            {
                NativeMethods.FPDF_DeviceToPage(
                    pageData.Page,
                    0,
                    0,
                    (int)pageData.Width,
                    (int)pageData.Height,
                    0,
                    point.X,
                    point.Y,
                    out var deviceX,
                    out var deviceY
                );

                return new PointF((float)deviceX, (float)deviceY);
            }
        }

        public RectangleF RectangleToPdf(int page, Rectangle rect)
        {
            using (var pageData = new PageData(_file, page))
            {
                NativeMethods.FPDF_DeviceToPage(
                    pageData.Page,
                    0,
                    0,
                    (int)pageData.Width,
                    (int)pageData.Height,
                    0,
                    rect.Left,
                    rect.Top,
                    out var deviceX1,
                    out var deviceY1
                );

                NativeMethods.FPDF_DeviceToPage(
                    pageData.Page,
                    0,
                    0,
                    (int)pageData.Width,
                    (int)pageData.Height,
                    0,
                    rect.Right,
                    rect.Bottom,
                    out var deviceX2,
                    out var deviceY2
                );

                return new RectangleF(
                    (float)deviceX1,
                    (float)deviceY1,
                    (float)(deviceX2 - deviceX1),
                    (float)(deviceY2 - deviceY1)
                );
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private class PageData : IDisposable
        {
            private readonly IntPtr _form;
            private bool _disposed;

            public IntPtr Page { get; private set; }

            public IntPtr TextPage { get; private set; }

            public double Width { get; private set; }

            public double Height { get; private set; }

            public PageData(IPdfFile file, int pageNumber)
            {
                var document = file.DocumentPointer;
                var form = file.FormPointer;

                _form = form;

                Page = NativeMethods.FPDF_LoadPage(document, pageNumber);
                TextPage = NativeMethods.FPDFText_LoadPage(Page);
                NativeMethods.FORM_OnAfterLoadPage(Page, form);
                NativeMethods.FORM_DoPageAAction(Page, form, NativeMethods.FPDFPAGE_AACTION.OPEN);

                Width = NativeMethods.FPDF_GetPageWidth(Page);
                Height = NativeMethods.FPDF_GetPageHeight(Page);
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    NativeMethods.FORM_DoPageAAction(Page, _form, NativeMethods.FPDFPAGE_AACTION.CLOSE);
                    NativeMethods.FORM_OnBeforeClosePage(Page, _form);
                    NativeMethods.FPDFText_ClosePage(TextPage);
                    NativeMethods.FPDF_ClosePage(Page);

                    _disposed = true;
                }
            }
        }
    }
}
