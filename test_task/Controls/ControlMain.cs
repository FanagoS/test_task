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
    public partial class ControlMain : UserControl
    {
        Control panel;
        int userId;
        int selectUserId;
        Point lastPoint;
        MainForm mainForm;
        List<int> userIdinListBox = new List<int>();
        DataBaseManager db = new DataBaseManager();
        DataSet available;
        public ControlMain(Control p, int id, MainForm form)
        {
            InitializeComponent();
            panel = p;
            userId = id;
            selectUserId = id;
            mainForm = form;
            available = db.GetCheckAvailableTables(userId);
            GetInformationForTextBox();

            pictureBox1.MouseEnter += (s, a) => { pictureBox1.BackgroundImage = Properties.Resources.join2; };
            pictureBox1.MouseLeave += (s, a) => { pictureBox1.BackgroundImage = Properties.Resources.join1; };
            pictureBox2.MouseEnter += (s, a) => { pictureBox2.BackgroundImage = Properties.Resources.people2; };
            pictureBox2.MouseLeave += (s, a) => { pictureBox2.BackgroundImage = Properties.Resources.people1; };
            pictureBox2.MouseClick += (s, a) => { selectUserId = userId; listBox1.ClearSelected(); GetInformationForTextBox(); };
            pictureBox3.MouseEnter += (s, a) => { pictureBox3.BackgroundImage = Properties.Resources.minus2; };
            pictureBox3.MouseLeave += (s, a) => { pictureBox3.BackgroundImage = Properties.Resources.minus1; };
            pictureBox3.MouseClick += (s, a) => { if (selectUserId != userId){ db.DeleteUser(selectUserId); UpdateInformarionListBox(); if (userId == 1) AllUserSalary(); } };
            pictureBox4.MouseEnter += (s, a) => { pictureBox4.BackgroundImage = Properties.Resources.save2; };
            pictureBox4.MouseLeave += (s, a) => { pictureBox4.BackgroundImage = Properties.Resources.save1; };
            pictureBox4.MouseClick += (s, a) => { db.UpdateUserInformation(selectUserId, textBox1.Text, textBox2.Text); UpdateInformarionListBox(); };

            UpdateInformarionListBox();
            VisibleMenuItem();
        }

        void GetInformationForTextBox()
        {
            textBox4.Text = "";
            DataSet userInform = db.InformationForUserId(selectUserId);
            textBox1.Text = userInform.Tables[0].Rows[0].ItemArray[0].ToString();
            textBox2.Text = userInform.Tables[0].Rows[0].ItemArray[1].ToString();
            textBox3.Text = userInform.Tables[0].Rows[0].ItemArray[2].ToString();
            if (userInform.Tables[0].Rows[0].ItemArray[3].GetType().Name != "DBNull")
            {
                DataSet chiefInform = db.GetUserInformarion(Convert.ToInt32(userInform.Tables[0].Rows[0].ItemArray[3]));
                if (chiefInform.Tables[0].Rows.Count>0)
                    textBox4.Text = chiefInform.Tables[0].Rows[0].ItemArray[0].ToString() + " " + chiefInform.Tables[0].Rows[0].ItemArray[1].ToString();
                chiefInform.Clear();
            }
            DateTime dateTime = DateTime.Parse(userInform.Tables[0].Rows[0].ItemArray[4].ToString());
            DateTime dateTimeToday = DateTime.Today;
            textBox5.Text = Convert.ToString(Convert.ToInt32(dateTimeToday.Subtract(dateTime).TotalDays)/365);
            textBox6.Text = Convert.ToString(GetCountSalary(selectUserId));
        }
        private double GetCountSalary(int userId)
        {
            int position = db.GetPositionForUserId(userId);
            int baseSalary = db.GetBaseSalary();
            double bonussSalary = 0;
            double salary = 0;
            int jobTime = Convert.ToInt32(textBox5.Text);
            
            if (position == 1)
            {
                if (jobTime >= 10)
                    salary = 1.3 * baseSalary;
                else
                    salary = jobTime * 0.03 * baseSalary + baseSalary;
            }
            else if (position == 2)
            {
                DataSet inSubmission = db.GetUserIdForChiefId(userId);
                foreach (DataRow i in inSubmission.Tables[0].Rows)
                    bonussSalary += GetCountSalary(Convert.ToInt32(i.ItemArray[0])) * 0.005;
                if (jobTime >= 8)
                    salary = 1.4 * baseSalary+bonussSalary;
                else
                    salary = jobTime * 0.05 * baseSalary + baseSalary+bonussSalary;
            }
            else if (position == 3)
            {
                List<int> list = new List<int>();
                list = Sss(db.GetUserIdForChiefId(userId), list);
                foreach (int i in list)
                {
                        bonussSalary += GetCountSalary(Convert.ToInt32(i)) * 0.003;   
                }
                if (jobTime >= 35)
                    salary = 1.35 * baseSalary + bonussSalary;
                else
                    salary = jobTime * 0.01 * baseSalary + baseSalary + bonussSalary;
            }
            return salary;
        }
        private void AllUserSalary()
        {
            double salary = 0;
            foreach (int i in userIdinListBox)
                salary += GetCountSalary(i);
            label8.Text = salary.ToString();
        }
        private List<int> Sss(DataSet dataSet, List<int> list, int flag = 0)
        {
            foreach (DataRow i in dataSet.Tables[0].Rows)
            {
                if (list.IndexOf(Convert.ToInt32(i.ItemArray[0])) == -1)
                {
                    list.Add(Convert.ToInt32(i.ItemArray[0]));
                }
            }
            List<int> listSearch = new List<int>(list);
            foreach (int i in listSearch)
            {
                if (listSearch.IndexOf(i) == flag)
                {
                    flag++;
                    list = Sss(db.GetUserIdForChiefId(i), list, flag);
                }
            }

            return list;
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(available.Tables[0].Rows[0].ItemArray[1])&&listBox1.SelectedIndex>=0)
            {
                selectUserId = userIdinListBox[listBox1.SelectedIndex];
                GetInformationForTextBox();
            }
        }
        private void UpdateInformarionListBox()
        {
            listBox1.Items.Clear();
            userIdinListBox.Clear();
            if (Convert.ToBoolean(available.Tables[0].Rows[0].ItemArray[2]))
                db.InformationAllUser(listBox1, userIdinListBox);
            else if (Convert.ToBoolean(available.Tables[0].Rows[0].ItemArray[0]))
            {
                DataSet inSubmission = db.GetUserIdForChiefId(userId);
                foreach (DataRow i in inSubmission.Tables[0].Rows)
                {
                    listBox1.Items.Add(db.InformationForUserId(Convert.ToInt32(i.ItemArray[0])).Tables[0].Rows[0].ItemArray[0] + " "
                        + db.InformationForUserId(Convert.ToInt32(i.ItemArray[0])).Tables[0].Rows[0].ItemArray[1]);
                    userIdinListBox.Add(Convert.ToInt32(i.ItemArray[0]));
                }
            }
        }
        private void ExitPictureBox_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            Program.ChangePanel(new ControlLogin(panel, mainForm), panel);
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

        private void PictureBox5_Click(object sender, EventArgs e)
        {

        }
        private void ExitPictureBox_MouseEnter(object sender, EventArgs e)
        {
            exitPictureBox.Image = Properties.Resources.no2;
        }

        private void ExitPictureBox_MouseLeave(object sender, EventArgs e)
        {
            exitPictureBox.Image = Properties.Resources.no1;
        }
        private void VisibleMenuItem()
        {
            if (userId == 1)
            {
                label8.Visible = true;
                label9.Visible = true;
                AllUserSalary();
            }
            foreach (PictureBox pictureBox in panel2.Controls)
                pictureBox.Visible = true;
            if (!Convert.ToBoolean(available.Tables[0].Rows[0].ItemArray[3]))
            {
                if (!Convert.ToBoolean(available.Tables[0].Rows[0].ItemArray[0]) || db.GetUserIdForChiefId(userId).Tables[0].Rows.Count == 0)
                {
                    pictureBox5.Location = pictureBox2.Location;
                    pictureBox2.Visible = false;
                    pictureBox3.Visible = false;
                    pictureBox4.Visible = false;
                }
                else
                    HideEditMenu();
            }
            else 
                HideEditMenu();
        }
        private void HideEditMenu()
        {
            if (!Convert.ToBoolean(available.Tables[0].Rows[0].ItemArray[3]))
            {
                pictureBox5.Location = pictureBox3.Location;
                pictureBox3.Visible = false;
                pictureBox4.Visible = false;
            }
        }

    }
}
