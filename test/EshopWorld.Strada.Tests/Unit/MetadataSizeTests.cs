using Eshopworld.Strada.Clients.Core;
using Xunit;

namespace EshopWorld.Strada.Tests.Unit
{
    public class MetadataSizeTests
    {
        /// <summary>
        ///     Ensures that the size of a file on disk is correctly calculated to the nearest Kilobyte.
        /// </summary>
        [Fact]
        public void CorrectSizeInKilobytesIsCalculated()
        {
            var sampleFile = Resources.SampleFile;
            var dataSizeInKilobytes = sampleFile.GetSizeInKilobytes();

            Assert.Equal(2, dataSizeInKilobytes);
        }
    }
}