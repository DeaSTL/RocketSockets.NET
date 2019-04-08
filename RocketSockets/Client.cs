using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using RocketSockets.SocketEvents;

namespace RocketSockets{
    public class ClientSocket{
        private TcpClient client;
        private NetworkStream network_stream;
        private Dictionary<string, Action<string>> callbacks;
        public String id;
        public ClientSocket(TcpClient _client){
            callbacks = new Dictionary<string, Action<string>>();
            client = _client;
            GetNetworkStream();
            Listen(Events.ClientID, (data) =>{
                id = data;

            });

        }

        //TODO: Send Length packet first and then send the message payload for dynamic buffer sizes
        //TODO: Create emit and event listeners

        //---------------------------------------
        //private methods
        //---------------------------------------

        /*
         * Gets the network stream from the current TcpClient object
         */
        private void GetNetworkStream(){
            network_stream = client.GetStream();
        }
        /*
         * listens for messages and processes the event listener
         * from the callbacks dictionary
         */
        private async Task ListenForMessages(){
            byte[] bytes = new byte[0x4000];
            await network_stream.ReadAsync(bytes, 0, 0x4000);
            string message = "";
            int iter = 0;
            //Creates a string without all the null characters
            while (true){
                if (bytes[iter] == 0){
                    break;
                }
                message += (char)bytes[iter];
                iter++;
            }
            String[] server_message_split = message.Split((char)0x1);
            String server_event = server_message_split[0];
            String server_message = server_message_split[1];
            callbacks[server_event](server_message);
        }
        /*
         * method for sending raw message data
         */
        private async Task SendMessage(String message){
            byte[] bytes = new byte[0x4000];
            for (int i = 0; i < message.Length; i++){

                bytes[i] = (byte)message.Substring(i).ToCharArray()[0];
            }

            await network_stream.WriteAsync(bytes, 0, 0x4000);
        }

        //---------------------------------------
        //public methods
        //---------------------------------------
        public async Task StartAsync(){
            while (true){
                await ListenForMessages();

            }

        }

        /*
         * Emits a message to the client with the event string 
         * attached via a unalphanumeric character 0x1
         */
        public void Emit(String _event, String _message){
            SendMessage(_event + (char)0x1 + _message);


        }
        /*
         * Creates a event for any payload that is sent from the server
         */
        public void Listen(String _event, Action<string> callback){
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
        /*
         * Creates a event for any payload that is sent from the server
         */
        public void Listen(String _event,Action<String> _callback) {
            socket.Listen(_event, _callback);
        }
        /*
         * Gets the id of the client
         */
        public String GetID() {
            return socket.id;
        }
        /*
         * Emits a message to the client with the event string 
         * attached via a unalphanumeric character 0x1
         */
        public void Emit(String _event,String _message) {
            socket.Emit(_event, _message);
        }
        /*
         * starts by connecting and then triggers the message 
         * listening subroutine
         */
        public async Task StartAsync() {
            await socket.StartAsync();
        }
        
        
    }
}
