using System.IO;
using ImageResizer;

namespace WorkerRole_Demo.WorkerRole
{
    public class ImageManipulator
    {
        public byte[] ResizeImage(byte[] image, int width)
        {
            return ProcessImage(image, new Instructions { Width = width });
        }

        private static byte[] ProcessImage(byte[] image, Instructions instructions)
        {
            using (var outputImage = new MemoryStream())
            {
                using (var inputImage = new MemoryStream(image))
                {
                    var job = new ImageJob(inputImage, outputImage, instructions);
                    job.Build();
                    return outputImage.ToArray();
                }
            }
        }
    }
}