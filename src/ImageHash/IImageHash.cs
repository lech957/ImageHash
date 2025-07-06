namespace CoenM.ImageHash
{
    using System;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;

    /// <summary>
    /// Interface for perceptual image hashing algorithm.
    /// </summary>
    public interface IImageHash
    {
        /// <summary>Hash the image using the algorithm.</summary>
        /// <param name="image">image to calculate hash from.</param>
        /// <param name="hashsize">Hash size to calculate. Use H64 for compatibility. The higher the hash size , the slower the calculation, but the the more precise the result.</param>
        /// <returns>hash value of the image.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="image"/> is <c>null</c>.</exception>
        byte[] Hash(Image<Rgba32> image, HashSizes hashsize = HashSizes.H64);
    }
}
