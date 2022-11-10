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

namespace PNDC_ClientToServerProject
{
    public partial class formEmojis : MetroFramework.Forms.MetroForm
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

        public formEmojis()
        {
            InitializeComponent();

            // Windows Form BorderCurved
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
        }

        private void formEmojis_Load(object sender, EventArgs e)
        {
            // Remove The Form Top Most Section
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            // Click To Exit The Application
            Application.Exit();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            // Click To Hide The Emoji Form
            this.Hide();
        }
    }
}
