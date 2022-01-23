using Davisol.Pdfium.Model.Document;
using Davisol.Pdfium.Native;
using Davisol.Pdfium.Render;
using Davisol.Pdfium.Windows.Print;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Interop;

namespace Davisol.Pdfium.Windows.Render
{
    /// <summary>
    /// Provides functionality to render a PDF document.
    /// </summary>
    public class PdfDocumentRenderer : IPdfDocumentRenderer
    {
        private IPdfDocument _document;
        private PdfFileRenderer _fileRenderer;
        private bool _disposed;

        private PdfDocumentRenderer(IPdfDocument document, PdfFileRenderer fileRenderer)
        {
            _document = document;
            _fileRenderer = fileRenderer;
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="owner">Window to show any UI for.</param>
        /// <param name="path">Path to the PDF document.</param>
        public static PdfDocumentRenderer FromFile(IWin32Window owner, string path)
        {
            return FromFile(owner, path, null);
        }
        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="owner">Window to show any UI for.</param>
        /// <param name="path">Path to the PDF document.</param>
        public static PdfDocumentRenderer FromFile(IWin32Window owner, string path, string password)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return FromFile(owner, File.OpenRead(path), password);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="owner">Window to show any UI for.</param>
        /// <param name="stream">Stream for the PDF document.</param>
        public static PdfDocumentRenderer FromFile(IWin32Window owner, Stream stream)
        {
            return FromFile(owner, stream, null);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="owner">Window to show any UI for.</param>
        /// <param name="stream">Stream for the PDF document.</param>
        public static PdfDocumentRenderer FromFile(IWin32Window owner, Stream stream, string password)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return ForDocument(owner, CreateDocument(owner, stream, password));
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="owner">Window to show any UI for.</param>
        /// <param name="path">Path to the PDF document.</param>
        public static PdfDocumentRenderer ForDocument(IWin32Window owner, IPdfDocument document)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            var fileRenderer = PdfFileRenderer.ForDocument(document);
            if (fileRenderer == null)
                throw new ArgumentNullException(nameof(fileRenderer));

            return new PdfDocumentRenderer(document, fileRenderer);
        }

        private static IPdfDocument CreateDocument(IWin32Window owner, Stream stream, string password)
        {
            try
            {
                while (true)
                {
                    try
                    {
                        return PdfDocument.Load(stream, password);
                    }
                    catch (PdfException ex)
                    {
                        if (owner != null && ex.Error == PdfError.PasswordProtected)
                        {
                            //using (var form = new PasswordForm())
                            //{
                            //    if (form.ShowDialog(owner) == DialogResult.OK)
                            //    {
                            //        password = form.Password;
                            //        continue;
                            //    }
                            //}
                        }

                        throw;
                    }
                }
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }


        /// <summary>
        /// <see cref="IPdfDocumentRenderer.Document"/>
        /// </summary>
        public IPdfDocument Document => _document;

        /// <summary>
        /// Renders a page of the PDF document to the provided graphics instance.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="graphics">Graphics instance to render the page on.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="bounds">Bounds to render the page in.</param>
        /// <param name="forPrinting">Render the page for printing.</param>
        public void Render(int page, Graphics graphics, float dpiX, float dpiY, Rectangle bounds, bool forPrinting)
        {
            Render(page, graphics, dpiX, dpiY, bounds, forPrinting ? PdfRenderFlags.ForPrinting : PdfRenderFlags.None);
        }

        /// <summary>
        /// Renders a page of the PDF document to the provided graphics instance.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="graphics">Graphics instance to render the page on.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="bounds">Bounds to render the page in.</param>
        /// <param name="flags">Flags used to influence the rendering.</param>
        public void Render(int page, Graphics graphics, float dpiX, float dpiY, Rectangle bounds, PdfRenderFlags flags)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            float graphicsDpiX = graphics.DpiX;
            float graphicsDpiY = graphics.DpiY;

            var dc = graphics.GetHdc();

            try
            {
                if ((int)graphicsDpiX != (int)dpiX || (int)graphicsDpiY != (int)dpiY)
                {
                    var transform = new NativeMethods.XFORM
                    {
                        eM11 = graphicsDpiX / dpiX,
                        eM22 = graphicsDpiY / dpiY
                    };

                    NativeMethods.SetGraphicsMode(dc, NativeMethods.GM_ADVANCED);
                    NativeMethods.ModifyWorldTransform(dc, ref transform, NativeMethods.MWT_LEFTMULTIPLY);
                }

                var point = new NativeMethods.POINT();
                NativeMethods.SetViewportOrgEx(dc, bounds.X, bounds.Y, out point);

                bool success = _fileRenderer.RenderPDFPageToDC(
                    page,
                    dc,
                    (int)dpiX, (int)dpiY,
                    0, 0, bounds.Width, bounds.Height,
                    FlagsToFPDFFlags(flags)
                );

                NativeMethods.SetViewportOrgEx(dc, point.X, point.Y, out point);

                if (!success)
                    throw new Win32Exception();
            }
            finally
            {
                graphics.ReleaseHdc(dc);
            }
        }

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="forPrinting">Render the page for printing.</param>
        /// <returns>The rendered image.</returns>
        public Image Render(int page, float dpiX, float dpiY, bool forPrinting)
        {
            var size = _document.PageSizes[page];

            return Render(page, (int)size.Width, (int)size.Height, dpiX, dpiY, forPrinting);
        }

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="flags">Flags used to influence the rendering.</param>
        /// <returns>The rendered image.</returns>
        public Image Render(int page, float dpiX, float dpiY, PdfRenderFlags flags)
        {
            var size = _document.PageSizes[page];

            return Render(page, (int)size.Width, (int)size.Height, dpiX, dpiY, flags);
        }

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="width">Width of the rendered image.</param>
        /// <param name="height">Height of the rendered image.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="forPrinting">Render the page for printing.</param>
        /// <returns>The rendered image.</returns>
        public Image Render(int page, int width, int height, float dpiX, float dpiY, bool forPrinting)
        {
            return Render(page, width, height, dpiX, dpiY, forPrinting ? PdfRenderFlags.ForPrinting : PdfRenderFlags.None);
        }

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="width">Width of the rendered image.</param>
        /// <param name="height">Height of the rendered image.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="flags">Flags used to influence the rendering.</param>
        /// <returns>The rendered image.</returns>
        public Image Render(int page, int width, int height, float dpiX, float dpiY, PdfRenderFlags flags)
        {
            return Render(page, width, height, dpiX, dpiY, 0, flags);
        }

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="width">Width of the rendered image.</param>
        /// <param name="height">Height of the rendered image.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="rotate">Rotation.</param>
        /// <param name="flags">Flags used to influence the rendering.</param>
        /// <returns>The rendered image.</returns>
        public Image Render(int page, int width, int height, float dpiX, float dpiY, PdfRotation rotate, PdfRenderFlags flags)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            if ((flags & PdfRenderFlags.CorrectFromDpi) != 0)
            {
                width = width * (int)dpiX / 72;
                height = height * (int)dpiY / 72;
            }

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bitmap.SetResolution(dpiX, dpiY);

            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            try
            {
                var handle = NativeMethods.FPDFBitmap_CreateEx(width, height, 4, data.Scan0, width * 4);

                try
                {
                    uint background = (flags & PdfRenderFlags.Transparent) == 0 ? 0xFFFFFFFF : 0x00FFFFFF;

                    NativeMethods.FPDFBitmap_FillRect(handle, 0, 0, width, height, background);

                    bool success = _fileRenderer.RenderPDFPageToBitmap(
                        page,
                        handle,
                        (int)dpiX, (int)dpiY,
                        0, 0, width, height,
                        (int)rotate,
                        FlagsToFPDFFlags(flags),
                        (flags & PdfRenderFlags.Annotations) != 0
                    );

                    if (!success)
                        throw new Win32Exception();
                }
                finally
                {
                    NativeMethods.FPDFBitmap_Destroy(handle);
                }
            }
            finally
            {
                bitmap.UnlockBits(data);
            }

            return bitmap;
        }

