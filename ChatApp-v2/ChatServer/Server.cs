using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuperSimpleTcp;

namespace ChatServer
{
    public partial class Server : Form
    {
        private SimpleTcpServer server;


        public Server()
        {
            InitializeComponent();
        }


        private void Server_Load(object sender, EventArgs e)
        {
            // start server
            if (Properties.Settings.Default.lastip != string.Empty)
                txtIP.Text = Properties.Settings.Default.lastip;
            server = new SimpleTcpServer(txtIP.Text, 8888);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
        }


        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            // call the receive method and pass in the received data
            ReceiveMessage(txtLog, txtMessage, e);
        }


        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            // log disconnect event
            Invoke((MethodInvoker)delegate
            {
                //txtLog.Text += $"<{DateTime.Now}-{e.IpPort}>: Disconnected \n\n";
                lstClients.Items.Remove(e.IpPort);
            });
        }


        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {
            // log connection event
            Invoke((MethodInvoker)delegate
            {
                //txtLog.Text += $"<{DateTime.Now}-{e.IpPort}>: Connected \n\n";
                lstClients.Items.Add(e.IpPort);
            });
        }


        /// <summary>
        /// This method receives a message and transforms it to usable data
        /// </summary>
        /// <param name="logBox"></param>
        /// <param name="messageBox"></param>
        /// <param name="e"></param>
        private void ReceiveMessage(RichTextBox logBox, TextBox messageBox, DataReceivedEventArgs e)
        {
            // log the incomming message
            Invoke((MethodInvoker)delegate {
                //logBox.Text += $"<{DateTime.Now}-{e.IpPort}>:\n send a message \n\n";
            });


            // format the data and present it
            Invoke((MethodInvoker)delegate {
                string _messageReceived = Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);
                string _finalMessage = _messageReceived.Replace("#", ":\n");
                txtReceived.Text += $"{DateTime.Now} " + _finalMessage + "\n\n";
            });
        }


        /// <summary>
        /// This method sends a message and transforms it to usable data
        /// </summary>
        private void SendMessage()
        {
            if (server.IsListening && !string.IsNullOrEmpty(txtMessage.Text) && lstClients.SelectedItem != null)
            {
                // convert the message and send to selected client
                string username = txtUsername.Text;
                string _messageToSend = $"{username}#{txtMessage.Text}";
                server.Send(lstClients.SelectedItem.ToString(), _messageToSend);

                // format data for own display
                string _finalMessage = _messageToSend.Replace("#", ":\n");
                txtReceived.Text += $"{DateTime.Now} " + _finalMessage + "\n\n";
                txtMessage.Text = string.Empty;
            }
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                server.Start();
                MessageBox.Show("Server started!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                SendMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void txtIP_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.lastip = txtIP.Text;
            Properties.Settings.Default.Save();
            MessageBox.Show("Application will be restarted for the address change become active");
            Application.Restart();
        }
    }
}
