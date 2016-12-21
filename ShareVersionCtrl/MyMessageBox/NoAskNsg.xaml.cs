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
    /// Interaction logic for NoAskNsg.xaml
    /// </summary>
    public partial class NoAskNsg : Window
    {
        public NoAskNsg(String Ask, String Title)
        {
            InitializeComponent();
            this.Title = Title;
            textBlock.Text = Ask;
            this.button_ok.Click += Button_ok_Click;
            this.button_cancel.Click += Button_cancel_Click;
            this.button_ok.Focus();
        }

        private void Button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Button_ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        public static bool? Show(String Ask, String Title)
        {
            NoAskNsg noAskMsg = new NoAskNsg(Ask,
                Title);
            return noAskMsg.ShowDialog();
        }
    }
}
