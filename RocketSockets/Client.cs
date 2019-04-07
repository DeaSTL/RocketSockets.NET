using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RocketSockets{
    public class ClientSocket
    {
        private TcpClient client;
        private NetworkStream network_stream;
        private Dictionary<string, Action<string>> callbacks;
        public ClientSocket(TcpClient _client)
        {
            client = _client;
            GetNetworkStream();

        }

        //TODO: Send Length packet first and then send the message payload for dynamic buffer sizes
        //TODO: Create emit and event listeners

        //---------------------------------------
        //private methods
        //---------------------------------------

        private void GetNetworkStream(){
            network_stream = client.GetStream();
        }
        private async Task ListenForMessages(){
            byte[] bytes = new byte[0x4000];
            await network_stream.ReadAsync(bytes, 0, 0x4000);
            string message = "";
            int iter = 0;
            //Creates a string without all the null characters
            while (true)
            {
                if (bytes[iter] == 0)
                {
                    break;
                }
                message += (char)bytes[iter];
                iter++;
            }

            Console.WriteLine(message);
        }
        private async Task SendMessage(String message){
            byte[] bytes = new byte[0x4000];
            for (int i = 0; i < message.Length; i++)
            {

                bytes[i] = (byte)message.Substring(i).ToCharArray()[0];
            }

            await network_stream.WriteAsync(bytes, 0, 0x4000);
        }

        //---------------------------------------
        //public methods
        //---------------------------------------
        public async Task StartAsync()
        {
            while (true)
            {
                await ListenForMessages();

            }

        }


        public void Emit(String _event, String _message)
        {
            SendMessage("message");


        }
        public void Listen(String _event, Action<string> callback)
        {
            callbacks.Add(_event, callback);

        }

    }
    public class Client{

        private TcpClient client;
        private string host;
        private int port;
        private ClientSocket socket;


        public Client(string _host,int _port){
            host = _host;
            port = _port;
            client = new TcpClient(host, port);
            socket = new ClientSocket(client);
            

        }
        public void Listen(String _event,Action<String> _callback) {
            socket.Listen(_event, _callback);
        }
        public void Emit(String _event,String _message) {
            socket.Emit(_event, _message);
        }
        public async Task StartAsync() {
            await socket.StartAsync();
        }
        
        
    }
}
