using Eshopworld.Strada.Clients.Core;
using Xunit;

namespace EshopWorld.Strada.Tests.Unit
{
    public class MetadataSizeTests
    {
        [Fact]
        public void CorrectSizeInKilobytesIsCalculated()
        {
            var sampleFile = Resources.SampleFile;
            var dataSizeInKilobytes = sampleFile.GetSizeInKilobytes();

            Assert.Equal(2, dataSizeInKilobytes);
        }
    }
}