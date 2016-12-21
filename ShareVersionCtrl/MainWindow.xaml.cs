using ShareVersionCtrl.MyMessageBox;
using ShareVersionCtrl.RefManage;
using ShareVersionCtrl.XMLRelated;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Win32;

namespace ShareVersionCtrl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 本软件用于管理社团共享文件的版本问题，比如说活动的PPT，
    ///     每次应当放入最新的PPT，但是旧版本也要保留，于是需要外部一个软件控制。
    /// 再比如缓存的软件，当软件有更新的时候，旧版本保留在硬盘里，但是给用户看到的
    ///     应当是最新版本，只有当需要用到旧版本的时候才给用户传送。
    /// 这个软件的数据使用XML存储，分为两部分，一部分存储文件结构，另一部分存储版本信息。
    /// 实现原理是把所有源文件和本程序同时放到一个私有的盘里，在此程序内设置一个文件夹
    ///     用于输出结构化的文件，同时在程序内查看结构化的目录信息，并管理版本和新增
    ///     文件等等。
    /// 本程序将实现“一键最新”，也就是所有可以方便地把文件加载到根目录下后，可以通过
    ///     一个按钮自动找到所有存放此文件旧版本的目录下，把软件的版本更新到最新。
    /// </summary>
    public partial class MainWindow : Window
    {
        FileAndFolderModel MainFolder;
        List<VersionModel> Versions;
        List<KeyValuePair<TreeViewItem, FileAndFolderModel>> treeFolderMap;
        List<KeyValuePair<TreeViewItem, SingleVersionFile>> treeSingleVersionMap;
        List<KeyValuePair<TreeViewItem, VersionModel>> treeVersionMap;
        FileAndFolderModel foundFileOrFolder = null;
        SingleVersionFile foundSingleVersionFile = null;
        VersionModel foundVersionModel = null;
        public String targetFolder = "../共享";
        private bool ModifyPer = false;
        RefManager refManager;

        public MainWindow()
        {
            InitializeComponent();
            treeFolderMap = new List<KeyValuePair<TreeViewItem, FileAndFolderModel>>();
            treeSingleVersionMap = new List<KeyValuePair<TreeViewItem, SingleVersionFile>>();
            treeVersionMap = new List<KeyValuePair<TreeViewItem, VersionModel>>();
            if (File.Exists("record.xml"))
            {
                XmlDecoder xmlDecoder = new XmlDecoder("record.xml");
                MainFolder = xmlDecoder.MainFolder;
                Versions = xmlDecoder.Versions;
            }
            else
            {
                MainFolder = new FileAndFolderModel("Folder");
                Versions = new List<VersionModel>();
            }
            refManager = new RefManager();
            newFile.IsEnabled = false;
            refreshFile.IsEnabled = false;
            deleteFile.IsEnabled = false;
            newFolder.IsEnabled = false;
            renameFolder.IsEnabled = false;
            deleteFolder.IsEnabled = false;
            createVerion.IsEnabled = false;
            deleteVersion.IsEnabled = false;
            newRef.IsEnabled = false;
            deleteRef.IsEnabled = false;
            //MessageBox.Show(MainFolder.GetTreeInfo(0));
            //MessageBox.Show(VersionModel.ShowAllVersion(Versions));
            ShowFolderTree();
            ShowVersionTree();
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
            treeViewVersion.SelectedItemChanged += TreeViewVersion_SelectedItemChanged;
            /*Directory.CreateDirectory(targetFolder);
            MessageBox.Show("" + Directory.Exists(targetFolder));
            Directory.Delete(targetFolder, true); //不能删除非空文件夹
            MessageBox.Show("" + Directory.Exists(targetFolder));*/
            this.KeyDown += MainWindow_KeyDown;
            newRef.Click += NewRef_Click;
            deleteRef.Click += DeleteRef_Click;
            createVerion.Click += CreateVerion_Click;
            deleteVersion.Click += DeleteVersion_Click;
        }

        private void DeleteVersion_Click(object sender, RoutedEventArgs e)
        {
            if (foundSingleVersionFile == null)
            {
                MessageBox.Show("软件错误，请联系管理员");
                return;
            }
            VersionModel father = null;
            for (int i=0; i< treeVersionMap.Count; i++)
            {
                if (treeVersionMap[i].Value.FileName.Equals(
                    foundSingleVersionFile.FatherName))
                {
                    father = treeVersionMap[i].Value;
                    break;
                }
            }
            if (father == null)
            {
                MessageBox.Show("软件错误，请联系管理员");
                return;
            }
            if (MainFolder.AskDeleteAllRef(father.FileName, foundSingleVersionFile.VersionName,
                "") == false) return;
            MainFolder.CommitDeleteRef(father.FileName, foundSingleVersionFile.VersionName);
            father.versionList.Remove(foundSingleVersionFile);
            refManager.DeleteVersion(father.FileName, foundSingleVersionFile.VersionName);
            ShowFolderTree();
            ShowVersionTree();
        }

        private void CreateVerion_Click(object sender, RoutedEventArgs e)
        {
            if (foundVersionModel == null)
            {
                MessageBox.Show("软件错误，请联系管理员");
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();
            if (result == false) return;
            //MessageBox.Show(openFileDialog.FileName);
            String targetFileName = openFileDialog.SafeFileName;
            if (foundVersionModel.IsExistVersion(targetFileName))
            {
                MessageBox.Show("已有此版本文件，请重试");
                return;
            }
            //MessageBox.Show(targetFileName);
            AskSingleInput.InputData inputData = new AskSingleInput.InputData();
            if (AskSingleInput.Show("版本描述", "新建版本", inputData) == false) return;
            SingleVersionFile svf = new SingleVersionFile();
            svf.FatherName = foundVersionModel.FileName;
            svf.VersionName = targetFileName;
            svf.Depict = inputData.Input;
            //MessageBox.Show("Time : " + DateTime.Now.ToString());
            svf.Date = DateTime.Now.ToString();
            refManager.CopyVersion(openFileDialog.FileName, svf);
            foundVersionModel.AddSingleVersion(svf);
            ShowVersionTree();
        }

        private void DeleteRef_Click(object sender, RoutedEventArgs e)
        {
            if (foundVersionModel == null)
            {
                MessageBox.Show("软件错误，请联系管理员");
                return;
            }
            if (foundVersionModel.versionList.Count != 0)
            {
                if (NoAskNsg.Show("删除非空引用将删除其下所有版本，确认删除吗？", 
                    "删除引用") == false) {
                    return;
                }
            }
            //MessageBox.Show("正在删除");
            //以后还要在这里实现删除的时候连带所有的引用一并删除！！！
            if (MainFolder.AskDeleteAllRef(foundVersionModel.FileName, null,
                "") == false) return;
            MainFolder.CommitDeleteRef(foundVersionModel.FileName, null);
            refManager.DeleteRef(foundVersionModel.FileName);
            Versions.Remove(foundVersionModel);
            ShowFolderTree();
            ShowVersionTree();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O && (Keyboard.Modifiers & (ModifierKeys.Control)) == (ModifierKeys.Control))
            {
                if (ModifyPer == true)
                {
                    MessageBox.Show("权限已开启，无需重复操作");
                    return;
                }
                if (PasswdCheck.Check())
                {
                    ModifyPer = true;
                    //MessageBox.Show("开启权限成功");
                    onSelectFileChanged();
                    onSelectVersionChanged();
                }
            }
        }

        private void NewRef_Click(object sender, RoutedEventArgs e)
        {
            AskSingleInput.InputData inputData = new AskSingleInput.InputData();
            if (AskSingleInput.Show("引用名称：", "新建引用", inputData) == true)
            {
                //MessageBox.Show("Success");
                foreach (VersionModel x in Versions)
                {
                    if (x.Equals(inputData.Input))
                    {
                        MessageBox.Show("该引用已存在");
                        return;
                    }
                }
                VersionModel versionModel = new VersionModel(inputData.Input);
                Versions.Add(versionModel);
                refManager.CreateRef(inputData.Input);
                ShowVersionTree();
            }
        }

        public void onSelectFileChanged()
        {
            if (ModifyPer == false) return;
            if (foundFileOrFolder == null)
            {
                newFile.IsEnabled = false;
                refreshFile.IsEnabled = false;
                deleteFile.IsEnabled = false;
                newFolder.IsEnabled = false;
                renameFolder.IsEnabled = false;
                deleteFolder.IsEnabled = false;
            }
            else
            {
                switch (foundFileOrFolder.Type)
                {
                    case FileAndFolderModel.Type_File:
                        newFile.IsEnabled = false;
                        refreshFile.IsEnabled = true;
                        deleteFile.IsEnabled = true;
                        newFolder.IsEnabled = false;
                        renameFolder.IsEnabled = false;
                        deleteFolder.IsEnabled = false;
                        break;
                    case FileAndFolderModel.Type_Folder:
                        newFile.IsEnabled = true;
                        refreshFile.IsEnabled = false;
                        deleteFile.IsEnabled = false;
                        newFolder.IsEnabled = true;
                        renameFolder.IsEnabled = true;
                        deleteFolder.IsEnabled = true;
                        break;
                }
            }
        }

        public void onSelectVersionChanged()
        {
            if (ModifyPer == false) return;
            newRef.IsEnabled = true;
            if (foundSingleVersionFile == null)
            {
                deleteVersion.IsEnabled = false;
            }
            else
            {
                deleteVersion.IsEnabled = true;
            }
            if (foundVersionModel == null)
            {
                createVerion.IsEnabled = false;
                deleteRef.IsEnabled = false;
            }
            else
            {
                createVerion.IsEnabled = true;
                deleteRef.IsEnabled = true;
            }
            
        }

        private void TreeViewVersion_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeViewVersion.SelectedItem != null)
            {
                TreeViewItem x = (TreeViewItem)treeViewVersion.SelectedItem;
                VersionModel versionModelFound = null;
                SingleVersionFile versionFileFound = null;
                foreach (KeyValuePair<TreeViewItem, VersionModel>
                    pair in treeVersionMap)
                {
                    if (pair.Key == x)
                    {
                        versionModelFound = pair.Value;
                        break;
                    }
                }
                if (versionModelFound == null)
                {
                    foreach (KeyValuePair<TreeViewItem, SingleVersionFile>
                    pair in treeSingleVersionMap)
                    {
                        if (pair.Key == x)
                        {
                            versionFileFound = pair.Value;
                            break;
                        }
                    }
                }
                // MessageBox.Show("Select Version!" + versionFileFound + "," +
                //    versionModelFound);
                if (versionFileFound == null && versionModelFound == null) return;
                if (versionFileFound != null)
                {
                    foundSingleVersionFile = versionFileFound;
                    foundVersionModel = null;
                }
                else
                {
                    foundSingleVersionFile = null;
                    foundVersionModel = versionModelFound;
                }
                onSelectVersionChanged();
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem != null)
            {
                FileAndFolderModel fileOrFolder = null;
                TreeViewItem x = (TreeViewItem)treeView.SelectedItem;
                foreach (KeyValuePair<TreeViewItem, FileAndFolderModel> 
                    pair in treeFolderMap)
                {
                    if (pair.Key == x)
                    {
                        fileOrFolder = pair.Value;
                        break;
                    }
                }
                if (fileOrFolder == null) return;
                foundFileOrFolder = fileOrFolder;
                //MessageBox.Show("FileName = " + fileOrFolder.FileName);
                onSelectFileChanged();
            }
        }

        public void ShowFolderTree()
        {
            treeFolderMap.Clear();
            treeView.Items.Clear();
            TreeViewItem rootFolder = new TreeViewItem();
            treeFolderMap.Add(new KeyValuePair<TreeViewItem, FileAndFolderModel>(
                rootFolder, MainFolder));
            rootFolder.Header = "MainFolder";
            MainFolder.ShowInTreeFolder(rootFolder, treeFolderMap);
            treeView.Items.Add(rootFolder);
            rootFolder.IsSelected = true;
            rootFolder.IsExpanded = true;
            //MessageBox.Show("FolderMapCount = " + treeFolderMap.Count);
            onSelectFileChanged();
            TreeView_SelectedItemChanged(null, null);
        }
        public void ShowVersionTree()
        {
            treeSingleVersionMap.Clear();
            treeVersionMap.Clear();
            treeViewVersion.Items.Clear();
            foreach (VersionModel x in Versions)
            {
                x.ShowInTree(treeViewVersion, treeSingleVersionMap, treeVersionMap);
            }
            if (treeVersionMap.Count != 0)
            {
                treeVersionMap[0].Key.IsSelected = true;
            }
            TreeViewVersion_SelectedItemChanged(null, null);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            //自动保存所有设置
            XmlEncoder.XmlOutput(MainFolder, Versions, "record.xml");
            base.OnClosing(e);
        }
    }
}
