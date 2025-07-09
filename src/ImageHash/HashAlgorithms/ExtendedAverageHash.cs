namespace CoenM.ImageHash.HashAlgorithms
{
    using System;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;

    /// <summary>
    /// Extended Average hash; Calculate a hash of an image based on visual characteristics by transforming the image to an nxn grayscale bitmap.
    /// Hash is based on each pixel compared to the bitmaps 3rd darkest, average and 3rd lightest grayscale values.Each pixel is representet as 3 bits.
    /// </summary>
    /// <remarks>
    /// Implementation based on David Oftedal's implementation of Average Hash. Algorithm specified by Dr. Neal Krawetz.
    /// See <see href="http://www.hackerfactor.com/blog/index.php?/archives/432-Looks-Like-It.html"/> for more information.
    /// </remarks>
    // ReSharper disable once StyleCop.SA1650
    public class ExtendedAverageHash : IImageHash
    {
        /// <inheritdoc />
        public byte[] Hash(Image<Rgba32> image, HashSizes hashsize = HashSizes.H64)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            int width = this.GetWidthOfHashSize(hashsize);
            int height = width;
            int nr_pixels = width * height;
            ulong most_significant_bits_mask = 1UL << (nr_pixels - 1);

            image.Mutate(ctx => ctx
                                .Resize(width, height)
                                .Grayscale(GrayscaleMode.Bt601)
                                .AutoOrient());
            byte[] hash = new byte[3 * nr_pixels / 8];

            image.ProcessPixelRows((imageAccessor) =>
                {
                    uint averageValue = 0;
                    uint darkValue = 0;
                    uint lightValue = 0;
                    byte min = 255;
                    byte max = 0;
                    for (var y = 0; y < height; y++)
                    {
                        Span<Rgba32> row = imageAccessor.GetRowSpan(y);
                        for (var x = 0; x < width; x++)
                        {
                            // We know 4 bytes (RGBA) are used to describe one pixel
                            // Also, it is already grayscaled, so R=G=B. Therefore, we can take one of these
                            // values for average calculation. We take the R (the first of each 4 bytes).
                            averageValue += row[x].R;
                            if (row[x].R < min)
                            {
                                min = row[x].R;
                            }

                            if (row[x].R > max)
                            {
                                max = row[x].R;
                            }
                        }
                    }

                    averageValue /= (uint)nr_pixels;
                    byte darkboundery = (byte)(min + ((byte)averageValue / 2));
                    int darkCount = 0;
                    int lightCount = 0;
                    byte lightboundery = (byte)(max - ((byte)averageValue / 2));
                    for (var y = 0; y < height; y++)
                    {
                        Span<Rgba32> row = imageAccessor.GetRowSpan(y);
                        for (var x = 0; x < width; x++)
                        {
                            if (row[x].R < darkboundery)
                            {
                                darkValue += row[x].R;
                                darkCount++;
                            }
                            else if (row[x].R > lightboundery)
                            {
                                lightValue += row[x].R;
                                lightCount++;
                            }
                        }
                    }

                    darkValue = darkCount != 0 ? darkValue / (uint)darkCount : 0;
                    lightValue = lightCount != 0 ? lightValue / (uint)lightCount : 255;

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

                            if (row[x].R <= darkValue)
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

                            if (row[x].R >= lightValue)
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
    }
}
