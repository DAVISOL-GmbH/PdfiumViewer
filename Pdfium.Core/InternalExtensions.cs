using System;
using System.Collections.Generic;
using System.Text;

namespace Davisol.Pdfium
{
    internal static class InternalExtensions
    {

        // System.String.GetHashCode(): http://referencesource.microsoft.com/#mscorlib/system/string.cs,0a17bbac4851d0d4
        // System.Web.Util.StringUtil.GetStringHashCode(System.String): http://referencesource.microsoft.com/#System.Web/Util/StringUtil.cs,c97063570b4e791a
        internal static int CombineHashCodes(params int[] hashCodes)
        {
            if (hashCodes == null)
                return 0;

            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            int i = 0;
            foreach (var hashCode in hashCodes)
            {
                if (i % 2 == 0)
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ hashCode;
                else
                    hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ hashCode;

                ++i;
            }

            return hash1 + (hash2 * 1566083941);
        }


    }
}
