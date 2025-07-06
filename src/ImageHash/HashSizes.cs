namespace CoenM.ImageHash
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Enum for different hash sizes.
    /// </summary>
    public enum HashSizes : int
    {
        /// <summary>
        /// 64 Bit -> 8*8 pixels = 8 byte hash
        /// </summary>
        H64 = 8,

        /// <summary>
        /// 256 Bit -> 16x16 pixels byte = 32 byte hash
        /// </summary>
        H256 = 16,

        /// <summary>
        /// 400 Bit -> 20x20 pixels byte = 50 byte hash
        /// </summary>
        H400 = 20,

        /// <summary>
        /// 1024 Bit -> 32*32 pixels = 128 byte hash
        /// </summary>
        H1024 = 32,
    }
}
