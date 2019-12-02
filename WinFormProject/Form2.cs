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
    public partial class Form2 : Form
    {
        private string stuID;
        private string stuName;
        private string team;
        private string locaID;
        private int selectedRowIndex;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(int selectedRowIndex, string v1, string v2, string v3, string v4)
        {
            InitializeComponent();
            this.selectedRowIndex = selectedRowIndex;
            this.stuID = v1;
            this.stuName = v2;
            this.team = v3;
            this.locaID = v4;
        }

        Form1 mainForm;
        private void Form2_Load(object sender, EventArgs e)
        {
            tbStudentID.Text = stuID;
            tbName.Text = stuName;
            tbTeamName.Text = team;
            tbLocationID.Text = locaID;

            if (Owner != null)
            {
                mainForm = Owner as Form1;
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            string[] rowDatas = {
                tbStudentID.Text,
                tbName.Text,
                tbTeamName.Text,
                tbLocationID.Text, };
            mainForm.InsertRow(rowDatas);
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string[] rowDatas = {
                tbStudentID.Text,
                tbName.Text,
                tbTeamName.Text,
                tbLocationID.Text };
            mainForm.UpdateRow(rowDatas);
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            mainForm.DeleteRow(stuID);
            this.Close();
        }

        private void btnTextBoxClear_Click(object sender, EventArgs e)
        {
            tbStudentID.Clear();
            tbName.Clear();
            tbTeamName.Clear();
            tbLocationID.Clear();
        }
    }
}
