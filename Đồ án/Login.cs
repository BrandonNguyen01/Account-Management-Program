using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Đồ_án
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            txtPass.UseSystemPasswordChar = true;
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

        private void llableRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Register rs = new Register();
            this.Hide();
            rs.ShowDialog();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtAcc.Text;
            string password = txtPass.Text;
            if (Check(username, password) == true)
            {
                MessageBox.Show("Đăng nhập thành công!", "Thông báo!", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Thông báo!", MessageBoxButtons.OK);
            }
        }

        // Kiểm tra password được nhập vào với Database
        bool Check( string username, string password)
        {
            // Tạo kết nối
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
            
            // Load dữ liệu vào datatable để kiểm tra
            query = "select * from App";
            command = new SqlCommand(query, connect);
            table = new DataTable();
            adapter = new SqlDataAdapter(command);
            adapter.Fill(table);

            // Kiểm tra.
            for (int index = 0; index < table.Rows.Count; index++ )
            {
                string checkUser,checkPass;
                checkPass = table.Rows[index]["Pass"].ToString();
                checkUser = table.Rows[index]["Account"].ToString();
                // So sánh dữ liệu.
                if (Compare(cipherHash, checkPass) == true && Compare(txtAcc.Text.ToString(), checkUser) == true)
                    return true;
            }

            return false;
        }

        // So sánh 2 chuỗi ký tự
        bool Compare(string s1, string s2)
        {
            return String.Compare(s1, s2) == 0;
        }

        // Ẩn hiện password
        private void btnShow_Click(object sender, EventArgs e)
        {
            if (txtPass.UseSystemPasswordChar == true)
                txtPass.UseSystemPasswordChar = false;
            else txtPass.UseSystemPasswordChar = true;
        }

        // Thoát
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
