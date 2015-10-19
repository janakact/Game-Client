using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_Client
{
    public partial class Form1 : Form
    {
        private NetworkClient networkClient;
        public Form1()
        {
            InitializeComponent();

            networkClient = new NetworkClient(Constant.SERVER_IP, Constant.SEND_PORT,Constant.LISTEN_PORT);
            networkClient.OnRecieve += onRecieve;

        }

        private void btnStartClient_Click(object sender, EventArgs e)
        {
            networkClient.StopListening();   
            networkClient.StartListening();
            
        }

        delegate void onRecieveCallback(object sender, DataRecieveEventArgs e);
        private void onRecieve(object sender, DataRecieveEventArgs e)
        {
            if (this.txtConsole.InvokeRequired)
            {
                onRecieveCallback d = new onRecieveCallback(onRecieve);
                this.Invoke(d, new object[] { sender, e });
            }
            else
            {
                txtConsole.Text += e.Data;
                txtConsole.Text +=  " - " + e.Time.ToString()+ "\r\n";
            }
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            networkClient.StopListening();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            networkClient.Send(txtSend.Text);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            networkClient.StopListening();
        }
    }
}
