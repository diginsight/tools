using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MipDocumentInspector
{
    public class FileHelper
    {
        public static string GetExtension(string documentPath)
        {
            var extension = Path.GetExtension(documentPath);
            return extension;
        }
    }
}
