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
    public partial class Game : Form
    {
        private NetworkClient networkClient;
        private string[,] grid;
        private Player[] players;// = new List<Player>();
        private Player thisPlayer;
        private List<Coin> coins;
        private List<LifePack> lifePacks;

        public Game()
        {
            InitializeComponent();

            players = new Player[5];
            thisPlayer = new Player();
            coins = new List<Coin>();
            lifePacks = new List<LifePack>();

            //initializing the list
            for (int i = 0; i < 5; i++) { players[i] = new Player(); }

            grid = new string[10, 10];
            
            for(int i =0; i<10;i++)
            {
                for (int j = 0; j < 10; j++)
                    grid[i, j] = "N";

            }
            
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
            send(txtSend.Text);
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
                String[] arr = data.Split(':','#');

                //Player details---------------------
                thisPlayer.name = arr[1];
                players[int.Parse(arr[1][1].ToString())]= thisPlayer;
                
                //Console.WriteLine(thisPlayer.name);
                //Console.WriteLine(players[1].name);


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
            //Console.WriteLine(thisPlayer.name);
            if (data.Substring(0, 2) == "S:")
            {
                String[] arr = data.Split(':',';','#' );

                thisPlayer.locationX = int.Parse(arr[2][0].ToString());
                thisPlayer.locationY = int.Parse(arr[2][2].ToString());
                thisPlayer.direction = int.Parse(arr[3][0].ToString());

               
            }

            if (data.Substring(0, 2) == "C:")
            {
                String[] arr = data.Split(':','#');

                Coin c = new Coin();
                c.xCordinate = int.Parse(arr[1][0].ToString());
                c.yCordinate = int.Parse(arr[1][2].ToString());
                c.lifeTime = int.Parse(arr[2]);
                c.value = int.Parse(arr[3]);

                coins.Add(c);
            }

            if (data.Substring(0, 2) == "L:")
            {
                String[] arr = data.Split(':', '#');
                LifePack l = new LifePack();
                l.xCordinate = int.Parse(arr[1][0].ToString());
                l.yCordinate = int.Parse(arr[1][2].ToString());
                l.lifeTime = int.Parse(arr[2]);

                lifePacks.Add(l);
            }


            if (data.Substring(0, 2) == "G:")
            {
                String[] arr = data.Split(':','#');

                for (int i = 0; i < arr.Length; i++) {
                    if (arr[i][0] == 'P')
                    {
                        String[] details = arr[i].Split(';');
                        int j = int.Parse(details[0][1].ToString());
                            if (j <5 && j>=0 )
                            {
                                if (players[j] == null)
                                    players[j] = new Player();
                                players[j].name = details[0];
                                players[j].locationX = int.Parse(details[1][0].ToString());
                                players[j].locationY = int.Parse(details[1][2].ToString());
                                players[j].direction = int.Parse(details[2][0].ToString());
                                players[j].isShot = int.Parse(details[3][0].ToString());
                                players[j].health = int.Parse(details[4]);
                                players[j].coins = int.Parse(details[5]);
                                players[j].points = int.Parse(details[6]);
                               
                            }
                    }

                }
                

            }


            else if (data == Constant.S2C_GAMESTARTED)
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
            int offsetX = 30,
                offsetY = 300;
            System.Drawing.Pen pen;
            pen = new System.Drawing.Pen(System.Drawing.Color.Red);
            System.Drawing.Graphics formGraphics = this.CreateGraphics();
            formGraphics.DrawLine(pen, 0, 0, 200, 200);

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
                    formGraphics.FillRectangle(b, new Rectangle(i * 20 + offsetX, j * 20 + offsetY, 10, 10));

                }
            }
            pen.Dispose();
            formGraphics.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            processRecievedMsg(txtMsg.Text);
        }

        public void send(string data)
        {
            networkClient.Send(data);
            txtConsole.AppendText(data);
            txtConsole.AppendText("\n");
        }
    }
}
