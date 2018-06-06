using System.IO;
using Eshopworld.Strada.Clients.Core;
using Xunit;

namespace EshopWorld.Strada.Tests.Unit
{
    public class CompressionTests
    {
        /// <summary>
        ///     Ensures that a large file is compressed to within 256kB - the Azure metadata transmission size-limit.
        /// </summary>
        [Fact]
        public void LargeFileIsCompressedToLessThanAzureSizeLimit()
        {
            var file = File.ReadAllBytes("MOCK_DATA.json");
            var compressedFile = file.Compress();

            const int maxFileSize = 256000;
            var compressedFileSize = compressedFile.Length;

            Assert.True(compressedFileSize < maxFileSize);
        }
    }
}