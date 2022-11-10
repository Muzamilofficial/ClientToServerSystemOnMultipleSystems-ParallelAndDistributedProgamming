using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PNDC_ClientToServerProject
{
    public partial class formClient : Form
    {
        // Making Public To Uername For Recalling It To Another Form
        public static string PassingIPAddress;

        // Initializing The Variables To Libraries
        private TcpClient Client;
        Socket server;
        MemoryStream ms;
        IPEndPoint endpoint = null;
        private StreamReader STR;
        private StreamWriter STW;
        public string recieve;
        public string Text_to_send;


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
        public formClient()
        {
            InitializeComponent();

            // Windows Form BorderCurved
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
        }

        // Method For Connect Client To Server
        public void ConnectToServer()
        {
            Client = new TcpClient();
            IPEndPoint IP_end = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
            try
            {
                Client.Connect(IP_end);
                if (Client.Connected)
                {
                    listBox1.Items.Add("connected to Server" + "\n");
                    STW = new StreamWriter(Client.GetStream());
                    STR = new StreamReader(Client.GetStream());
                    STW.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync(); //start receiving data in background
                    backgroundWorker2.WorkerSupportsCancellation = true; //cancel threads

                }
            }

            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString());
            }
        }

        // Method For Sending The Messages To Server
        public void SendingMessageToServer()
        {
            if (richTextBox1.Text != "")
            {
                Text_to_send = richTextBox1.Text;
                backgroundWorker2.RunWorkerAsync();

            }

            if (richTextBox1.Text == "bye" || richTextBox1.Text == "Bye")
            {
                Application.Exit();
            }

            richTextBox1.Text = "";
        }

        // Method For BacgroundWork 1
        public void BackgroundWorkForFirstInstance()
        {
            while (Client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    this.listBox1.Invoke(new MethodInvoker(delegate() { listBox1.Items.Add("you :" + recieve + "\n"); }));
                    recieve = "";

                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message.ToString());
                }
            }
        }

        // Method For BacgroundWork 2
        public void BackgroundWorkForSecondInstance()
        {
            if (Client.Connected)
            {
                STW.WriteLine(Text_to_send);
                this.listBox1.Invoke(new MethodInvoker(delegate() { listBox1.Items.Add("Me :" + Text_to_send + "\n"); }));

            }
            else { MessageBox.Show("sending faild"); }
            backgroundWorker2.CancelAsync();
        }

        // Initialize The Hastable Library To emotions
        Hashtable emotions;

        // Method For Adding The Command To Emojis
        void CreateEmotions()
        {
            emotions = new Hashtable(6);
            emotions.Add(":)", PNDC_ClientToServerProject.Properties.Resources.all_the_best_emoji);
        }

        // Method For Adding The Emotions Into RichTextbox
        void AddEmotions()
        {
            foreach (string emote in emotions.Keys)
            {
                while (richTextBox1.Text.Contains(emote))
                {
                    int ind = richTextBox1.Text.IndexOf(emote);
                    richTextBox1.Select(ind, emote.Length);
                    Clipboard.SetImage((Image)emotions[emote]);

                    richTextBox1.Paste();
                }
            }
        }

        private void formClient_Load(object sender, EventArgs e)
        {
            // Remove The Form Top Most Section
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            // Focus On Textbox1 On Form Load
            textBox1.Focus();

            // Adding The Function Of Emotions CreateEmotions Which Will Create Emotions On Form Load
            CreateEmotions();
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            // Cliek to exit the application
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Show formSelectionDashboard Form On Button Click
            formSelectionDashboard fm = new formSelectionDashboard();
            fm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Calling The Method Of ConnectToServer
            ConnectToServer();

            // Calling Of Method For CallingDashboard
            //CallingDashboard();
        }

        //public void CallingDashboard()
        //{
        //    formSelectionDashboard frmserverdash = new formSelectionDashboard();
        //    frmserverdash.Show();
        //}

        private void button4_Click(object sender, EventArgs e)
        {
            // Calling The Method Of SendingMessageToServer
            SendingMessageToServer();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Calling Of Method BackgroundWorkForFirstInstance
            BackgroundWorkForFirstInstance();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            // Calling Of Method BackgroundWorkForSecondInstance
            BackgroundWorkForSecondInstance();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // Add The Method Of AddEmotions Which Will Emotions Into RichBox By The Help Of Commands Provided By The User
            AddEmotions();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form_SendServerFiles ssf = new Form_SendServerFiles();
            ssf.Show();
            this.Hide();
        }
    }
}
