using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using RocketSockets.SocketEvents;


namespace RocketSockets{

    public class Utils{
        /*
         * Generates a id stream containing all 
         * alphanumeric characters with underscores and minuses 
         */
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
    
    
    public class ServerSocket{
        public string id;
        private Server server;
        private TcpClient client;
        public String ip;
        private NetworkStream network_stream;
        private Dictionary<string, Action<string,ServerSocket>> callbacks;
        private Dictionary<string, Object> meta;
        public ServerSocket(
            TcpClient _client,
            Server _server,
            Dictionary<string, Action<string, ServerSocket>> _callbacks){

            callbacks = _callbacks;
            
            //Generates and ID for socket indexing
            id = Utils.GenerateID(32);
            
            server = _server;
            client = _client;
            
            ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            //Listens for messages and will later respond to events and handle events
            GetNetworkStream();
            ListenForMessages();


        }
        /*
         * Gets the network stream from the current TcpClient object
         */
        private void GetNetworkStream()
        {
            network_stream = client.GetStream();

        }
        /*
         * Sends a message with a buffer size of 0x4000
         */
         //TODO: make buffer sizes variable
        private async Task SendMessage(String _message) {
            Console.WriteLine(_message);
            byte[] message_length_bytes = new byte[0x2];
            //converts the 16 bit message length to two bytes
            message_length_bytes[0] = (byte)(_message.Length & 0xff);
            message_length_bytes[1] = (byte)(_message.Length & 0xff00 >> 8);

            /*
             * Sends one request to send the message length and
             * then receives a okay signal from the receiver
             * after all this, it will then send the message
             */
            await network_stream.WriteAsync(message_length_bytes, 0, 0x2);
           
            byte[] message_bytes = new byte[_message.Length];
          
                
            for (int i = 0; i < _message.Length; i++)
            {

                message_bytes[i] = (byte)_message[i];
            }
            await network_stream.WriteAsync(message_bytes, 0, _message.Length);


        


        }
        /*
         * listens for messages and processes the event listener
         * from the callbacks dictionary
         */
        private async Task ListenForMessages(){
            //TODO: create a while loop for continuous reading from the buffer.
            //TODO: create event lookup and event handler
            //Creates the network stream to allow for back and forth communication
            while (true){

                byte[] message_length_byte = new byte[0x2];
                //Receives the message length
                await network_stream.ReadAsync(message_length_byte, 0, 0x2);
                int message_length = (message_length_byte[0] + (message_length_byte[1] >> 8));

                byte[] message_bytes = new byte[message_length];
                //Receives the message payload
                await network_stream.ReadAsync(message_bytes, 0, message_length);

                string message = "";
                for (int i = 0; i < message_length; i++)
                {
                    message += (char)message_bytes[i];
                }
                //Splits the message receieved by the client by the delimiter 0x1
                String[] client_message_split = message.Split((char)0x1);
                String client_event = client_message_split[0];
                String client_message = client_message_split[1];
                
                callbacks[client_event](client_message,this);

            }

        }
        /*
         * Emits a message to the client with the event string 
         * attached via a unalphanumeric character 0x1
         */
        public void Emit(String _event, String _message) {
            //Sends the message with the event and message concatenated together
            SendMessage(_event + (char)0x1 + _message);
        }

    }
    public class Server{
        public Dictionary<string,ServerSocket> sockets;
        private TcpListener server;
        private String ip;
        private int port;
        public bool isRunnning = false;
        private Dictionary<string, Action<string,ServerSocket>> callbacks;
        public Server(String ip,int port){
            this.ip = ip;
            this.port = port;
            callbacks = new Dictionary<string, Action<string,ServerSocket>>();
            sockets = new Dictionary<string, ServerSocket>();
            server = new TcpListener(IPAddress.Parse(this.ip),port);
            
        }
        /*
         * waits for clients to connect to the server and 
         * then adds them to the socket list
         */
        private async Task ListenForClientsAsync(){
            TcpClient client = await server.AcceptTcpClientAsync();
            ServerSocket socket = new ServerSocket(client,this,callbacks);
            socket.Emit(Events.ClientID, socket.id);
            socket.Emit(Events.ClientConnected, "connected");


            //adds the newely created socket to the list of sockets so that we can later index the socket
            sockets[socket.id] = socket;

        }
        //---------------------------------
        //public methods
        //---------------------------------

        /*
         * Creates a async main loop for the server
         * client listener
         */
        public async Task StartAsync(){
            server.Start();
            isRunnning = true;
            while (isRunnning){
                await ListenForClientsAsync();

            }
        }
        /*
         * Creates a new event listener for any client that emits
         * to the server. 
         * This event listener will have a Action<string> callback 
         */
        public void Listen(String _event,Action<String,ServerSocket> _callback) {
            
            callbacks[_event] = _callback;
        }
        /*
         * Sends a event to all clients connected to the server
         */
        public void Emit(String _event, String _message) {

            foreach (var socket in sockets.Keys)
            {
                sockets[socket].Emit(_event, _message);
            }
        }
        public ServerSocket GetClient(String _id) {
            return sockets[_id];
        }
        
    }
}
