using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElysiumAutoQueue.Content
{
    class GameState
    {
        public string name;
        public List<GameStateCriteria> criterias = new List<GameStateCriteria>();

        public GameState(string name)
        {
            this.name = name;
        }

    }

    

}
