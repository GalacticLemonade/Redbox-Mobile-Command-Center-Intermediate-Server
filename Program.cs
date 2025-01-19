using Newtonsoft.Json;
using System.Net.Sockets;

namespace Redbox_Mobile_Command_Center_Intermediate_Server {

    public class KioskRow {
        public int KioskID { get; set; }
    }

    class Program {
        static TCPServer server;
        static TCPClient client;
        static List<KioskRow> kiosksTable;
        static Dictionary<TcpClient, int> currentKioskDictionary = new Dictionary<TcpClient, int>();

        static readonly Dictionary<int, string> kioskAddrMap = new Dictionary<int, string>
        {
            { 35618, "192.168.1.123:11600" }
        };

        static void Main(string[] args) {

            kiosksTable = new List<KioskRow> {
                new KioskRow { KioskID = 35618 }
            };

            server = new TCPServer("0.0.0.0", 11500);
            server.Start();

            StartIP();

            // Prevent closing of application
            while (true) { }
        }

        public static async void StartIP() {
            client = new TCPClient();
        }

        public async static Task<string> OnServerIncomingData(string message, TcpClient IncomingClient) {

            string[] arguments = message.Split(' ');

            switch (arguments[0]) {
                case "get-all-kiosks":

                    string kioskJsonTable = JsonConvert.SerializeObject(kiosksTable, Formatting.None);

                    return kioskJsonTable; // get kiosks from db(?)
                case "switch-to-kiosk":
                    int KioskID = Int32.Parse(arguments[1]);

                    if (!kioskAddrMap.ContainsKey(KioskID)) {
                        return "503";
                    }

                    string addrport = kioskAddrMap[KioskID];

                    //ping kiosk
                    await client.ConnectAsync(addrport.Split(":")[0], Int32.Parse(addrport.Split(":")[1]));
                    await client.SendMessageAsync("ping-kiosk");
                    string response = await client.ReceiveMessageAsync();
                    Console.WriteLine(response);
                    if (response != "200") {
                        return "503";
                    }

                    currentKioskDictionary[IncomingClient] = KioskID;

                    return "200";
                case "select-no-kiosk":
                    currentKioskDictionary.Remove(IncomingClient);
                    return "200";
                case "execute-kiosk-command":
                    string Command = arguments[1];
                    string slot = arguments[2];
                    string deck = arguments[3];

                    if (!currentKioskDictionary.ContainsKey(IncomingClient)) {
                        return "503";
                    }

                    int RDBXKioskID = currentKioskDictionary[IncomingClient];

                    if (!kioskAddrMap.ContainsKey(RDBXKioskID)) {
                        return "503";
                    }

                    string RDBXaddrport = kioskAddrMap[RDBXKioskID];

                    await client.ConnectAsync(RDBXaddrport.Split(":")[0], Int32.Parse(RDBXaddrport.Split(":")[1]));

                    await client.SendMessageAsync("execute-command " + Command + " " + slot + " " + deck);

                    string res = await client.ReceiveMessageAsync();
                    return res;
            }

            //await client.SendMessageAsync(message);
            //string response = await client.ReceiveMessageAsync();
            //return response;

            return String.Empty;
        }

        public static void OnDisconnect(TcpClient IncomingClient) {
            currentKioskDictionary.Remove(IncomingClient);
        }
    }
}
