﻿using Davisol.Pdfium.Model.Content;
using Davisol.Pdfium.Model.Shape;
using System;
using System.Collections.Generic;
using System.Text;

namespace Davisol.Pdfium.Model.Link
{
    /// <summary>
    /// Describes a link on a page.
    /// </summary>
    public class PdfPageLink
    {
        /// <summary>
        /// The location of the link.
        /// </summary>
        public FloatRect Bounds { get; private set; }

        /// <summary>
        /// The target of the link.
        /// </summary>
        public int? TargetPage { get; private set; }

        /// <summary>
        /// The target URI of the link.
        /// </summary>
        public string Uri { get; private set; }

        /// <summary>
        /// Creates a new instance of the PdfPageLink class.
        /// </summary>
        /// <param name="bounds">The location of the link</param>
        /// <param name="targetPage">The target page of the link</param>
        /// <param name="uri">The target URI of the link</param>
        public PdfPageLink(FloatRect bounds, int? targetPage, string uri)
        {
            Bounds = bounds;
            TargetPage = targetPage;
            Uri = uri;
        }
    }
}
