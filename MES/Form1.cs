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

        // 셀클릭 이벤트
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox2.Text = row.Cells["OrderNo"].Value.ToString();
                textBox3.Text = row.Cells["ItemName"].Value.ToString();
                textBox4.Text = row.Cells["Qty"].Value.ToString();
            }
        }

        // 수정
        private void button3_Click(object sender, EventArgs e)
        {
            // 로컬 수정
            //    if (dataGridView1.CurrentRow != null)
            //    {
            //        var (orderNo, ItemName, qty) = GetFormData();

            //        dataGridView1.CurrentRow.Cells["OrderNo"].Value = orderNo;
            //        dataGridView1.CurrentRow.Cells["ItemName"].Value = ItemName;
            //        dataGridView1.CurrentRow.Cells["Qty"].Value = qty;
            //    }
            //    else
            //    {
            //        MessageBox.Show("수정할 항목을 먼저 선택해주세요");
            //    }

            // DB 접근 수정
            var (orderNo, itemName, qty) = GetFormData();

            if (string.IsNullOrWhiteSpace(orderNo) || string.IsNullOrWhiteSpace(itemName) || string.IsNullOrWhiteSpace(qty))
            {
                MessageBox.Show("모든 값을 입력해주세요.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE WorkOrder SET ItemName = @ItemName, Qty = @Qty WHERE OrderNo = @OrderNo";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                    cmd.Parameters.AddWithValue("@Qty", int.Parse(qty));
                    cmd.Parameters.AddWithValue("@OrderNo", orderNo);

                    try
                    {
                        int affectedRows = cmd.ExecuteNonQuery();
                        if (affectedRows > 0)
                        {
                            MessageBox.Show("수정이 완료되었습니다!");
                            LoadData();
                            ClearTextBoxes();
                        }
                        else
                        {
                            MessageBox.Show("수정할 데이터가 없습니다.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"에러 발생: {ex.Message}");
                    }
                }
            }
        }

        // 삭제
        private void button4_Click(object sender, EventArgs e)
        {
            // 로컬 수정
            //if (dataGridView1.CurrentRow != null)
            //{
            //    DialogResult result = MessageBox.Show("정말 삭제하시겠습니까?", "삭제확인", MessageBoxButtons.YesNo);
            //    if (result == DialogResult.Yes)
            //    {
            //        dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            //        ClearTextBoxes();
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("삭제할 항목을 먼저 선택해주세요");
            //}
            

            // DB 접근 수정
            if (dataGridView1.CurrentRow != null)
            {
                var (orderNo, _, _) = GetFormData(); // OrderNo만 필요하니까 나머지는 무시(_)

                var confirm = MessageBox.Show($"작업지시 [{orderNo}] 를 정말 삭제하시겠습니까?", "삭제 확인", MessageBoxButtons.YesNo);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = "DELETE FROM WorkOrder WHERE OrderNo = @OrderNo";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@OrderNo", orderNo);

                            try
                            {
                                int affectedRows = cmd.ExecuteNonQuery();

                                if (affectedRows > 0)
                                {
                                    MessageBox.Show("삭제가 완료되었습니다!");
                                    LoadData();       // 삭제 후 새로고침
                                    ClearTextBoxes(); // 입력칸 비우기
                                }
                                else
                                {
                                    MessageBox.Show("삭제할 데이터를 찾을 수 없습니다.");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"에러 발생: {ex.Message}");
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("삭제할 항목을 선택해주세요!");
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

        // View
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

        // Insert
        private void button7_Click(object sender, EventArgs e)
        {
            var (orderNo, ItemName, qty) = GetFormData();

            if (string.IsNullOrEmpty(orderNo) || string.IsNullOrEmpty(ItemName) || string.IsNullOrEmpty(qty))
            {
                MessageBox.Show("모든 값을 입력해 주세요");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO WorkOrder (OrderNo, ItemName, Qty) VALUES (@OrderNo, @ItemName, @Qty)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderNo", orderNo);
                    cmd.Parameters.AddWithValue("@ItemName", ItemName);
                    cmd.Parameters.AddWithValue("@Qty", int.Parse(qty));

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("등록이 완료되었습니다!");

                        // 등록 성공 후 새로고침
                        LoadData();
                        ClearTextBoxes();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"에러 발생: {ex.Message}");
                    }
                }
            }
        }

        // 데이터 조회
        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT OrderNo, ItemName, Qty FROM WorkOrder";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string keyword = textBox5.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                LoadData(); // 검색어가 없으면 전체 데이터 불러오기
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT OrderNo , ItemName , Qty  FROM WorkOrder WHERE OrderNo LIKE @Keyword OR ItemName LIKE @Keyword";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
            }
        }
    }
}
