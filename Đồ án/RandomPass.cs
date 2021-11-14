using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Đồ_án
{
    public partial class RandomPass : Form
    {
        private List<char> C = new List<char>();

        public RandomPass()
        {
            InitializeComponent();
            txtNum.Text = "8";
        }

        private void btnRanPass_Click(object sender, EventArgs e)
        {
            int Num = Int32.Parse(txtNum.Text);
            string s = GenPass(Num);
            while( CheckPassword( s)==false)
            {
                s = GenPass(Num);
            }                
            txtRanPass.Text= s;
        }

        // Tạo mật khẩu ngẫu nhiên
        private string GenPass(int l)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] chars = new char[l];
            Random rd = new Random();
            
                for (int i = 0; i < l; i++)
                {
                    chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
                }
            
            return new string(chars);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Register rs = new Register(txtRanPass.Text);
            this.Hide();
            rs.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Register rs = new Register();
            this.Hide();
            rs.ShowDialog();
        }

        // Kiểm tra mật khẩu sau khi tạo
        private bool CheckPassword(string pass)
        {
            //không có khoảng trắng
            if (pass.Contains(" "))
                return false;

            //có 1 nhất 1 chữ hoa
            if (!pass.Any(char.IsUpper))
                return false;

            //có 1 nhất 1 chữ thường
            if (!pass.Any(char.IsLower))
                return false;

            bool t = false;
            //có 1 nhất 1 ký tự đặt biệt
            string specialCharacters = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
            char[] specialCharactersArray = specialCharacters.ToCharArray();
            foreach (char c in specialCharactersArray)
            {
                if (pass.Contains(c))
                    t = true;
            }

            char[] Numm = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            foreach (char c in Numm)
            {
                if (pass.Contains(c) && (t == true))
                    return true;
            }

            return false;
        }
    }
}
