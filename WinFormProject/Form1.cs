using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormProject
{
    public partial class Form1 : Form
    {
        MySqlConnection conn;
        MySqlDataAdapter dataAdapter;
        DataSet dataSet;
        int selectedRowIndex;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string connStr = "server=localhost;port=3306;database=inform;uid=root;pwd=1234";
            conn = new MySqlConnection(connStr);
            dataAdapter = new MySqlDataAdapter("SELECT * FROM student", conn);
            dataSet = new DataSet();

            dataAdapter.Fill(dataSet, "student");
            dataGridView1.DataSource = dataSet.Tables["student"];

            SetSearchComboBox();
        }

        #region ComboBox 세팅

        private void SetSearchComboBox()
        {
            string sql = "SELECT distinct team FROM student";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            try
            {
     
                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) 
                {
                    cbTeamName.Items.Add(reader.GetString("team"));
                }
                reader.Close();

        
                sql = "SELECT distinct locaID FROM student";
                cmd = new MySqlCommand(sql, conn);
                reader = cmd.ExecuteReader();
                while (reader.Read()) 
                {
                    cbLocationID.Items.Add(reader.GetString("locaID"));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void cbCountryCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sql = "SELECT distinct locaID FROM student WHERE team=@team";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@team", cbTeamName.Text);

            cbLocationID.Items.Clear();     

            try
            {
  
                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())  
                {
                    cbLocationID.Items.Add(reader.GetString("locaID"));
                }
                reader.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string queryStr;

            #region Select QueryString 만들기
            string[] conditions = new string[5];
            conditions[0] = (tbStudentID.Text != "") ? "stuID=@stuID" : null;
            conditions[1] = (tbName.Text != "") ? "stuName=@stuName" : null;
            conditions[2] = (cbTeamName.Text != "") ? "team=@team" : null;
            conditions[3] = (cbLocationID.Text != "") ? "locaID=@locaID" : null;


            if (conditions[0] != null || conditions[1] != null || conditions[2] != null || conditions[3] != null)
            {
                queryStr = $"SELECT * FROM student WHERE ";
                bool firstCondition = true;
                for (int i = 0; i < conditions.Length; i++)
                {
                    if (conditions[i] != null)
                        if (firstCondition)
                        {
                            queryStr += conditions[i];
                            firstCondition = false;
                        }
                        else
                        {
                            queryStr += " and " + conditions[i];
                        }
                }
            }
            else
            {
                queryStr = "SELECT * FROM student";
            }
            #endregion

            #region SelectCommand 객체 생성 및 Parameters 설정
            dataAdapter.SelectCommand = new MySqlCommand(queryStr, conn);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@stuID", tbStudentID.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@stuName", tbName.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@team", cbTeamName.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@locaID", cbLocationID.Text);

            #endregion

            try
            {
                conn.Open();
                dataSet.Clear();
                if (dataAdapter.Fill(dataSet, "student") > 0)
                    dataGridView1.DataSource = dataSet.Tables["student"];
                else
                    MessageBox.Show("찾는 데이터가 없습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRowIndex = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[selectedRowIndex];

            Form2 Dig = new Form2(
                selectedRowIndex,
                row.Cells[0].Value.ToString(),
                row.Cells[1].Value.ToString(),
                row.Cells[2].Value.ToString(),
                row.Cells[3].Value.ToString()
                );

            Dig.Owner = this;               
            Dig.ShowDialog();               
            Dig.Dispose();
        }


        public void InsertRow(string[] rowDatas)
        {
            string queryStr = "INSERT INTO student (stuID, stuName, team, locaID) " +
                "VALUES(@stuID, @stuName, @team, @locaID)";
            dataAdapter.InsertCommand = new MySqlCommand(queryStr, conn);
            dataAdapter.InsertCommand.Parameters.Add("@stuID", MySqlDbType.Int32);
            dataAdapter.InsertCommand.Parameters.Add("@stuName", MySqlDbType.VarChar);
            dataAdapter.InsertCommand.Parameters.Add("@team", MySqlDbType.VarChar);
            dataAdapter.InsertCommand.Parameters.Add("@locaID", MySqlDbType.Int32);

            #region Parameter를 이용한 처리
            dataAdapter.InsertCommand.Parameters["@stuID"].Value = rowDatas[0];
            dataAdapter.InsertCommand.Parameters["@stuName"].Value = rowDatas[1];
            dataAdapter.InsertCommand.Parameters["@team"].Value = rowDatas[2];
            dataAdapter.InsertCommand.Parameters["@locaID"].Value = rowDatas[3];

            try
            {
                conn.Open();
                dataAdapter.InsertCommand.ExecuteNonQuery();

                dataSet.Clear();                                       
                dataAdapter.Fill(dataSet, "student");                      
                dataGridView1.DataSource = dataSet.Tables["student"];                               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            #endregion
        }


        internal void DeleteRow(string stuID)
        {
            string sql = "DELETE FROM student WHERE stuID=@stuID";
            dataAdapter.DeleteCommand = new MySqlCommand(sql, conn);
            dataAdapter.DeleteCommand.Parameters.AddWithValue("@stuID", stuID);

            try
            {
                conn.Open();
                dataAdapter.DeleteCommand.ExecuteNonQuery();

                dataSet.Clear();
                dataAdapter.Fill(dataSet, "student");
                dataGridView1.DataSource = dataSet.Tables["student"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        internal void UpdateRow(string[] rowDatas)
        {
            string sql = "UPDATE student SET stuName=@stuName, team=@team, locaID=@locaID WHERE stuID=@stuID";
            dataAdapter.UpdateCommand = new MySqlCommand(sql, conn);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@stuID", rowDatas[0]);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@stuName", rowDatas[1]);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@team", rowDatas[2]);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@locaID", rowDatas[3]);

            try
            {
                conn.Open();
                dataAdapter.UpdateCommand.ExecuteNonQuery();

                dataSet.Clear(); 
                dataAdapter.Fill(dataSet, "student");
                dataGridView1.DataSource = dataSet.Tables["student"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        private void btnInsert_Click(object sender, EventArgs e)
        {
            Form2 Dig = new Form2();
            Dig.Owner = this;               
            Dig.ShowDialog();               
            Dig.Dispose();
        }




        private void btnClear_Click(object sender, EventArgs e)
        {
            tbStudentID.Clear();
            tbName.Clear();
            cbTeamName.Text = "";
            cbLocationID.Text = "";
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
    }
}
