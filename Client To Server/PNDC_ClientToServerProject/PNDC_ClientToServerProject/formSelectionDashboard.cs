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

namespace PNDC_ClientToServerProject
{
    public partial class formSelectionDashboard : MetroFramework.Forms.MetroForm
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

        public formSelectionDashboard()
        {
            InitializeComponent();

            // Windows Form BorderCurved
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Remove The Form Top Most Section
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // To exit the application
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            formClient frmclient = new formClient();
            frmclient.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ServerSection.formServerLogin frmserver = new formServerLogin();
            frmserver.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Hiding The Dashbaord On btnClick
            this.Hide();
        }
    }
}
