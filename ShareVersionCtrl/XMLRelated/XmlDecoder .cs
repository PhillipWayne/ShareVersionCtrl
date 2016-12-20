using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace ShareVersionCtrl.XMLRelated
{
    public class XmlDecoder
    {
        public FileAndFolderModel MainFolder;
        public List<VersionModel> Versions;
        XmlDocument doc;

        public XmlDecoder(String sourceFile)
        {
            MainFolder = new FileAndFolderModel("Folder");
            Versions = new List<VersionModel>();
            //MessageBox.Show("XmlDecorder!");
            doc = new XmlDocument();
            doc.Load(sourceFile);
            //解析目录结构
            XmlNode fatherxmlNode = doc.SelectSingleNode("Info");
            XmlNode xmlNode1 = fatherxmlNode.ChildNodes.Item(0);
            XmlNodeList xmlNodeList = xmlNode1.ChildNodes;
            LoopInFolfer(MainFolder, xmlNodeList);
            //解析版本列表
            XmlNode xmlNode2 = fatherxmlNode.ChildNodes.Item(1);
            XmlNodeList xmlNodeList2 = xmlNode2.ChildNodes;
            foreach (XmlNode xn in xmlNodeList2)
            {
                XmlElement xe = (XmlElement)xn;
                VersionModel versionModel = new VersionModel(
                    xe.GetAttribute("FileName").ToString());
                XmlNodeList xnl = xe.ChildNodes;
                foreach (XmlNode x in xnl)
                {
                    versionModel.AddSingleVersion((XmlElement)x);
                }
                Versions.Add(versionModel);
            }
        }
        
        private void LoopInFolfer(FileAndFolderModel Folder, XmlNodeList xmlNodeList)
        {
            foreach (XmlNode xn in xmlNodeList)
            {
                XmlElement xe = (XmlElement)xn;
                //MessageBox.Show(xe.GetAttribute("Type").ToString());
                FileAndFolderModel ffchild = new FileAndFolderModel(
                    xe.GetAttribute("Type").ToString());
                switch (ffchild.Type)
                {
                    case FileAndFolderModel.Type_File:
                        ffchild.SetFile(xe.GetAttribute("FileName").ToString(),
                            xe.GetAttribute("VersionName").ToString());
                        Folder.AddToFolder(ffchild);
                        break;
                    case FileAndFolderModel.Type_Folder:
                        XmlNodeList xnl = xe.ChildNodes;
                        LoopInFolfer(ffchild, xnl);
                        ffchild.SetFolder(xe.GetAttribute("FolderName"));
                        Folder.AddToFolder(ffchild);
                        break;
                }
            }
        }
    }
}
