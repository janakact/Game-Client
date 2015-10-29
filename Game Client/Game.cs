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
        private string[,] grid;
        private String [] players;

        public Game()
        {
            InitializeComponent();
            players = new String[5];
            grid = new string[10, 10];
            for(int i =0; i<10;i++)
            {
                for (int j = 0; j < 10; j++)
                    grid[i, j] = "N";

      
            }
            //grid[1, 2] = Constant.WATER;
            //grid[5, 3] = Constant.STONE;
            //grid[9, 9] = Constant.BRICK;

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
            //To Pani - update the grid[] as required.
            //This is the parser. add if conditions to identify messages and do the required process

            if(data.Length<2) //Pre test for invalid messages :: Improve the condition
            {
                //Invalid message
                return;
            }

            if (data.Substring(0,2)=="I:")
            {
                //Game init 
                String[] arr = data.Split(':');

                //Add players---------------------



                //Add bricks,stones,water------------
                String[] brickCordinates = arr[2].Split(';');
                for (int i = 0; i < brickCordinates.Length; i++) {
                    int x = int.Parse(brickCordinates[i][0].ToString());
                    int y = int.Parse(brickCordinates[i][2].ToString());
                    grid[x, y] = Constant.BRICK;
                }

                String[] stoneCordinates = arr[3].Split(';');
                for (int i = 0; i < stoneCordinates.Length; i++)
                {
                    int x = int.Parse(stoneCordinates[i][0].ToString());
                    int y = int.Parse(stoneCordinates[i][2].ToString());
                    grid[x, y] = Constant.STONE;
                }

                String[] waterCordinates = arr[4].Split(';');
                for (int i = 0; i < waterCordinates.Length; i++)
                {
                    int x = int.Parse(waterCordinates[i][0].ToString());
                    int y = int.Parse(waterCordinates[i][2].ToString());
                    grid[x, y] = Constant.WATER;
                }

            }
            else if(data==Constant.S2C_GAMESTARTED)
            {
                //Game started
            }
            //Add others
            else
            {
                //Messages which can't be recognized
            }

            updateInterface();

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
                    if (grid[i, j] == Constant.WATER) b = brushWater;
                    if (grid[i, j] == Constant.STONE) b = brushStone;
                    if (grid[i, j] == Constant.BRICK) b = brushBrick;
                    formGraphics.FillRectangle(b, new Rectangle(i * (width) + offsetX, j * height + offsetY,width-5, height-5));

                }
            }
            //pen.Dispose();
            formGraphics.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            processRecievedMsg(txtMsg.Text);
        }
    }
}
