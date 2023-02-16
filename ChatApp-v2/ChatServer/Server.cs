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
            server = new SimpleTcpServer(txtIP.Text, 8888);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
        }


        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            ReceiveMessage(txtLog, txtMessage, e);
        }


        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                txtLog.Text += $"<{DateTime.Now}-{e.IpPort}>: Disconnected \n\n";
                lstClients.Items.Remove(e.IpPort);
            });
        }


        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                txtLog.Text += $"<{DateTime.Now}-{e.IpPort}>: Connected \n\n";
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
            Invoke((MethodInvoker)delegate {
                logBox.Text += $"<{DateTime.Now}-{e.IpPort}>:\n send a message \n\n";
            });

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
                string username = txtUsername.Text;
                string _messageToSend = $"{username}#{txtMessage.Text}";
                server.Send(lstClients.SelectedItem.ToString(), _messageToSend);

                string _finalMessage = _messageToSend.Replace("#", ":\n");
                txtReceived.Text += $"{DateTime.Now} " + _finalMessage + "\n\n";
                txtMessage.Text = string.Empty;
            }
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start();
            txtLog.Text += $"<{DateTime.Now}>: Chat server started \n\n";
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                SendMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
