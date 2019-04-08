using System;
using RocketSockets;

namespace RocketSocketsServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Server server = new Server("0.0.0.0", 5000);
            
            Console.WriteLine("Server is Running....");
            server.Listen("fuck", (data,socket) =>
            {
                Console.WriteLine(data);
                Console.WriteLine(socket.id);
                socket.Emit("shit",socket.ip);

            });
            server.StartAsync().Wait();
            
        }
    }
}
