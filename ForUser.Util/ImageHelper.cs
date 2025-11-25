using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Util
{
    public static class ImageHelper
    {
        public static string GetImageExtension(string contentType)
        {
            return contentType switch
            {
                "image/png" => ".png",
                "image/jpeg" => ".jpg",
                "image/gif" => ".gif",
                "image/bmp" => ".bmp",
                "image/tiff" => ".tiff",
                _ => ".bin"
            };
        }
    }
}
