﻿using Davisol.Pdfium.Model.Bookmark;
using Davisol.Pdfium.Model.Content;
using Davisol.Pdfium.Model.Link;
using Davisol.Pdfium.Model.Shape;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Davisol.Pdfium.Model.Document
{
    /// <summary>
    /// Represents a PDF document.
    /// </summary>
    public interface IPdfDocument : IDisposable
    {
        /// <summary>
        /// Number of pages in the PDF document.
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Bookmarks stored in this PdfFile
        /// </summary>
        PdfBookmarkCollection Bookmarks { get; }

        /// <summary>
        /// Size of each page in the PDF document.
        /// </summary>
        IList<FloatSize> PageSizes { get; }


        /// <summary>
        /// Save the PDF document to the specified location.
        /// </summary>
        /// <param name="path">Path to save the PDF document to.</param>
        void Save(string path);

        /// <summary>
        /// Save the PDF document to the specified location.
        /// </summary>
        /// <param name="stream">Stream to save the PDF document to.</param>
        void Save(Stream stream);

        /// <summary>
        /// Finds all occurences of text.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <param name="matchCase">Whether to match case.</param>
        /// <param name="wholeWord">Whether to match whole words only.</param>
        /// <param name="readBounds">Specify true to include the bounds of the matched string within the page.</param>
        /// <returns>All matches.</returns>
        PdfMatches Search(string text, bool matchCase, bool wholeWord, bool readBounds = false);

        /// <summary>
        /// Finds all occurences of text.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <param name="matchCase">Whether to match case.</param>
        /// <param name="wholeWord">Whether to match whole words only.</param>
        /// <param name="page">The page to search on.</param>
        /// <param name="readBounds">Specify true to include the bounds of the matched string within the page.</param>
        /// <returns>All matches.</returns>
        PdfMatches Search(string text, bool matchCase, bool wholeWord, int page, bool readBounds = false);

        /// <summary>
        /// Finds all occurences of text.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <param name="matchCase">Whether to match case.</param>
        /// <param name="wholeWord">Whether to match whole words only.</param>
        /// <param name="startPage">The page to start searching.</param>
        /// <param name="endPage">The page to end searching.</param>
        /// <param name="readBounds">Specify true to include the bounds of the matched string within the page.</param>
        /// <returns>All matches.</returns>
        PdfMatches Search(string text, bool matchCase, bool wholeWord, int startPage, int endPage, bool readBounds = false);

        ///// <summary>
        ///// Creates a <see cref="PrintDocument"/> for the PDF document.
        ///// </summary>
        ///// <returns></returns>
        //PrintDocument CreatePrintDocument();

        ///// <summary>
        ///// Creates a <see cref="PrintDocument"/> for the PDF document.
        ///// </summary>
        ///// <param name="printMode">Specifies the mode for printing. The default
        ///// value for this parameter is CutMargin.</param>
        ///// <returns></returns>
        //PrintDocument CreatePrintDocument(PdfPrintMode printMode);

        ///// <summary>
        ///// Creates a <see cref="PrintDocument"/> for the PDF document.
        ///// </summary>
        ///// <param name="settings">The settings used to configure the print document.</param>
        ///// <returns></returns>
        //PrintDocument CreatePrintDocument(PdfPrintSettings settings);

        /// <summary>
        /// Returns all links on the PDF page.
        /// </summary>
        /// <param name="page">The page to get the links for.</param>
        /// <param name="size">The size of the page.</param>
        /// <returns>A collection with the links on the page.</returns>
        PdfPageLinks GetPageLinks(int page, FloatSize size);

        /// <summary>
        /// Delete the page from the PDF document.
        /// </summary>
        /// <param name="page">The page to delete.</param>
        void DeletePage(int page);

        /// <summary>
        /// Rotate the page.
        /// </summary>
        /// <param name="page">The page to rotate.</param>
        /// <param name="rotation">How to rotate the page.</param>
        void RotatePage(int page, PdfRotation rotation);

        /// <summary>
        /// Get metadata information from the PDF document.
        /// </summary>
        /// <returns>The PDF metadata.</returns>
        PdfInformation GetInformation();

        /// <summary>
        /// Get all text on the page.
        /// </summary>
        /// <param name="page">The page to get the text for.</param>
        /// <returns>The text on the page.</returns>
        string GetPdfText(int page);

        /// <summary>
        /// Get all text matching the text span.
        /// </summary>
        /// <param name="textSpan">The span to get the text for.</param>
        /// <returns>The text matching the span.</returns>
        string GetPdfText(PdfTextSpan textSpan);

        /// <summary>
        /// Get all bounding rectangles for the text span.
        /// </summary>
        /// <description>
        /// The algorithm used to get the bounding rectangles tries to join
        /// adjacent character bounds into larger rectangles.
        /// </description>
        /// <param name="textSpan">The span to get the bounding rectangles for.</param>
        /// <returns>The bounding rectangles.</returns>
        IList<PdfRectangle> GetTextBounds(PdfTextSpan textSpan);

        ///// <summary>
        ///// Convert a point from device coordinates to page coordinates.
        ///// </summary>
        ///// <param name="page">The page number where the point is from.</param>
        ///// <param name="point">The point to convert.</param>
        ///// <returns>The converted point.</returns>
        //PointF PointToPdf(int page, Point point);

        ///// <summary>
        ///// Convert a point from page coordinates to device coordinates.
        ///// </summary>
        ///// <param name="page">The page number where the point is from.</param>
        ///// <param name="point">The point to convert.</param>
        ///// <returns>The converted point.</returns>
        //Point PointFromPdf(int page, PointF point);

        ///// <summary>
        ///// Convert a rectangle from device coordinates to page coordinates.
        ///// </summary>
        ///// <param name="page">The page where the rectangle is from.</param>
        ///// <param name="rect">The rectangle to convert.</param>
        ///// <returns>The converted rectangle.</returns>
        //RectangleF RectangleToPdf(int page, Rectangle rect);

        ///// <summary>
        ///// Convert a rectangle from page coordinates to device coordinates.
        ///// </summary>
        ///// <param name="page">The page where the rectangle is from.</param>
        ///// <param name="rect">The rectangle to convert.</param>
        ///// <returns>The converted rectangle.</returns>
        //Rectangle RectangleFromPdf(int page, RectangleF rect);
    }
}
