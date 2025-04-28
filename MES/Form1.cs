using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Collections;


namespace MES
{

    public partial class Form1 : Form
    {
        string connectionString = "Server=localhost;Database=MES_TEST;Trusted_Connection=True;";

        public Form1()
        {
            InitializeComponent();

            comboBox1.Items.Add("대한민국");
            comboBox1.Items.Add("중국");
            comboBox1.Items.Add("일본");
            comboBox1.Items.Add("인도");
            comboBox1.Items.Add("미국");
            comboBox1.Items.Add("대만");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label2.Text = textBox1.Text;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem != null)
            {
                string selecter = comboBox1.SelectedItem.ToString();
                label3.Text = selecter;
            }
            else
            {
                label3.Text = "나라를 선택해 주세요";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("생산지시번호");
            dt.Columns.Add("품목명");
            dt.Columns.Add("수량");

            // 샘플 데이터
            dt.Rows.Add("WD-001", "제품A", 100);
            dt.Rows.Add("WD-002", "제품B", 200);
            dt.Rows.Add("WD-003", "제품C", 300);

            // DataGridView 할당
            dataGridView1.DataSource = dt;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox2.Text = row.Cells["생산지시번호"].Value.ToString();
                textBox3.Text = row.Cells["품목명"].Value.ToString();
                textBox4.Text = row.Cells["수량"].Value.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                var (orderNo, ItemName, qty) = GetFormData();

                dataGridView1.CurrentRow.Cells["생산지시번호"].Value = orderNo;
                dataGridView1.CurrentRow.Cells["품목명"].Value = ItemName;
                dataGridView1.CurrentRow.Cells["수량"].Value = qty;
            }
            else
            {
                MessageBox.Show("수정할 항목을 먼저 선택해주세요");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                DialogResult result = MessageBox.Show("정말 삭제하시겠습니까?", "삭제확인", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
                    ClearTextBoxes();
                }
            }
            else
            {
                MessageBox.Show("삭제할 항목을 먼저 선택해주세요");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
            // 포커스 세팅
            textBox2.Focus();
        }

        // 텍스트 박스 초기화
        private void ClearTextBoxes()
        {
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        // 텍스트박스에서 현재 데이터 가져오기
        private (string OrderNo, string ItemName, string Qyt) GetFormData()
        {
            return (textBox2.Text.Trim(), textBox3.Text.Trim(), textBox4.Text.Trim());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT OrderNo, ItemName, Qty From WorkOrder";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;
            }

        }
    }
}
