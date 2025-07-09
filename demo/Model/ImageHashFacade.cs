namespace Demo.Model
{
    using System;
    using CoenM.ImageHash;
    using CoenM.ImageHash.HashAlgorithms;

    public class ImageHashFacade : IDemoImageHash
    {
        private readonly IFileSystem _fileSystem;
        private readonly AverageHash _averageHash;
        private readonly DifferenceHash _differenceHash;
        private readonly PerceptualHash _perceptualHash;
        private readonly ExtendedAverageHash _extendedAverageHash;


        public ImageHashFacade(IFileSystem fileSystem)
        {
            this._fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _averageHash = new AverageHash();
            _differenceHash = new DifferenceHash();
            _perceptualHash = new PerceptualHash();
            _extendedAverageHash = new ExtendedAverageHash();
        }

        public byte[] CalculateAverageHash(string filename) => CoenM.ImageHash.ImageHashExtensions.Hash(_averageHash, _fileSystem.OpenRead(filename),HashSizes.H256);

        public byte[] CalculateExtendedAverageHash(string filename) => CoenM.ImageHash.ImageHashExtensions.Hash(_extendedAverageHash, _fileSystem.OpenRead(filename), HashSizes.H256);

        public byte[] CalculateDifferenceHash(string filename) => CoenM.ImageHash.ImageHashExtensions.Hash(_differenceHash, _fileSystem.OpenRead(filename), HashSizes.H256);

        public byte[] CalculatePerceptualHash(string filename) => CoenM.ImageHash.ImageHashExtensions.Hash(_perceptualHash, _fileSystem.OpenRead(filename), HashSizes.H256);
    }
}
