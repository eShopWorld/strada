using System.IO;
using System.IO.Compression;

namespace Eshopworld.Strada.Clients.Core
{
    /// <summary>
    ///     CompressionExtensions provide compression and decompression features
    ///     for byte array inputs.
    /// </summary>
    public static class CompressionExtensions
    {
        /// <summary>
        ///     Compress compresses a byte array, in GZip format, to allow large
        ///     metadata transmission in Azure, whose transmission size-limit is
        ///     256kB at time of writing.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns>A compressed byte array representing the original input.</returns>
        /// <remarks>
        ///     Compresses metadata streams, up to 2.47MB in size,
        ///     to within the Azure metadata transmission size-limit.
        /// </remarks>
        public static byte[] Compress(this byte[] raw)
        {
            using (var memory = new MemoryStream())
            {
                using (var gzip = new GZipStream(memory,
                    CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }

                return memory.ToArray();
            }
        }

        /// <summary>
        ///     Decompress decompresses a byte array to its original size and format.
        /// </summary>
        /// <param name="gzip"></param>
        /// <returns>
        ///     A decompressed byte array that represents the original compressed
        ///     byte array.
        /// </returns>
        private static byte[] Decompress(this byte[] gzip)
        {
            using (var stream = new GZipStream(new MemoryStream(gzip),
                CompressionMode.Decompress))
            {
                const int size = 4096;
                var buffer = new byte[size];
                using (var memory = new MemoryStream())
                {
                    var count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0) memory.Write(buffer, 0, count);
                    } while (count > 0);

                    return memory.ToArray();
                }
            }
        }
    }
}