using ShareVersionCtrl.MyMessageBox;
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
        public const String passwd = "pkumakerspace";
        FileAndFolderModel MainFolder;
        List<VersionModel> Versions;
        List<KeyValuePair<TreeViewItem, FileAndFolderModel>> treeFolderMap;
        List<KeyValuePair<TreeViewItem, SingleVersionFile>> treeSingleVersionMap;
        List<KeyValuePair<TreeViewItem, VersionModel>> treeVersionMap;
        FileAndFolderModel foundFileOrFolder = null;
        SingleVersionFile foundSingleVersionFile = null;
        VersionModel foundVersionModel = null;
        public String targetFolder = "../共享";

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
            //MessageBox.Show(MainFolder.GetTreeInfo(0));
            //MessageBox.Show(VersionModel.ShowAllVersion(Versions));
            ShowFolderTree();
            ShowVersionTree();
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
            treeViewVersion.SelectedItemChanged += TreeViewVersion_SelectedItemChanged;
            onSelectFileChanged();
            onSelectVersionChanged();
            /*Directory.CreateDirectory(targetFolder);
            MessageBox.Show("" + Directory.Exists(targetFolder));
            Directory.Delete(targetFolder, true); //不能删除非空文件夹
            MessageBox.Show("" + Directory.Exists(targetFolder));*/
            this.KeyDown += MainWindow_KeyDown;
            newRef.Click += NewRef_Click;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers & (ModifierKeys.Control)) == (ModifierKeys.Control))
            {
                AskSingleInput.InputData inputData = new AskSingleInput.InputData();
                if (AskSingleInput.Show("管理员修改密码：", "保存设置", inputData) == true)
                {
                    //MessageBox.Show("Success");
                    if (inputData.Input.Equals(passwd))
                    {
                        //密码通过
                        XmlEncoder.XmlOutput(MainFolder, Versions, "record.xml");
                        MessageBox.Show("修改保存成功");
                    }
                    else
                    {
                        MessageBox.Show("密码错误");
                    }
                }
            }
        }

        private void NewRef_Click(object sender, RoutedEventArgs e)
        {
            AskSingleInput.InputData inputData = new AskSingleInput.InputData();
            if (AskSingleInput.Show("引用名称：", "新建引用", inputData) == true)
            {
                //MessageBox.Show("Success");
                VersionModel versionModel = new VersionModel(inputData.Input);
                Versions.Add(versionModel);
                ShowVersionTree();
            }
        }

        public void onSelectFileChanged()
        {
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
            rootFolder.IsExpanded = true;
            //MessageBox.Show("FolderMapCount = " + treeFolderMap.Count);
        }
        public void ShowVersionTree()
        {
            treeViewVersion.Items.Clear();
            foreach (VersionModel x in Versions)
            {
                x.ShowInTree(treeViewVersion, treeSingleVersionMap, treeVersionMap);
            }
        }
    }
}
