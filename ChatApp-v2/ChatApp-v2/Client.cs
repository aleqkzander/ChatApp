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
            if (Properties.Settings.Default.lastip != string.Empty) 
                txtIP.Text = Properties.Settings.Default.lastip;
            client = new SimpleTcpClient(txtIP.Text, 8888);
            client.Events.Connected += Events_Connected;
            client.Events.DataReceived += Events_DataReceived;
            client.Events.Disconnected += Events_Disconnected;
        }


        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            // log to console when disconnect occured
            this.Invoke((MethodInvoker)delegate
            {
                //txtLog.Text += $"<{DateTime.Now}> Disconnected \n\n";
            });
        }


        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            // call receive method and pass the receivced data from the event
            ReceiveMessage(txtLog, txtMessage, e);
        }


        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            // log to console when connection occured
            this.Invoke((MethodInvoker)delegate
            {
                //txtLog.Text += $"<{DateTime.Now}> Connected \n\n";
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
            // return when data empty
            if (e.Data.Array == null)
            {
                return;
            }


            // console log
            Invoke((MethodInvoker)delegate {
                //logBox.Text += $"<{DateTime.Now}-{e.IpPort}>: send a message \n\n";
            });


            // receivce message and format data for displaying
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
                // convert data and send to user
                string username = txtUsername.Text;
                string _messageToSend = $"{username}#{txtMessage.Text}";
                client.Send(_messageToSend);

                // format data for own display
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
                MessageBox.Show("Client connected");
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
            MessageBox.Show("Application will be restarted now");
            Application.Restart();
        }
    }
}
