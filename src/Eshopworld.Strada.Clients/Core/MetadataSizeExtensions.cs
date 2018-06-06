using System;

namespace Eshopworld.Strada.Clients.Core
{
    /// <summary>
    ///     MetadataSizeExtensions provide size-calculation functionality to serialised metadata.
    /// </summary>
    public static class MetadataSizeExtensions
    {
        /// <summary>
        ///     GetSizeInKilobytes returns the size of <see cref="serialisedMetadata" /> in Kilobytes.
        /// </summary>
        /// <param name="serialisedMetadata">The serialised metadata.</param>
        /// <returns>The size of <see cref="serialisedMetadata" />.</returns>
        /// <remarks>Rounds up to the nearest Kilobyte.</remarks>
        public static long GetSizeInKilobytes(this string serialisedMetadata)
        {
            var preciseDataSizeInKilobytes = serialisedMetadata.Length / 1024f;
            return (int) Math.Ceiling(preciseDataSizeInKilobytes);
        }
    }
}