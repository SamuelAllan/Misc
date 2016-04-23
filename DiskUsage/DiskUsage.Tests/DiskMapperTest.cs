using NUnit.Framework;

namespace DiskUsage.Tests
{
    [TestFixture]
    internal sealed class DiskMapperTest
    {
        [Test]
        public void MapEDrive()
        {
            var rootE = DiskMapper.Map(@"E:\");
            XmlWriter.WriteToXmlFile(rootE, 1024, @"E:\folderSizes.xml");
            Assert.That(rootE, Is.Not.Null);
        }
    }
}
