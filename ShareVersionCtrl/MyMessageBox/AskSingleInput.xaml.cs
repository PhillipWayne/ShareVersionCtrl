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
    /// Interaction logic for AskSingleInput.xaml
    /// </summary>
    public partial class AskSingleInput : Window
    {
        InputData inputData;

        public AskSingleInput(String Ask, String Title, InputData inputData_)
        {
            InitializeComponent();
            inputData = inputData_;
            this.Title = Title;
            textBlock.Text = Ask;
            this.button_ok.Click += Button_ok_Click;
            this.button_cancel.Click += Button_cancel_Click;
        }

        private void Button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Button_ok_Click(object sender, RoutedEventArgs e)
        {
            if (! inputData.IsInputLegal(textBox.Text)) return;
            inputData.Input = textBox.Text;
            this.DialogResult = true;
            this.Close();
        }

        public static bool? Show(String Ask, String Title, InputData inputData)
        {
            AskSingleInput askSingleInput = new AskSingleInput(Ask, 
                Title, inputData);
            return askSingleInput.ShowDialog();
        }

        public class InputData
        {
            public String Input;
            public virtual bool IsInputLegal(String Input)
            {
                bool IsLegal = Input != null && (!Input.Equals(""));
                if (!IsLegal)
                {
                    MessageBox.Show("输入不能为空");
                }
                return IsLegal;
            }
        }
    }
}
