using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Redbox_Mobile_Command_Center_Intermediate_Server {
    public class TCPServer {
        private TcpListener _listener;

        public TCPServer(string ipAddress, int port) {
            _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
        }

        public void Start() {
            _listener.Start();
            Console.WriteLine($"Server started on {_listener.LocalEndpoint}. Waiting for clients...");
            AcceptClientsAsync();
        }

        public void Stop() {
            _listener.Stop();
            Console.WriteLine("Server stopped.");
        }

        private async void AcceptClientsAsync() {
            while (true) {
                try {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");
                    HandleClientAsync(client);
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                    break;
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client) {
            using (client) {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                try {
                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Received: {message}");

                        // echo the message back to the client
                        string response = $"Server received: {message}";
                        byte[] responseData = Encoding.UTF8.GetBytes(response);
                        await stream.WriteAsync(responseData, 0, responseData.Length);
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error handling client: {ex.Message}");
                }
                finally {
                    Console.WriteLine($"Client disconnected: {client.Client.RemoteEndPoint}");
                }
            }
        }
    }
}
