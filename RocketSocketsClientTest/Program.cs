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
                
                client.Emit("fuck", "message1");
                Console.WriteLine(client.GetID());

                
            });
            client.Listen("shit", (data) =>{
                Console.WriteLine(data);

            });

            client.StartAsync().Wait();
            Console.ReadLine();
           
        }
    }
}
