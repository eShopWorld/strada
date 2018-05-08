using Eshopworld.Strada.Clients.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EshopWorld.Strada.Tests
{
    [TestClass]
    public class DataSegmentorTests
    {
        [TestMethod]        
        public void CorrectSizeInKilobytesIsCalculated()
        {
            var sampleFile = Resources.SampleFile;
            var dataSizeInKilobytes = DataSegmentor.CalculateDataSizeInKilobytes(sampleFile);

            Assert.AreEqual(2, dataSizeInKilobytes);
        }
    }
}