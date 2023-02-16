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

namespace ChatApp_v2
{
    public partial class Client : Form
    {
        SimpleTcpClient client;

        public Client()
        {
            InitializeComponent();
        }

        private void Client_Load(object sender, EventArgs e)
        {
            client = new SimpleTcpClient(txtIP.Text, 8888);
            client.Events.Connected += Events_Connected;
            client.Events.DataReceived += Events_DataReceived;
            client.Events.Disconnected += Events_Disconnected;
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtLog.Text += $"<{DateTime.Now}> Disconnected \n\n";
            });
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            ReceiveMessage(txtLog, txtMessage, e);
        }

        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtLog.Text += $"<{DateTime.Now}> Connected \n\n";
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
            if (e.Data.Array == null)
            {
                return;
            }

            Invoke((MethodInvoker)delegate {
                logBox.Text += $"<{DateTime.Now}-{e.IpPort}>: send a message \n\n";
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
            if (client.IsConnected && !string.IsNullOrEmpty(txtMessage.Text))
            {
                string username = txtUsername.Text;
                string _messageToSend = $"{username}#{txtMessage.Text}";
                client.Send(_messageToSend);

                string _finalMessage = _messageToSend.Replace("#", ":\n");
                txtReceived.Text += $"{DateTime.Now} " + _finalMessage + "\n\n";
                txtMessage.Text = string.Empty;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n\n" + ex.ToString());
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
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
