using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_task.Controls
{
    public partial class ControlLogin : UserControl
    {
        Point lastPoint;
        Control panel;
        MainForm mainForm;
        ControlRegistr controlRegistr;
        DataBaseManager db = new DataBaseManager();
        int userId = 0;
        public ControlLogin(Control p, MainForm form)
        {
            InitializeComponent();
            panel = p;
            mainForm = form;
            textBox1.Leave += (s, a) => { TooltipTextLeave("Введите логин", textBox1, pictureBox1, Properties.Resources.user1); };
            textBox2.Leave += (s, a) => { TooltipTextLeave("Введите пароль", textBox2, pictureBox2, Properties.Resources.pass1); };
            textBox1.Enter += (s, a) => { TooltipTextEnter("Введите логин", textBox1, pictureBox1, Properties.Resources.user2); };
            textBox2.Enter += (s, a) => { TooltipTextEnter("Введите пароль", textBox2, pictureBox2, Properties.Resources.pass2); };
        }

        private void Button1_Click(object sender, EventArgs e)
        {

            userId = db.Authorization(textBox1.Text, textBox2.Text);
            if (userId == 0)
            {
                MessageBox.Show("Неверный логин или пароль");
                textBox1.Text = "";
                textBox2.Text = "";
                TooltipTextLeave("Введите логин", textBox1, pictureBox1, Properties.Resources.user1);
                TooltipTextLeave("Введите пароль", textBox2, pictureBox2, Properties.Resources.pass1);
            }
            else
                Program.ChangePanel(new ControlMain(panel, userId, mainForm), panel);
        }
        void TooltipTextLeave(string text, TextBox textBox, PictureBox pictureBox, Bitmap picture)
        {
            if (textBox.Text == "")
            {
                textBox.Text = text;
                textBox.ForeColor = Color.Gray;
                pictureBox.BackgroundImage = picture;
            }
        }
        void TooltipTextEnter(string text, TextBox textBox, PictureBox pictureBox, Bitmap picture)
        {
            if (textBox.Text == text)
            {
                textBox.Text = "";
                textBox.ForeColor = Color.WhiteSmoke;
                pictureBox.BackgroundImage = picture;
            }
        }
        private void Label2_MouseEnter(object sender, EventArgs e)
        {
            label2.ForeColor = Color.WhiteSmoke;
        }

        private void Label2_MouseLeave(object sender, EventArgs e)
        {
            label2.ForeColor = Color.Gray;
        }

        private void ExitPictureBox_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ExitPictureBox_MouseEnter(object sender, EventArgs e)
        {
            exitPictureBox.Image = Properties.Resources.no2;
        }

        private void ExitPictureBox_MouseLeave(object sender, EventArgs e)
        {
            exitPictureBox.Image = Properties.Resources.no1;
        }

        private void Panel3_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mainForm.Left += e.X - lastPoint.X;
                mainForm.Top += e.Y - lastPoint.Y;
            }
        }

        private void Panel3_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint.X = e.X;
            lastPoint.Y = e.Y;
        }
        private void Label2_Click(object sender, EventArgs e)
        {
            controlRegistr = new ControlRegistr(panel, mainForm);
            panel.Controls.Add(controlRegistr);
            controlRegistr.Left -= 800;
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            controlRegistr.Left += 20;
            this.Left += 20;
            if (this.Left == 800)
            {
                timer1.Stop();
                panel.Controls.RemoveAt(0);
            }
        }
    }
}