        private NativeMethods.FPDF FlagsToFPDFFlags(PdfRenderFlags flags)
        {
            return (NativeMethods.FPDF)(flags & ~(PdfRenderFlags.Transparent | PdfRenderFlags.CorrectFromDpi));
        }

        /// <summary>
        /// Convert a point from device coordinates to page coordinates.
        /// </summary>
        /// <param name="page">The page number where the point is from.</param>
        /// <param name="point">The point to convert.</param>
        /// <returns>The converted point.</returns>
        public PointF PointToPdf(int page, Point point)
        {
            return _fileRenderer.PointToPdf(page, point);
        }

        /// <summary>
        /// Convert a point from page coordinates to device coordinates.
        /// </summary>
        /// <param name="page">The page number where the point is from.</param>
        /// <param name="point">The point to convert.</param>
        /// <returns>The converted point.</returns>
        public Point PointFromPdf(int page, PointF point)
        {
            return _fileRenderer.PointFromPdf(page, point);
        }

        /// <summary>
        /// Convert a rectangle from device coordinates to page coordinates.
        /// </summary>
        /// <param name="page">The page where the rectangle is from.</param>
        /// <param name="rect">The rectangle to convert.</param>
        /// <returns>The converted rectangle.</returns>
        public RectangleF RectangleToPdf(int page, Rectangle rect)
        {
            return _fileRenderer.RectangleToPdf(page, rect);
        }

        /// <summary>
        /// Convert a rectangle from page coordinates to device coordinates.
        /// </summary>
        /// <param name="page">The page where the rectangle is from.</param>
        /// <param name="rect">The rectangle to convert.</param>
        /// <returns>The converted rectangle.</returns>
        public Rectangle RectangleFromPdf(int page, RectangleF rect)
        {
            return _fileRenderer.RectangleFromPdf(page, rect);
        }

        /// <summary>
        /// Creates a <see cref="PrintDocument"/> for the PDF document.
        /// </summary>
        /// <returns></returns>
        public PrintDocument CreatePrintDocument()
        {
            return CreatePrintDocument(PdfPrintMode.CutMargin);
        }

        /// <summary>
        /// Creates a <see cref="PrintDocument"/> for the PDF document.
        /// </summary>
        /// <param name="printMode">Specifies the mode for printing. The default
        /// value for this parameter is CutMargin.</param>
        /// <returns></returns>
        public PrintDocument CreatePrintDocument(PdfPrintMode printMode)
        {
            return CreatePrintDocument(new PdfPrintSettings(printMode, null));
        }

        /// <summary>
        /// Creates a <see cref="PrintDocument"/> for the PDF document.
        /// </summary>
        /// <param name="settings">The settings used to configure the print document.</param>
        /// <returns></returns>
        public PrintDocument CreatePrintDocument(PdfPrintSettings settings)
        {
            return new PdfPrintDocument(this, settings);
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
                _fileRenderer?.Dispose();
                _fileRenderer = null;
                _document = null;

                _disposed = true;
            }
        }
    }
}
