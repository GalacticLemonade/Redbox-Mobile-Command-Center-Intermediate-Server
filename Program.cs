namespace Redbox_Mobile_Command_Center_Intermediate_Server {
    internal class Program {
        static void Main(string[] args) {
            TCPServer server = new TCPServer("0.0.0.0", 11500);
            server.Start();



            // Prevent closing of application
            while (true) { }
        }
    }
}
