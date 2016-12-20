using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ShareVersionCtrl.XMLRelated
{
    class XmlEncoder
    {
        public XmlEncoder() { }
        public static void XmlOutput(FileAndFolderModel MainFolder, 
            List<VersionModel> Versions, String FileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Info></Info>");
            XmlElement root = doc.DocumentElement;
            XmlElement mainFolder = doc.CreateElement("MainFolder");
            root.AppendChild(mainFolder);
            MainFolder.AddNodeToFolder(mainFolder, doc);
            XmlElement allFileVersion = doc.CreateElement("AllFileVersion");
            root.AppendChild(allFileVersion);
            foreach (VersionModel x in Versions)
            {
                x.AddNodeTo(allFileVersion, doc);
            }

            XmlTextWriter tr = new XmlTextWriter(FileName, 
                Encoding.GetEncoding("utf-8"));
            doc.WriteContentTo(tr);
            tr.Close();
        }
    }
}
