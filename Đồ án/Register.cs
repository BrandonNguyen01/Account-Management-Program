using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace Đồ_án
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
            txtPass.UseSystemPasswordChar = true;
            txtPass1.UseSystemPasswordChar = true;
        }
        
        SqlConnection connect;
        string str = "Data Source=.\\SQLEXPRESS;Initial Catalog=MMH;Integrated Security=True";
        SqlCommand command;
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable table;
        string query = "";

        // Hàm băm Hash
        static string Hash(string input)
        {
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        // Mã hóa Des
        public string EncryptDES(string source, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteHash;
            byte[] byteBuff;

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB;
            byteBuff = Encoding.UTF8.GetBytes(source);

            string encoded = Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return encoded;
        }
        
        // Mã hóa Vigenere
        public void EncryptVinegere(ref StringBuilder s, string key)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsLower(s[i]))
                    s[i] = char.ToLower(s[i]);
                else
                    s[i] = char.ToUpper(s[i]);
            }
            key = key.ToString();
            int j = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsLetter(s[i]))
                {
                    s[i] = (char)(s[i] + key[j] - 'a');
                    if (s[i] > 'z') s[i] = (char)(s[i] - 'z' + 'a' - 1);
                }
                else
                {
                    s[i] = (char)(s[i] + key[j] - 'A');
                    if (s[i] > 'Z') s[i] = (char)(s[i] - 'Z' + 'A' - 1);
                }
                j = j + 1 == key.Length ? 0 : j + 1;
            }
        }

        public Register(string s):this ()
        {
            string pas = s;
            txtPass.Text = pas;
            txtPass1.Text = pas;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RandomPass rp = new RandomPass();
            this.Hide();
            rp.ShowDialog();
            this.Show();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            // Kiểm tra nhập tên tài khoản
            if (txtAcount.Text == "")
                MessageBox.Show("Vui lòng nhập tên tài khoản", "Thông Báo!", MessageBoxButtons.OK);
            else
            {
                // Kiểm tra pass
                if (CheckPassword(txtPass.Text) == false)
                {
                    MessageBox.Show("Password chưa đúng quy định! Phải có chữ Hoa, ký tự, số.", "Thông Báo!", MessageBoxButtons.OK);
                    txtPass.Text = txtPass1.Text = "";
                }
                // Kiểm tra nhập lại pass
                if (CheckPassword2() == false)
                {
                    MessageBox.Show("Xác nhận mật khẩu chưa đúng", "Thông Báo!", MessageBoxButtons.OK);
                }
                else
                {
                    // Tạo kết nối database
                    connect = new SqlConnection(str);
                    if (connect.State != ConnectionState.Open)
                        connect.Open();

                    // Tạo key mặc định
                    string keyVi = "uit";
                    string keyDes = "nt216";
                    string cipherVi, cipherDes, cipherHash;

                    // Mã hóa vigenere
                    StringBuilder StrB = new StringBuilder(txtPass.Text);
                    EncryptVinegere(ref StrB, keyVi);
                    cipherVi = Convert.ToString(StrB);

                    // Mã hóa DES
                    cipherDes = EncryptDES(cipherVi, keyDes);

                    // Dùng Hash băm đoạn cipher
                    cipherHash = Hash(cipherDes.ToString()).ToString();

                    // Truyền dữ liệu vào database
                    query = "INSERT INTO App(Account, Pass) VALUES (" + "N'" + txtAcount.Text + "', N'" + cipherHash + "')";
                    command = new SqlCommand(query, connect);
                    command.ExecuteNonQuery();
                    connect.Close();

                    MessageBox.Show("Đăng ký thành công!", "Thông Báo!", MessageBoxButtons.OK);
                }
            }
        }

        // Kiểm tra xác nhận pass lần 2
        private bool CheckPassword2()
        {
            if (txtPass.Text == txtPass1.Text) return true;
            else
                return false;
        }

        // Kiểm tra pass
        private  bool CheckPassword(string pass)
        {
            if (pass.Length < 8)
                return false;
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
                if (pass.Contains(c)&&(t==true))
                    return true;
            }

            return false;
        }

        // Ẩn hiện password
        private void btnShow_Click(object sender, EventArgs e)
        {
            if (txtPass.UseSystemPasswordChar == true)
                txtPass.UseSystemPasswordChar = false;
            else txtPass.UseSystemPasswordChar = true;
            if (txtPass1.UseSystemPasswordChar == true)
                txtPass1.UseSystemPasswordChar = false;
            else txtPass1.UseSystemPasswordChar = true;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            Check ck = new Check();
            ck.ShowDialog();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Login lg = new Login();
            this.Hide();
            lg.ShowDialog();
        }
    }
}
