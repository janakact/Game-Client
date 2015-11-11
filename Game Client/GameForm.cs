using System;
using System.Collections;
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
    public partial class GameForm : Form
    {
        private NetworkClient networkClient;
        private Game game;

        public GameForm()
        {
            InitializeComponent();
            game = new Game();
            networkClient =  NetworkClient.getInstance(Constant.SERVER_IP, Constant.SEND_PORT,Constant.LISTEN_PORT);
            networkClient.OnRecieve += onRecieve;

        }

        private void btnStartClient_Click(object sender, EventArgs e)
        {
            networkClient.StopListening();
            send("JOIN#");
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
                game.processMsg(e.Data);
                updateInterface();
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
            send(txtSend.Text);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            networkClient.StopListening();
        }

        private void txtConsole_TextChanged(object sender, EventArgs e)
        {

        }

        

        public void updateInterface()
        {
            //New method
            GraphicsManager g = new GraphicsManager(panelGrid.CreateGraphics(),panelGrid.Height,panelGrid.Width);
            g.draw(game);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            game.processMsg(txtMsg.Text);
            updateInterface();
        }

        public void send(string data)
        {
            networkClient.Send(data);
            txtConsole.AppendText(data);
            txtConsole.AppendText("\n");
        }

        private void panelGrid_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
