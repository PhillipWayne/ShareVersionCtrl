﻿using ShareVersionCtrl.XMLRelated;
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
        FileAndFolderModel MainFolder;
        List<VersionModel> Versions;
        List<KeyValuePair<TreeViewItem, FileAndFolderModel>> treeFolderMap;
        List<KeyValuePair<TreeViewItem, SingleVersionFile>> treeSingleVersionMap;
        List<KeyValuePair<TreeViewItem, VersionModel>> treeVersionMap;
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
            XmlEncoder.XmlOutput(MainFolder, Versions, "record.xml");
            //MessageBox.Show(MainFolder.GetTreeInfo(0));
            //MessageBox.Show(VersionModel.ShowAllVersion(Versions));
            ShowFolderTree();
            ShowVersionTree();
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
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
                    }
                }
                if (fileOrFolder == null) return;
                MessageBox.Show("FileName = " + fileOrFolder.FileName);
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
            TreeViewItem versionTree = new TreeViewItem();
            versionTree.Header = "Versions";
            treeViewVersion.Items.Add(versionTree);
            foreach (VersionModel x in Versions)
            {
                x.ShowInTree(versionTree);
            }
            versionTree.IsExpanded = true;
        }
    }
}
