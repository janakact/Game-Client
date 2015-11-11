using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Client
{
    class Player
    {
        public String name;
        public int locationX;
        public int locationY;
        public int health;
        public int coins;
        public int isShot;
        public int direction;
        public int points;
    }

    class LifePack
    {
        public int xCordinate;
        public int yCordinate;
        public int lifeTime;
    }

    class Coin
    {
        public int value;
        public int lifeTime;
        public int xCordinate;
        public int yCordinate;

    }




}
