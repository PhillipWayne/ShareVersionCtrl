using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareVersionCtrl.XMLRelated
{
    public class FileAndFolderModel
    {
        public List<FileAndFolderModel> Children;
        public const int Type_NULL = 0;
        public const int Type_File = 1;
        public const int Type_Folder = 2;
        public int Type = Type_NULL;
        public String FileName;
        public String VersionName;

        public FileAndFolderModel(String Type_)
        {
            if (Type_.Equals("File"))
            {
                Type = Type_File;
            }
            else if (Type_.Equals("Folder"))
            {
                Type = Type_Folder;
            }
            switch (Type)
            {
                case Type_File:
                    Children = null;
                    break;
                case Type_Folder:
                    Children = new List<FileAndFolderModel>();
                    break;
            }
        }

        public void SetFile(String FileName_, String VersionName_)
        {
            if (Type == Type_File)
            {
                FileName = FileName_;
                VersionName = VersionName_;
            }
        }

        public void AddToFolder(FileAndFolderModel child)
        {
            if (Type == Type_Folder)
            {
                Children.Add(child);
            }
        }

        public String GetTreeInfo(int Step)
        {
            switch (Type)
            {
                case Type_File:
                    return "<" + FileName + "," + VersionName + ">";
                case Type_Folder:
                    String str = "(";
                    foreach (FileAndFolderModel child in Children)
                    {
                        str += "\r\n";
                        for (int i = -1; i < Step; i++) str += "    ";
                        str += child.GetTreeInfo(Step + 1);
                    }
                    str += "\r\n";
                    for (int i = 0; i < Step; i++) str += "    ";
                    return str + ")";
            }
            return "%BUG%";
        }
        
    }
}
