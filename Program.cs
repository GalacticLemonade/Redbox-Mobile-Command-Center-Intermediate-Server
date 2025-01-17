namespace Redbox_Mobile_Command_Center_Intermediate_Server {
    class Program {
        static TCPServer server;
        static TCPClient client;

        static void Main(string[] args) {
            server = new TCPServer("0.0.0.0", 11500);
            server.Start();

            //StartIP();

            // Prevent closing of application
            while (true) { }
        }

        public static async void StartIP() {
            client = new TCPClient();
            await client.ConnectAsync("192.168.1.123", 11600);
        }

        public async static Task<string> OnServerIncomingData(string message) {

            //await client.SendMessageAsync(message);
            //string response = await client.ReceiveMessageAsync();
            //return response;

            return String.Empty;
        }
    }
}
