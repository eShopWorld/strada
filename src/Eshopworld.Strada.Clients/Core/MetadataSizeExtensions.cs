using System;

namespace Eshopworld.Strada.Clients.Core
{
    public static class MetadataSizeExtensions
    {
        public static long GetSizeInKilobytes(this string serialisedMetadata)
        {
            var preciseDataSizeInKilobytes = serialisedMetadata.Length / 1024f;
            return (int) Math.Ceiling(preciseDataSizeInKilobytes);
        }
    }
}