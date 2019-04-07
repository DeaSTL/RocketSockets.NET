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
                Console.WriteLine(data);
                
            });
            client.StartAsync().Wait();
           
        }
    }
}
