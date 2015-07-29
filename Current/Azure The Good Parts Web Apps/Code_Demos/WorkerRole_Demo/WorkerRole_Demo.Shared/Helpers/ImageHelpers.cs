using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WorkerRole_Demo.Shared.Helpers
{
    public static class ImageHelpers
    {
        public static Stream ToStream(this Image image, ImageFormat formaw)
        {
            var stream = new MemoryStream();
            image.Save(stream, formaw);
            stream.Position = 0;
            return stream;
        }
    }
}