﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ElysiumAutoQueue.Content
{
    class DataSender
    {

        public static System.Threading.Timer dataSendTimer;

        public static string endpoint_live = "https://elysiumstatus.com/auto-queue-update";
        public static string endpoint_dev = "http://10.0.0.13:8080/auto-queue-update";
        public static string endpoint = endpoint_live; //Adjusted by config.

        public static async void sendData()
        {
            DataSender.endpoint = ProgramConfig.config.getEndpoint();

            Console.WriteLine("Sending data...");

            //Fetch password
            string password_autoqueue = ProgramConfig.config.password_autoqueue;


            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                { "password", password_autoqueue },
                { "autoqueue", OutConfig.export() }
                };

                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(endpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();

                //Send response 
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Response: " + responseString);
                Console.ForegroundColor = ConsoleColor.White;

            }
        }

        public static void TimerCallback(Object o)
        {
            DataSender.sendData();

            GC.Collect();
        }
    }
}
