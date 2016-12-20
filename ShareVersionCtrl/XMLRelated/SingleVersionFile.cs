using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareVersionCtrl.XMLRelated
{
    public class SingleVersionFile
    {
        public String FatherName;
        public String VersionName;
        public String Depict;
        public String Date;

        public override string ToString()
        {
            return "<" + VersionName + "," + Depict + "," 
                + Date + ">";
        }
    }
}
