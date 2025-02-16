using System.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
namespace Udemy.MultiThreading.Lecture3
{

    public class ImageProgram
    {
        public const string SOURCE_FILE = "./resources/many-flowers.jpg";
        public const string DESTINATION_FILE = "./out/many-flowers.jpg";
        public static void MajorAction()
        {
            using (Image<Rgba32> originImage = Image.Load<Rgba32>(SOURCE_FILE)) {
                Image<Rgba32> resultImage = originImage.CloneAs<Rgba32>();
                
                Stopwatch stopwatch = Stopwatch.StartNew();
                
                // RecolorImage(originImage, resultImage, 0,0, originImage.Width, originImage.Height);
                RecolorSingleThreaded(originImage, resultImage, 6);

                stopwatch.Stop();
                
                Console.WriteLine($"Operation took {stopwatch.ElapsedMilliseconds} milliseconds.");

                resultImage.SaveAsJpeg(DESTINATION_FILE);
            }
        }

        public static void RecolorSingleThreaded(Image<Rgba32> originalImage, Image<Rgba32> resultImage, int numberOfThreads) {
            List<Thread> threads = new List<Thread>();
            int width = originalImage.Width;
            int height = originalImage.Height / numberOfThreads;

            for(int i = 0 ; i < numberOfThreads; i++) {
                int threadMultipier = i;
                threads.Add(new Thread( () => {
                    int leftCorner = 0;
                    int topCorner = height * threadMultipier;

                    RecolorImage(originalImage, resultImage, leftCorner, topCorner, width, height);
                }));
            }
            threads.ForEach(T => T.Start());
            threads.ForEach(T => T.Join());
        }

        public static void RecolorImage(
            Image<Rgba32> originalImage, Image<Rgba32> resultImage, 
            int leftCorner, int rightCorner,
            int width, int height
        ) {
            for(int x = leftCorner; x < leftCorner + width && x < originalImage.Width; x++) {
                for(int y = rightCorner; y < rightCorner + height && y < originalImage.Height; y++) {
                    RecolorPixel(originalImage, resultImage, x, y);
                }
            }
        }
        
        public static void RecolorPixel(Image<Rgba32> originalImage, Image<Rgba32> resultImage, int x, int y) {
            Rgba32 rgba = originalImage[x,y];
            byte red = rgba.R;
            byte green = rgba.G;
            byte blue = rgba.B;
            
            Rgba32 newRgba;

            if(IsShadeOfGray(red, green, blue)) {
                newRgba = new Rgba32(
                    (byte)Math.Min(255, red + 10),
                    (byte)Math.Max(0, green - 80),
                    (byte)Math.Max(0, blue - 20)
                );
            }
            else {
                newRgba = rgba;
            }
            resultImage[x,y] = newRgba;
        }

        public static bool IsShadeOfGray(byte red, byte green, byte blue) {
            return  Math.Abs(red   - green) < 30 &&
                    Math.Abs(red   - blue ) < 30 &&
                    Math.Abs(green - blue ) < 30;
                    
        }
    }
}