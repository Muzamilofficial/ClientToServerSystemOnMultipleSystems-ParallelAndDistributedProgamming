using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PNDC_ClientToServerProject
{
    public partial class Form_SendServerFiles : MetroFramework.Forms.MetroForm
    {
        public Form_SendServerFiles()
        {
            InitializeComponent();
        }
        private string IP = "127.0.0.1";
        TcpListener listener;
        TcpClient client;
        Socket socketForClient;
        private Thread serverThread;
        private Thread findPC;
        private Thread notification;
        int flag = 0;
        string fileName = "";
        private bool serverRunning = false;
        private bool isConnected = false;
        int x = 9;
        int y = 308;
        int fileReceived = 0;
        string savePath;
        string senderIP;
        string senderMachineName;
        string targetIP;
        string targetName;
        Notificationform f2;

        private void Form_SendServerFiles_Load(object sender, EventArgs e)
        {
            // Getting IP Address From Client PC
            ipBox.Text = formClient.PassingIPAddress;

            notificationLabel.ForeColor = Color.Red;
            notificationLabel.Text = "Application is offline";
        }
        void startServer()
        {
            try
            {
                serverRunning = true;
                listener = new TcpListener(IPAddress.Parse(IP), 11000);
                listener.Start();
                serverThread = new Thread(new ThreadStart(serverTasks));
                serverThread.Start();
                while (!serverThread.IsAlive) ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //thread: waiting for client request and receiving data two times and resets.
        void serverTasks()
        {
            try
            {
                while (true)
                {
                    if (fileReceived == 1)
                    {
                        if (MessageBox.Show("Save File?", "File received", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            File.Delete(savePath);
                            fileReceived = 0;
                        }
                        else
                        {
                            fileReceived = 0;
                        }
                    }

                    client = listener.AcceptTcpClient();
                    Invoke((MethodInvoker)delegate
                    {
                        notificationPanel.Visible = true;
                        notificationTempLabel.Text = "File coming..." + "\n" + fileName + "\n" + "From: " + senderIP + " " + senderMachineName;
                        fileNotificationLabel.Text = "File Coming from " + senderIP + " " + senderMachineName;
                    });
                    isConnected = true;
                    NetworkStream stream = client.GetStream();
                    if (flag == 1 && isConnected)
                    {
                        savePath = savePathLabel.Text + "\\" + fileName;
                        using (var output = File.Create(savePath))
                        {
                            // read the file divided by 1KB
                            var buffer = new byte[1024];
                            int bytesRead;
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                output.Write(buffer, 0, bytesRead);
                            }
                            //MessageBox.Show("ok");
                            flag = 0;
                            client.Close();
                            isConnected = false;
                            fileName = "";
                            Invoke((MethodInvoker)delegate
                            {
                                notificationTempLabel.Text = "";
                                notificationPanel.Visible = false;
                                fileNotificationLabel.Text = "";
                            });
                            fileReceived = 1;
                        }
                    }
                    else if (flag == 0 && isConnected)
                    {
                        Byte[] bytes = new Byte[256];
                        String data = null;
                        int i;
                        // Loop to receive all the data sent by the client.
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        }
                        string[] msg = data.Split('@');
                        fileName = msg[0];
                        senderIP = msg[1];
                        senderMachineName = msg[2];
                        client.Close();
                        isConnected = false;
                        flag = 1;
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
                flag = 0;
                isConnected = false;
                if (client != null)
                    client.Close();
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            if (serverRunning)
            {
                if (listener != null)
                    listener.Stop();
                if (serverThread != null)
                {
                    serverThread.Abort();
                    serverThread.Join();
                }

            }
            Application.Exit();
        }

        private void changeSaveLocButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browse = new FolderBrowserDialog();
            if (browse.ShowDialog() == DialogResult.OK)
            {
                string savePath = browse.SelectedPath;
                savePathLabel.Text = savePath;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (serverRunning)
            {
                serverRunning = false;
                onlinePCList.Items.Clear();
                if (listener != null)
                    listener.Stop();
                if (serverThread != null)
                {
                    serverThread.Abort();
                    serverThread.Join();
                }

                notificationLabel.ForeColor = Color.Red;
                notificationLabel.Text = "Application is Offline";
                infoLabel.Text = "";
                fileNameLabel.Text = ".";
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            ipBox.Text = "";
            onlinePCList.Items.Clear();
            notificationLabel.ForeColor = Color.Green;
            notificationLabel.Text = "Finding...";
            //searchPC();
            try
            {
                findPC = new Thread(new ThreadStart(searchPC));
                findPC.Start();
                while (!findPC.IsAlive) ;
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
        }
        void searchPC()
        {
            bool isNetworkUp = NetworkInterface.GetIsNetworkAvailable();
            if (isNetworkUp)
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        this.IP = ip.ToString();
                    }
                }
                Invoke((MethodInvoker)delegate
                {
                    infoLabel.Text = "This Computer: " + this.IP;
                });
                string[] ipRange = IP.Split('.');
                for (int i = 100; i < 255; i++)
                {
                    Ping ping = new Ping();
                    //string testIP = "192.168.1.67";
                    string testIP = ipRange[0] + '.' + ipRange[1] + '.' + ipRange[2] + '.' + i.ToString();
                    if (testIP != this.IP)
                    {
                        ping.PingCompleted += new PingCompletedEventHandler(pingCompletedEvent);
                        ping.SendAsync(testIP, 100, testIP);
                    }
                }

                Invoke((MethodInvoker)delegate
                {
                    notificationLabel.ForeColor = Color.Green;
                    notificationLabel.Text = "Application is Online";
                });
                //Starting this program as a server.
                if (!serverRunning)
                    startServer();
            }
            else
            {
                Invoke((MethodInvoker)delegate
                {
                    notificationLabel.ForeColor = Color.Red;
                    notificationLabel.Text = "Application is Offline";
                });
                MessageBox.Show("Not connected to LAN");
            }
        }
        //for searching online computers
        void pingCompletedEvent(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            if (e.Reply.Status == IPStatus.Success)
            {
                string name;
                try
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                    name = hostEntry.HostName;
                }
                catch (SocketException ex)
                {
                    name = ex.Message;
                }
                Invoke((MethodInvoker)delegate
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = ip;
                    item.SubItems.Add(name);
                    onlinePCList.Items.Add(item);
                });
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "All Files|*.*";
            openFileDialog1.Title = "Select a File";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileNameLabel.Text = openFileDialog1.FileName;  //file path
                fileNameLabel.Tag = openFileDialog1.SafeFileName; //file name only.
            }
            timer1.Start();
        }

        private void sendFileButton_Click(object sender, EventArgs e)
        {
            targetIP = null;
            targetName = null;
            if ((onlinePCList.SelectedIndices.Count > 0 || ipBox.Text != "") && serverRunning && fileNameLabel.Text != ".")
            {
                if (ipBox.Text != "")
                {
                    targetIP = ipBox.Text;
                    targetName = "";
                }
                else
                {
                    targetIP = onlinePCList.SelectedItems[0].Text;
                    targetName = onlinePCList.SelectedItems[0].SubItems[1].Text;
                }
                try
                {
                    Ping p = new Ping();
                    PingReply r;
                    r = p.Send(targetIP);
                    if (!(r.Status == IPStatus.Success))
                    {
                        MessageBox.Show("Target computer is not available.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        notification = new Thread(new ThreadStart(showNotification));
                        notification.Start();

                        fileNotificationLabel.Text = "Please don't do other tasks. File sending to " + targetIP + " " + targetName + "...";
                        //closing the server
                        listener.Stop();
                        serverThread.Abort();
                        serverThread.Join();
                        serverRunning = false;

                        //now making this program a client
                        socketForClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socketForClient.Connect(new IPEndPoint(IPAddress.Parse(targetIP), 11000));
                        string fileName = fileNameLabel.Tag.ToString();
                        //long fileSize = new FileInfo(fileNameLabel.Text).Length;
                        byte[] fileNameData = Encoding.Default.GetBytes(fileName + "@" + this.IP + "@" + Environment.MachineName);
                        socketForClient.Send(fileNameData);
                        socketForClient.Shutdown(SocketShutdown.Both);
                        socketForClient.Close();
                        socketForClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socketForClient.Connect(new IPEndPoint(IPAddress.Parse(targetIP), 11000));
                        socketForClient.SendFile(fileNameLabel.Text);
                        socketForClient.Shutdown(SocketShutdown.Both);
                        socketForClient.Close();
                        //notification.Abort();
                        //notification.Join();
                        //notificationTempLabel.Text = "";
                        //notificationPanel.Visible = false;
                        Invoke((MethodInvoker)delegate
                        {
                            f2.Dispose();
                        });
                        MessageBox.Show("File sent to " + targetIP + " " + targetName, "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    if (socketForClient != null)
                    {
                        socketForClient.Shutdown(SocketShutdown.Both);
                        socketForClient.Close();
                    }
                }
                finally
                {
                    for (int i = 0; i < onlinePCList.SelectedIndices.Count; i++)
                    {
                        onlinePCList.Items[this.onlinePCList.SelectedIndices[i]].Selected = false;
                    }
                    fileNotificationLabel.Text = ".";
                    //again making this program a server
                    startServer();
                }
            }
        }
        void showNotification()
        {
            f2 = new Notificationform(targetName, targetIP);
            f2.ShowDialog();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            fileNameLabel.Text = ".";
            timer1.Stop();
        }
    }
}
