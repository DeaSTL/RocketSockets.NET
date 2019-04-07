using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace RocketSockets{
    public class Utils{

        public static String GenerateID(int length){
            String alpha_chars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm_-1234567890";

            List<char> random_chars = new List<char>();
            for (int i = 0; i < alpha_chars.Length; i++){
                random_chars.Add(alpha_chars[i]);
            }
            

            Random rand = new Random();
            String output = "";
            
            for (int i = 0; i < length; i++){
                
                output += random_chars[rand.Next(0,random_chars.Count)];

            }
            return output;

        }



    }
    public class ServerSocket
    {
        public string Id;
        private Server server;
        private TcpClient client;
        private NetworkStream networkStream;
        private Dictionary<string, Action<string>> callbacks;
        public ServerSocket(TcpClient client, Server server)
        {
            //Generates and ID for socket indexing
            Id = Utils.GenerateID(32);
            this.server = server;
            this.client = client;
            //listens for messages and will later respond to events and handle events
            GetNetworkStream();
            ListenForMessages();


        }
        private void GetNetworkStream()
        {
            networkStream = client.GetStream();

        }
        private async Task SendMessage(String _message) {
            byte[] bytes = new byte[0x4000];

            for (int i = 0; i < _message.Length; i++) {
                bytes[i] = (byte)_message.ToCharArray()[i];
            }



            await networkStream.WriteAsync(bytes,0,0x4000);


        }
        private async Task ListenForMessages()
        {
            //TODO: create a while loop for continuous reading from the buffer.
            //TODO: create event lookup and event handler
            //creates the network stream to allow for back and forth communication
            while (true)
            {

                byte[] bytes = new byte[16384];

                //reads from the current stream
                await networkStream.ReadAsync(bytes, 0, 16384);
                string message = "";
                int iter = 0;
                while (true)
                {
                    //checks for the character 0x0 and then stops reading from the buffer
                    if (bytes[iter] == 0)
                    {
                        break;
                    }
                    message += (char)bytes[iter];
                    iter++;

                }
                //splits the message receieved by the client by the delimiter 0x1
                String[] client_message_split = message.Split((char)0x1);
                String client_event = client_message_split[0];
                String client_message = client_message_split[1];
                Console.WriteLine(client_event);
                Console.WriteLine(client_message);

            }

        }
        public void Emit(String _event, String _message) {
            //sends the message with the event and message concatenated together
            SendMessage(_event + (char)0x1 + _message);
        }
        public void Listen(String _event,Action<String> _callback) {
            callbacks.Add(_event, _callback);

        }
    }
    public class Server{
        public List<ServerSocket> sockets;
        private TcpListener server;
        private String Ip;
        private int Port;
        public bool isRunnning = false;
        public Server(String ip,int port){
            this.Ip = ip;
            this.Port = port;
            sockets = new List<ServerSocket>();
            server = new TcpListener(IPAddress.Parse(this.Ip),port);
            server.Start();
        }
        //waits for clients to connect to the server and then adds them to the socket list
        private async Task ListenForClientsAsync(){
            TcpClient client = await server.AcceptTcpClientAsync();
            ServerSocket socket = new ServerSocket(client,this);
            
            //adds the newely created socket to the list of sockets so that we can later index the socket
            sockets.Add(socket);

        }
        //Initializer for the server
        public async Task StartAsync(){
            isRunnning = true;
            while (isRunnning){
                await ListenForClientsAsync();

            }
        }
        
        
    }
}
