using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerSection;

namespace ServerSection
{
    public partial class formServerLogin : Form
    {
        // Make Windows Form Border Radius Curved
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn
            (
            int nLeftRect,
            int nTopRect,
            int RightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
            );

        public formServerLogin()
        {
            InitializeComponent();

            // Windows Form BorderCurved
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
        }

        private void formServer_Load(object sender, EventArgs e)
        {
            // Remove The Form Top Most Section
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            // Focus On Textbox1 On Form Load
            textBox1.Focus();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Cliek to exit the application
            Application.Exit();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        // Method For Login
        public void Login()
        {
            if (textBox1.Text == "8929")
            {
                formServer frmserver = new formServer();
                frmserver.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid Pincode", "Server Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                textBox1.Clear();
                textBox1.Focus();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Calling Of Method Login
            Login();
        }

        // KeyDown For Textbox 1 To Button 3
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3.Focus();
            }
        }
    }
}
