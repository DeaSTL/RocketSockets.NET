using System;
using RocketSockets;
using RocketSockets.SocketEvents;


namespace RocketSocketsClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("localhost",5000);
            client.Listen(Events.ClientConnected, (data) =>{
                
                client.Emit("hello", "message");
                Console.WriteLine(client.GetID());

                
            });
            client.Listen("hello", (data) =>{
                client.Emit("hello","hello");

            });

            client.StartAsync().Wait();
           
           
        }
    }
}
