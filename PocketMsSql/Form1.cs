using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PocketMsSql
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (!IsValid()) return;

            btnGo.Enabled = false;
            progressBarQuery.Visible = true;
            backgroundWorkerQuery.RunWorkerAsync();            
        }

        private void ShowResult()
        {
            try
            {
                var strConn = string.Format("Server={0};Database={1};User Id={2};Password={3};", txtHost.Text, txtDataBase.Text, txtUser.Text, txtPass.Text);
                using (var cnn = new SqlConnection(strConn))
                {
                    var cmd = new SqlCommand() { Connection = cnn, CommandType = CommandType.Text, CommandText = txtQuery.Text, CommandTimeout = 600 };
                    var dt = new DataTable();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (InvokeRequired)
                        BeginInvoke((MethodInvoker)delegate
                        {
                            dgvResult.DataSource = dt;
                        });
                }
            }
            catch (SqlException ex)
            {
                foreach (var erro in ex.Errors)
                    MessageBox.Show(erro.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValid()
        {
            foreach (var control in this.Controls)
                if (control is TextBox)
                    SetColorField(((TextBox)control));

            foreach (var control in this.Controls)
                if (control is TextBox)
                    if (string.IsNullOrEmpty(((TextBox)control).Text.Trim()))
                        return false;

            return true;
        }

        private void SetColorField(TextBox txtBox)
        {
            txtBox.BackColor = string.IsNullOrEmpty(txtBox.Text.Trim()) ? Color.LightSalmon : Color.White;
        }

        private void backgroundWorkerQuery_DoWork(object sender, DoWorkEventArgs e)
        {
            ShowResult();
        }

        private void backgroundWorkerQuery_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBarQuery.Visible = false;
            btnGo.Enabled = true;
        }
    }
}