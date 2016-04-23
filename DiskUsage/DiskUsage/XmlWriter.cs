using System;
using System.Text;
using System.Xml;

namespace DiskUsage
{
    public sealed class XmlWriter : IDisposable
    {
        public XmlWriter(XmlTextWriter writer)
        {
            this.writer = writer;
        }

        public static void WriteToXmlFile(Folder folder, long thresholdMb, string filepath)
        {
            var xmlWriter = new XmlTextWriter(filepath, Encoding.UTF8);
            using (var writer = new XmlWriter(xmlWriter))
            {
                writer.WriteToXml(folder, thresholdMb);
            }
        }

        public void WriteToXml(Folder folder, long thresholdMb)
        {
            var total = folder.TotalSizeInBytes/1024/1024;
            var files = folder.FileSizeInBytes/1024/1024;
            var children = folder.ChildSizeInBytes/1024/1024;
            writer.WriteStartElement("folder");
            writer.WriteAttributeString("name", folder.Name);
            writer.WriteAttributeString("totalMB", total.ToString());
            writer.WriteAttributeString("filesMB", files.ToString());
            writer.WriteAttributeString("childrenMB", children.ToString());

            foreach (var child in folder.Children)
            {
                if (child.TotalSizeInBytes/1024/1024 < thresholdMb)
                {
                    continue;
                }
                WriteToXml(child, thresholdMb);
            }
            
            writer.WriteEndElement();
        }

        public void Dispose()
        {
            writer.Dispose();
        }

        private readonly XmlTextWriter writer;
    }
}
