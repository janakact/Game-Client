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
            int offsetX = 0,
                offsetY = 0,
                height = panelGrid.Height / 10,
                width = panelGrid.Width/10;
          
            //System.Drawing.Pen pen;
            //pen = new System.Drawing.Pen(System.Drawing.Color.Red);
            System.Drawing.Graphics formGraphics = panelGrid.CreateGraphics();
            //formGraphics.DrawLine(pen, 0, 0, 200, 200);

            System.Drawing.SolidBrush brushEmpty = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            System.Drawing.SolidBrush brushWater = new System.Drawing.SolidBrush(System.Drawing.Color.CadetBlue);
            System.Drawing.SolidBrush brushStone = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.SolidBrush brushBrick = new System.Drawing.SolidBrush(System.Drawing.Color.Brown);
            
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Brush b = brushEmpty;
                    if (game.grid[i, j] == Constant.WATER) b = brushWater;
                    if (game.grid[i, j] == Constant.STONE) b = brushStone;
                    if (game.grid[i, j] == Constant.BRICK) b = brushBrick;
                    formGraphics.FillRectangle(b, new Rectangle(i * (width) + offsetX, j * height + offsetY,width-5, height-5));

                }
            }
            //pen.Dispose();
            formGraphics.Dispose();
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
    }
}
