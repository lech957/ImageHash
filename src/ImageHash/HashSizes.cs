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
        /// 64 Bit -> 8 byte hash
        /// </summary>
        H64 = 8,

        /// <summary>
        /// 128 Bit -> 16 byte hash
        /// </summary>
        H128 = 16,

        /// <summary>
        /// 256 Bit -> 32 byte hash
        /// </summary>
        H256 = 32,

        /// <summary>
        /// 512 Bit -> 64 byte hash
        /// </summary>
        H512 = 64,
    }
}
