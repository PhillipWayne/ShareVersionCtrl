using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;

namespace ShareVersionCtrl.XMLRelated
{
    public class VersionModel
    {
        public String FileName;
        public List<SingleVersionFile> versionList;
        public VersionModel(String FileName_)
        {
            FileName = FileName_;
            versionList = new List<SingleVersionFile>();
        }
        public void AddSingleVersion(XmlElement x)
        {
            SingleVersionFile svf = new SingleVersionFile();
            svf.FatherName = FileName;
            svf.VersionName = x.GetAttribute("VersionName").ToString();
            svf.Depict = x.GetAttribute("Depict").ToString();
            svf.Date = x.GetAttribute("Date").ToString();
            versionList.Add(svf);
        }
        public override bool Equals(object obj)
        {
            if (obj is String)
            {
                return FileName.Equals((String)obj);
            }
            if (obj is VersionModel)
            {
                return FileName.Equals(((VersionModel)obj).FileName);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public String GetTreeInfo()
        {
            String str = FileName;
            foreach (SingleVersionFile x in versionList)
            {
                str += "\r\n    " + x.ToString();
            }
            return str;
        }
        public void AddNodeTo(XmlElement root, XmlDocument doc)
        {
            XmlElement node = doc.CreateElement("Version");
            node.SetAttribute("FileName", FileName);
            root.AppendChild(node);
            foreach (SingleVersionFile svf in versionList)
            {
                XmlElement v = doc.CreateElement("SingleVersion");
                node.AppendChild(v);
                v.SetAttribute("VersionName", svf.VersionName);
                v.SetAttribute("Depict", svf.Depict);
                v.SetAttribute("Date", svf.Date);
            }
        }
        public static String ShowAllVersion(List<VersionModel> Versions)
        {
            String str = "Versions:";
            foreach (VersionModel x in Versions)
            {
                str += "\r\n" + x.GetTreeInfo();
            }
            return str;
        }
        public void ShowInTree(TreeView treeView, 
            List<KeyValuePair<TreeViewItem, SingleVersionFile>> treeSingleVersionMap, 
            List<KeyValuePair<TreeViewItem, VersionModel>> treeVersionMap)
        {
            TreeViewItem tvi = new TreeViewItem();
            tvi.Header = FileName;
            treeView.Items.Add(tvi);
            treeVersionMap.Add(new KeyValuePair<TreeViewItem, VersionModel>(
                tvi, this));
            foreach (SingleVersionFile svf in versionList)
            {
                TreeViewItem x = new TreeViewItem();
                x.Header = svf.VersionName + "(" + svf.Depict + 
                    "," + svf.Date + ")";
                tvi.Items.Add(x);
                treeSingleVersionMap.Add(new KeyValuePair<TreeViewItem, SingleVersionFile>
                    (x, svf));
            }
        }
    }
}