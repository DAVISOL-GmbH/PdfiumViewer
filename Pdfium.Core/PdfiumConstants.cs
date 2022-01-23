using System;
using System.Collections.Generic;
using System.Text;

namespace Davisol.Pdfium
{
    public static class PdfiumConstants
    {
        // Interned strings are cached over AppDomains. This means that when we
        // lock on this string, we actually lock over AppDomain's. The Pdfium
        // library is not thread safe, and this way of locking
        // guarantees that we don't access the Pdfium library from different
        // threads, even when there are multiple AppDomain's in play.
        public static readonly string NativeThreadLockKey = string.Intern("e362349b-001d-4cb2-bf55-a71606a3e36f");

    }
}
