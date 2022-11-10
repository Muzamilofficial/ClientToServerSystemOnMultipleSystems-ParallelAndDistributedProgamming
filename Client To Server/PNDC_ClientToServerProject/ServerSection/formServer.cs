using System;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerSection
{
    public partial class formServer : Form
    {
        // Initializing The Variables To Libraries
        delegate void showMessageInThread(string message);
        private TcpClient Client;
        private StreamReader STR;
        private StreamWriter STW;
        Thread listenThread = null;
        public string recieve;
        public string Text_to_send;

        NetworkStream serverStream = default(NetworkStream);

        //Latest work
        UdpClient listener = null;
        IPEndPoint endpoint = null;

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

        public formServer()
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

            // RichTextbox 1 Displays This Message On Form Load
            ShowMsg("Waiting for a client!");

        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Click To Exit The Program
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Back To formServerLogin Form
            formServerLogin frmserverlog = new formServerLogin();
            frmserverlog.Show();
            this.Hide();
        }

        // Method For Starting The
        private void StartServer()
        {
            TcpListener listner = new TcpListener(IPAddress.Any, int.Parse(textBox2.Text));
            listner.Start();
            Client = listner.AcceptTcpClient();
            listenThread = new Thread(new ThreadStart(Listening));
            STR = new StreamReader(Client.GetStream());
            STW = new StreamWriter(Client.GetStream());
            STW.AutoFlush = true;

            //listenThread = new Thread(new ThreadStart(Listening));
            //listenThread.Start();
            backgroundWorker1.RunWorkerAsync(); //start receiving data in background
            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        // Method For Listening Used In Start Server Method
        private void Listening()
        {
            while (true)
            {
                //take the coming data

                byte[] comingDataFromClient = listener.Receive(ref endpoint);
                //this.richTextBox1.Text += Encoding.ASCII.GetString(comingDataFromClient);
                ImageConverter convertData = new ImageConverter();
                Image image = (Image)convertData.ConvertFrom(comingDataFromClient);
                //pictureBox1.Image = image;
            }
        }

        // Show Message Of Clients Connected
        private void ShowMsg(string msg)
        {
            this.richTextBox2.Text += msg + "\r\n";
        }

        // Method For BackgroundWorkForFirstInstance
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

        // Method For BackgroundWorkForSecondInstance
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

        private void button3_Click(object sender, EventArgs e)
        {
            //Calling Of Method StartServer
            StartServer();
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

        private void button4_Click(object sender, EventArgs e)
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

            using (StreamWriter writer = new StreamWriter("chatsave.txt"))
            {
                foreach (var item in listBox1.Items)
                {
                    writer.WriteLine(item.ToString());
                }
            }
        }
    }
}
