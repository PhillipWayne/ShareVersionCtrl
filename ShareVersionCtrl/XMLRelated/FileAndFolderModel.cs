using ShareVersionCtrl.MyMessageBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;

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
        public String FolderName;
        public TreeViewItem myTvi;

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

        public void SetFolder(String FolderName_)
        {
            if (Type == Type_Folder)
            {
                FolderName = FolderName_;
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

        public void AddNodeTo(XmlElement root, XmlDocument doc)
        {
            switch (Type)
            {
                case Type_File:
                    XmlElement file = doc.CreateElement("Child");
                    file.SetAttribute("Type", "File");
                    file.SetAttribute("FileName", FileName);
                    file.SetAttribute("VersionName", VersionName);
                    root.AppendChild(file);
                    break;
                case Type_Folder:
                    XmlElement folder = doc.CreateElement("Child");
                    folder.SetAttribute("Type", "Folder");
                    folder.SetAttribute("FolderName", FolderName);
                    root.AppendChild(folder);
                    AddNodeToFolder(folder, doc);
                    break;
            }
        }
        public void AddNodeToFolder(XmlElement folder, XmlDocument doc)
        {
            foreach (FileAndFolderModel child in Children)
            {
                child.AddNodeTo(folder, doc);
            }
        }

        public void ShowInTreeView(TreeViewItem treeViewItem, List<KeyValuePair<TreeViewItem, FileAndFolderModel>> treeFolderMap)
        {
            switch (Type)
            {
                case Type_File:
                    myTvi = new TreeViewItem();
                    myTvi.Header = FileName + "(" + VersionName + ")";
                    treeViewItem.Items.Add(myTvi);
                    treeFolderMap.Add(new KeyValuePair<TreeViewItem, FileAndFolderModel>(
                        myTvi, this));
                    break;
                case Type_Folder:
                    myTvi = new TreeViewItem();
                    myTvi.Header = FolderName;
                    treeViewItem.Items.Add(myTvi);
                    treeFolderMap.Add(new KeyValuePair<TreeViewItem, FileAndFolderModel>(
                        myTvi, this));
                    ShowInTreeFolder(myTvi, treeFolderMap);
                    break;
            }
        }
        public void ShowInTreeFolder(TreeViewItem treeViewItem, List<KeyValuePair<TreeViewItem, FileAndFolderModel>> treeFolderMap)
        {
            foreach (FileAndFolderModel child in Children)
            {
                child.ShowInTreeView(treeViewItem, treeFolderMap);
            }
        }
        public bool AskDeleteAllRef(String tarFileName, String tarVersionName, 
            String FatherPath)
        {
            switch (Type)
            {
                case Type_File:
                    if (FileName.Equals(tarFileName))
                    {
                        if (tarVersionName == null) //忽略版本信息
                        {
                            if (NoAskNsg.Show("确认删除：" + FatherPath + FileName,
                                "删除确认") == false) return false;
                        }
                        else
                        {
                            if (VersionName.Equals(tarVersionName))
                            {
                                if (NoAskNsg.Show("确认删除：" + FatherPath + FileName,
                                "删除确认") == false) return false;
                            }
                        }
                    }
                    break;
                case Type_Folder:
                    foreach (FileAndFolderModel child in Children)
                    {
                        if (child.AskDeleteAllRef(tarFileName, tarVersionName,
                            FatherPath + FolderName + "/") == false) return false;
                    }
                    break;
            }
            return true;
        }
        public bool CommitDeleteRef(String tarFileName, String tarVersionName)
        {
            switch (Type)
            {
                case Type_File:
                    if (FileName.Equals(tarFileName))
                    {
                        if (tarVersionName == null) //忽略版本信息
                        {
                            return true;
                        }
                        else if (VersionName.Equals(tarVersionName)) return true;
                    }
                    break;
                case Type_Folder:
                    for (int i=0; i<Children.Count; i++)
                    {
                        if (Children[i].CommitDeleteRef(tarFileName, tarVersionName)
                            == true)
                        {
                            //删除这一级的child
                            Children.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
            }
            return false; //不删除
        }
        public bool TryNewFolder(String newFolderName)
        {
            if (Type != Type_Folder) return false;
            foreach (FileAndFolderModel child in Children)
            {
                if (child.Type == Type_Folder &&
                    child.FolderName.Equals(newFolderName)) return false;
            }
            //可以添加且不冲突
            FileAndFolderModel ffm = new FileAndFolderModel("Folder");
            ffm.FolderName = newFolderName;
            Children.Add(ffm);
            return true;
        }
    }
}
