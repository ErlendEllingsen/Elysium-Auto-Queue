using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElysiumAutoQueue.Content
{
    class WaitingIncidentMonitor
    {
        //15 minute is deadline for incidents...
        private static int incident_deadline = 15 * 60; //in seconds


        public static List<DateTime> incidents = new List<DateTime>();

        public static void addIncident()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Waiting incident logged at " + DateTime.Now.ToString() + ".");
            Console.ForegroundColor = ConsoleColor.White;

            incidents.Add(DateTime.Now);
            clearIncidents();   
        }

        public static bool isLogonUnstable()
        {
            clearIncidents();
            if (incidents.Count > 5) return true; 
            return false; 
        }

        public static void clearIncidents()
        {

            int incidentsPre = incidents.Count;
            

            List<DateTime> newList = new List<DateTime>();

            for (int i = 0; i < incidents.Count; i++)
            {
                DateTime incident = incidents[i];
                TimeSpan ts = (DateTime.Now.Subtract(incident));
                if (ts.TotalSeconds >= incident_deadline) continue;
                newList.Add(incident); 

            }

            incidents = newList;

            int incidentsPost = incidents.Count;

            if (incidentsPre != incidentsPost)
            {
                Console.WriteLine("Pre-cleanup: Currently " + incidentsPre + " incidents");
                Console.WriteLine("Post-cleanup: Currently " + incidentsPost + " incidents");
            }
            

        }

    

    }
}
