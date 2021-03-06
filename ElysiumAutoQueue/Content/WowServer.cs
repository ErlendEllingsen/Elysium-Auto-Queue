﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElysiumAutoQueue.Content
{
    class WowServer
    {

        public string name;
        public string elysiumStatusIdentifier = ""; 

        public bool queueAvailable = true; //0 in queue still means "available" (true). false = error
        public int queue = 0;
        public DateTime last_updated;
        
        public WowServer(string name, string elysiumStatusIdentifier)
        {
            this.name = name;
            this.elysiumStatusIdentifier = elysiumStatusIdentifier;
        }

        public void update(int queue, bool queueAvailable = true)
        {
            this.queue = queue;
            this.queueAvailable = queueAvailable;
            this.last_updated = DateTime.Now;
        }

    }
}
