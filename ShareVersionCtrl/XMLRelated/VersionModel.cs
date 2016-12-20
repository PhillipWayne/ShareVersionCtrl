using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}