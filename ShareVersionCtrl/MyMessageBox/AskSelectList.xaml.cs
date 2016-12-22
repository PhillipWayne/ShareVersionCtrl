using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ShareVersionCtrl.MyMessageBox
{
    /// <summary>
    /// Interaction logic for AskSelectList.xaml
    /// </summary>
    public partial class AskSelectList : Window
    {
        InputData inputData;

        public AskSelectList(String Title, InputData inputData_)
        {
            InitializeComponent();
            inputData = inputData_;
            this.Title = Title;
            this.listView.ItemsSource = inputData.listItems;
            this.listView.MouseDoubleClick += ListView_MouseDoubleClick;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.listView.SelectedItems.Count != 1) return;
            if (inputData.IsInputLegal(listView.SelectedIndex) == false) return;
            //MessageBox.Show("Select:" + listView.SelectedIndex);
            inputData.SelectIndex = listView.SelectedIndex;
            this.DialogResult = true;
            this.Close();
        }

        public static bool? Show(String Title, InputData inputData)
        {
            AskSelectList askSelectList = new AskSelectList(
                Title, inputData);
            return askSelectList.ShowDialog();
        }

        public class InputData
        {
            public int SelectIndex;
            public List<String> listItems;

            public virtual bool IsInputLegal(int Index)
            {
                SelectIndex = Index;
                if (SelectIndex < 0) return false;
                if (SelectIndex >= listItems.Count) return false;
                return true;
            }
        }
    }
}
