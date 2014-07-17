using System;
using System.Drawing.Imaging;

namespace Se7enRedLines.UI
{
    public static class ImageFormatHelper
    {
        public static string GetExtension(ImageFormat imageFormat)
        {
            if (imageFormat.Equals(ImageFormat.Bmp))
                return ".bmp";
            if (imageFormat.Equals(ImageFormat.Gif))
                return ".gif";
            if (imageFormat.Equals(ImageFormat.Jpeg))
                return ".jpg";
            if (imageFormat.Equals(ImageFormat.Png))
                return ".png";

            throw new NotSupportedException("Unsupported image format: " + imageFormat);
        }

        public static ImageFormat GetImageFormat(string extension)
        {
            switch (extension)
            {
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".gif":
                    return ImageFormat.Gif;
                case ".jpg":
                    return ImageFormat.Jpeg;
                case ".png":
                    return ImageFormat.Png;
            }

            throw new NotSupportedException("Unsupported extension:" + extension);
        }
    }
}