using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generative_raster_filters
{
    class Helpers
    {
        public static string ShortFile(string filePath, int maxLength)
        {
            return filePath.Length > maxLength + 3 ? "..." + filePath.Substring(filePath.Length - maxLength - 3) : filePath;
        }
    }
}
