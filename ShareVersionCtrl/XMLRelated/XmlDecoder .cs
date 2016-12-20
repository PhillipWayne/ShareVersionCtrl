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
        XmlDocument doc;

        public XmlDecoder()
        {
            MainFolder = new FileAndFolderModel("Folder");
            //MessageBox.Show("XmlDecorder!");
            doc = new XmlDocument();
            doc.Load(@"record.xml");
            XmlNode xmlNode = doc.SelectSingleNode("MainFolder");
            XmlNodeList xmlNodeList = xmlNode.ChildNodes;
            LoopInFolfer(MainFolder, xmlNodeList);
        }
        
        private void LoopInFolfer(FileAndFolderModel Folder, XmlNodeList xmlNodeList)
        {
            foreach (XmlNode xn in xmlNodeList)
            {
                XmlElement xe = (XmlElement)xn;
                //MessageBox.Show(xe.GetAttribute("Type").ToString());
                FileAndFolderModel ffchild = new FileAndFolderModel(
                    xe.GetAttribute("Type").ToString());
                XmlNodeList xnl = xe.ChildNodes;
                switch (ffchild.Type)
                {
                    case FileAndFolderModel.Type_File:
                        ffchild.SetFile(xnl.Item(0).InnerText, 
                            xnl.Item(1).InnerText);
                        Folder.AddToFolder(ffchild);
                        break;
                    case FileAndFolderModel.Type_Folder:
                        LoopInFolfer(ffchild, xnl);
                        Folder.AddToFolder(ffchild);
                        break;
                }
            }
        }
    }
}
