using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Đồ_án
{
    public partial class Check : Form
    {
        public Check()
        {
            InitializeComponent();
            showData();
        }

        SqlConnection connect;
        string str = "Data Source=.\\SQLEXPRESS;Initial Catalog=MMH;Integrated Security=True";
        SqlCommand command;
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable table;

        void showData()
        {
            dataGridView1.ClearSelection();

            // Tạo kết nối
            connect = new SqlConnection(str);
            connect.Open();

            // Câu lệnh sql và thực hiện câu lệnh
            string query = "select * from App";
            command = new SqlCommand(query, connect);

            // Load dữ liệu từ database và dataTable
            table = new DataTable();
            adapter = new SqlDataAdapter(command);
            adapter.Fill(table);
            // Ngắt kết nối
            connect.Close();

            // Load từ dataTable lên DataGridView
            dataGridView1.DataSource = table;
        }
    }
}
