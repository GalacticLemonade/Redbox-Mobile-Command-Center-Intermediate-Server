﻿namespace Redbox_Mobile_Command_Center_Intermediate_Server {
    class Program {
        static TCPServer server;
        static TCPClient client;
        static int onKiosk = 0;

        static void Main(string[] args) {
            server = new TCPServer("0.0.0.0", 11500);
            server.Start();

            StartIP();

            // Prevent closing of application
            while (true) { }
        }

        public static async void StartIP() {
            client = new TCPClient();
        }

        public async static Task<string> OnServerIncomingData(string message) {

            string[] arguments = message.Split(' ');

            switch (arguments[0]) {
                case "get-all-kiosks":
                    return "35618"; // get kiosks from db(?)
                case "switch-to-kiosk":
                    int KioskID = Int32.Parse(arguments[1]);

                    if (KioskID != 35618) {
                        return "500-kiosk not found";
                    }

                    break;
                    /*
                case "run-on-kiosk":
                    int KioskID = Int32.Parse(arguments[1]);
                    string Command = arguments[2];

                    if (KioskID == 35618) {
                        await client.ConnectAsync("192.168.1.123", 11600);

                        await client.SendMessageAsync(Command);

                        string response = await client.ReceiveMessageAsync();
                        Console.WriteLine(response);
                        return response;
                    }

                    break;
                    */
            }

            //await client.SendMessageAsync(message);
            //string response = await client.ReceiveMessageAsync();
            //return response;

            return String.Empty;
        }
    }
}
