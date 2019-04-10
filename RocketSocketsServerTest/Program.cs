using System;
using RocketSockets;

namespace RocketSocketsServerTest{
    class Program{
        static void Main(string[] args){
            
            Server server = new Server("127.0.0.1", 5000);
            
            Console.WriteLine("Server is Running....");
            server.Listen("server", (data,socket) =>{
                Console.WriteLine(data);
                
                socket.Emit("hello",socket.ip);

            });
            server.StartAsync().Wait();
            
        }
    }
}
