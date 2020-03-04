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
    public partial class ControlRegistr : UserControl
    {
        Control panel;
        MainForm mainForm;
        ControlLogin controlRegistr;
        Point lastPoint;
        DataBaseManager db = new DataBaseManager();
        int userId = 0;

        public ControlRegistr(Control p, MainForm form)
        {
            InitializeComponent();
            panel = p;
            mainForm = form;
            db.GetPosition(listBox1);
            db.GetNamePotentialChief(listBox2);
            textBox1.Leave += (s, a) => { TooltipTextLeave("Введите логин", textBox1); };
            textBox2.Leave += (s, a) => { TooltipTextLeave("Введите пароль", textBox2); };
            textBox3.Leave += (s, a) => { TooltipTextLeave("Введите имя и фамилию", textBox3); };
            textBox1.Enter += (s, a) => { TooltipTextEnter("Введите логин", textBox1); };
            textBox2.Enter += (s, a) => { TooltipTextEnter("Введите пароль", textBox2); };
            textBox3.Enter += (s, a) => { TooltipTextEnter("Введите имя и фамилию", textBox3); };

            listBox1.SelectedIndex = 0;
            listBox2.SelectedIndex = 0;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            controlRegistr = new ControlLogin(panel, mainForm);
            panel.Controls.Add(controlRegistr);
            controlRegistr.Left += 800;
            timer1.Start();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length >= 5 && textBox2.Text.Length >= 5)
            {
                if (textBox3.Text.Split(' ').Length == 2)
                {
                    if (db.CheckLogin(textBox1.Text))
                    {
                        userId = db.GetLastUserId() + 1;
                        if (db.Register(userId, textBox1.Text, textBox2.Text, textBox3.Text.Split(' ')[0], textBox3.Text.Split(' ')[1], listBox1.SelectedIndex + 1, SetChiefId()))
                            Program.ChangePanel(new ControlMain(panel, userId, mainForm), panel);
                        else
                            MessageBox.Show("Ошибка регистрации");
                        
                    }
                    else
                        MessageBox.Show("Логин уже занят");
                    
                }
                else
                    MessageBox.Show("Введите имя и фамилию через пробел");
                
            }
            else
                MessageBox.Show("Логин и пароль должны содержать 5 или более символов");
            
        }
        int SetChiefId()
        {
            if (listBox2.SelectedIndex == 0)
                return 0;
            else
                return db.GetUserIdForName(listBox2.SelectedItem.ToString().Split(' ')[0], listBox2.SelectedItem.ToString().Split(' ')[1]);
        }
        void TooltipTextLeave(string text, Control textBox)
        {
            if (textBox.Text == "")
            {
                textBox.Text = text;
                textBox.ForeColor = Color.Gray;
            }
        }
        void TooltipTextEnter(string text, Control textBox)
        {
            if (textBox.Text == text)
            {
                textBox.Text = "";
                textBox.ForeColor = Color.WhiteSmoke;
            }
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

        private void Panel6_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mainForm.Left += e.X - lastPoint.X;
                mainForm.Top += e.Y - lastPoint.Y;
            }
        }

        private void Panel6_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint.X = e.X;
            lastPoint.Y = e.Y;
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            controlRegistr.Left -= 20;
            this.Left -= 20;
            if (this.Left == -800)
            {
                timer1.Stop();
                panel.Controls.RemoveAt(0);
            }
        }
    }
}
