using ShareVersionCtrl.MyMessageBox;
using ShareVersionCtrl.XMLRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShareVersionCtrl
{
    class PasswdCheck
    {
        //public const String passwd = "pkumakerspace";
        public const String passwd = "root";
        public static bool Check()
        {
            AskSingleInput.InputData inputData = new AskSingleInput.InputData();
            if (AskSingleInput.Show("管理员修改密码：", "保存设置", inputData) == true)
            {
                //MessageBox.Show("Success");
                if (inputData.Input.Equals(passwd))
                {
                    //密码通过
                    //MessageBox.Show("密码正确");
                    return true;
                }
                else
                {
                    MessageBox.Show("密码错误");
                    return false;
                }
            }
            return false;
        }
    }
}
