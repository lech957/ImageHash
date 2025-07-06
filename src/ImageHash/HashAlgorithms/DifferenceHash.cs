namespace CoenM.ImageHash.HashAlgorithms
{
    using System;
    using System.Reflection;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;

    /// <summary>
    /// Difference hash; Calculate a hash of an image based on visual characteristics by transforming the image to an 9x8 grayscale bitmap.
    /// Hash is based on each pixel compared to it's right neighbor pixel.
    /// </summary>
    /// <remarks>
    /// Algorithm specified by David Oftedal and slightly adjusted by Dr. Neal Krawetz.
    /// See <see href="http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html"/> for more information.
    /// </remarks>
    // ReSharper disable once StyleCop.SA1650
    public class DifferenceHash : IImageHash
    {
         /// <inheritdoc />
        public byte[] Hash(Image<Rgba32> image, HashSizes hashsize = HashSizes.H64)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            int width = this.GetWidthOfHashSize(hashsize) + 1;
            int height = this.GetWidthOfHashSize(hashsize);
            int nr_pixels = width * height;
            ulong most_significant_bits_mask = 1UL << (nr_pixels - 1);

            // We first auto orient because with and height differ.
            image.Mutate(ctx => ctx
                                .AutoOrient()
                                .Resize(width, height)
                                .Grayscale(GrayscaleMode.Bt601));

            var hash = new byte[nr_pixels / 8];

            image.ProcessPixelRows((imageAccessor) =>
                {
                    int counter = 0;
                    int value = 0;
                    int currentByte = 0;
                    for (var y = 0; y < height; y++)
                    {
                        Span<Rgba32> row = imageAccessor.GetRowSpan(y);
                        Rgba32 leftPixel = row[0];

                        for (var index = 1; index < width; index++)
                        {
                            Rgba32 rightPixel = row[index];
                            if (leftPixel.R < rightPixel.R)
                            {
                                value |= 1 << counter;
                            }

                            counter++;
                            if (counter == 8)
                            {
                                hash[currentByte] = (byte)value;
                                counter = 0;
                                currentByte++;
                                value = 0;
                            }

                            leftPixel = rightPixel;
                        }
                    }
                });

            return hash;
        }
    }
}
