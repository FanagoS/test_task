using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_task
{
    public partial class MainForm : Form
    {
        DataBaseManager db = new DataBaseManager();
        public MainForm()
        {
            InitializeComponent();
            panel1.Controls.Add(new Controls.ControlLogin(panel1, this));
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            db.CloseConnection();
        }

    }
}
