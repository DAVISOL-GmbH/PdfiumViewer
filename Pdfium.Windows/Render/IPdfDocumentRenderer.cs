using Davisol.Pdfium.Model.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davisol.Pdfium.Windows.Render
{
    public interface IPdfDocumentRenderer
    {
        IPdfDocument Document { get; }
    }
}
