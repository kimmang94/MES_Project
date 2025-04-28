using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MES
{
    public partial class Form1 : Form
    {
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
    }
}
