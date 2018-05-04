using System.IO;
using Eshopworld.Strada.Clients.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EshopWorld.Strada.Tests
{
    [TestClass]
    public class DataSegmentorTests
    {
        [TestMethod]
        [DeploymentItem(@"JsonTest.json")]
        public void CorrectSizeInKilobytesIsCalculated()
        {
            // var sampleFile = File.ReadAllText(@"JsonTest.json");
            var sampleFile = Resources.SampleFile;
            var fileSizeInKilobytes = DataSegmentor.CalculateSizeInKilobytes(sampleFile);

            Assert.AreEqual(1.544921875, fileSizeInKilobytes);
        }
    }
}