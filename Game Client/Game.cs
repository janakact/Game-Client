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
    public partial class Game : Form
    {
        private NetworkClient networkClient;
        private char[,] grid;

        public Game()
        {
            InitializeComponent();

            grid = new char[10, 10];
            for(int i =0; i<10;i++)
            {
                for (int j = 0; j < 10; j++)
                    grid[i, j] = 'N';
            }
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
                processRecievedMsg(e.Data);
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

        private void txtConsole_TextChanged(object sender, EventArgs e)
        {

        }

        private void processRecievedMsg(String data)
        {
            if (data.Substring(0,2)=="I:")
            {
                String[] arr = data.Split(':');

            }
            if(data==Constant.S2C_GAMESTARTED)
            {

            }

            String txt = "";
            for(int i=0; i<10; i++)
            {
                for (int j = 0; j < 10; j++)
                    txt += grid[i, j];
                txt += "\r\n";
            }
            txtGrid.Text = txt;
        }
    }
}
