namespace CoenM.ImageHash.HashAlgorithms
{
    using System;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;

    /// <summary>
    /// Average hash; Calculate a hash of an image based on visual characteristics by transforming the image to an 8x8 grayscale bitmap.
    /// Hash is based on each pixel compared to the bitmaps average grayscale.
    /// </summary>
    /// <remarks>
    /// Implementation based on David Oftedal's implementation of Average Hash. Algorithm specified by Dr. Neal Krawetz.
    /// See <see href="http://www.hackerfactor.com/blog/index.php?/archives/432-Looks-Like-It.html"/> for more information.
    /// </remarks>
    // ReSharper disable once StyleCop.SA1650
    public class AverageHash : IImageHash
    {
        /// <inheritdoc />
        public byte[] Hash(Image<Rgba32> image, HashSizes hashsize = HashSizes.H64)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            int width = GetWidthHeight(hashsize);
            int height = width;
            int nr_pixels = width * height;
            ulong most_significant_bits_mask = 1UL << (nr_pixels - 1);

            image.Mutate(ctx => ctx
                                .Resize(width, height)
                                .Grayscale(GrayscaleMode.Bt601)
                                .AutoOrient());
            byte[] hash = new byte[nr_pixels / 8];

            image.ProcessPixelRows((imageAccessor) =>
                {
                    uint averageValue = 0;
                    for (var y = 0; y < height; y++)
                    {
                        Span<Rgba32> row = imageAccessor.GetRowSpan(y);
                        for (var x = 0; x < width; x++)
                        {
                            // We know 4 bytes (RGBA) are used to describe one pixel
                            // Also, it is already grayscaled, so R=G=B. Therefore, we can take one of these
                            // values for average calculation. We take the R (the first of each 4 bytes).
                            averageValue += row[x].R;
                        }
                    }

                    averageValue /= (uint)nr_pixels;

                    // Compute the hash: each bit is a pixel
                    // 1 = higher than average, 0 = lower than average
                    int counter = 0;
                    int currentByte = 0;
                    int value = 0;
                    for (var y = 0; y < height; y++)
                    {
                        Span<Rgba32> row = imageAccessor.GetRowSpan(y);
                        for (var x = 0; x < width; x++)
                        {
                            if (row[x].R >= averageValue)
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
                        }
                    }
                });

            return hash;
        }

        private static int GetWidthHeight(HashSizes size)
        {
            return (int)size;
        }
    }
}
