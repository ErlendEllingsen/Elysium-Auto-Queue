using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElysiumAutoQueue.Content
{
    class GameStateCriteria
    {
        public string name;
        public int gameX, gameY;
        public List<string> acceptedColors = new List<string>();
        public bool requiresMouseHover = false;



        public GameStateCriteria(string name, int gameX, int gameY, string acceptedColors, bool requiresMouseHover = false)
        {
            this.name = name;
            this.gameX = gameX;
            this.gameY = gameY;



            this.acceptedColors = acceptedColors.Split(',').ToList();

            this.requiresMouseHover = requiresMouseHover;
        }

    }
}
