using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Game_Client
{
    class GraphicsManager
    {
        Graphics graphics;
        int height;
        int width;
        public GraphicsManager(Graphics g, int height, int width)
        {
            graphics = g;
            this.height = height;
            this.width = width;
        }

        public void draw(Game game)
        {
            int offsetX = 0,
                offsetY = 0,
                blockHeight =  height/10,
                blockWidt =  width/10;



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
                    graphics.FillRectangle(b, new Rectangle(i * (blockWidt) + offsetX, j * blockHeight + offsetY, blockWidt - 5, blockHeight - 5));

                }
            }
            graphics.Dispose();
        }
    }
}
