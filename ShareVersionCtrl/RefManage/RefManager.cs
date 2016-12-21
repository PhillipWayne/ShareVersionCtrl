using ShareVersionCtrl.XMLRelated;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareVersionCtrl.RefManage
{
    public class RefManager
    {
        public const String RefFolder = "RefFiles";

        public RefManager()
        {
            if (! Directory.Exists(RefFolder))
            {
                Directory.CreateDirectory(RefFolder);
            }
        }

        public bool CreateRef(String RefName)
        {
            //如果返回false说明文件夹已经存在
            if (Directory.Exists(RefFolder + "/" + RefName)) return false;
            Directory.CreateDirectory(RefFolder + "/" + RefName);
            return true;
        }

        public bool DeleteRef(String RefName)
        {
            //返回false说明文件夹不存在
            if (! Directory.Exists(RefFolder + "/" + RefName)) return false;
            Directory.Delete(RefFolder + "/" + RefName, true); //彻底删除非空文件夹
            return true;
        }

        public bool DeleteVersion(String RefName, String VersionName)
        {
            if (!File.Exists(RefFolder + "/" + RefName + "/" + VersionName)) return false;
            File.Delete(RefFolder + "/" + RefName + "/" + VersionName);
            return true;
        }

        public void CopyVersion(String source, SingleVersionFile svf)
        {
            File.Copy(source, Environment.CurrentDirectory + "\\" +
                RefFolder + "\\" + svf.FatherName + "\\" + svf.VersionName);
        }
    }
}
