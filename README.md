# RocketSockets.NET
A fast, easy, real-time, socket library.

## Client
```csharp
using System;
using RocketSockets;
using RocketSockets.SocketEvents;


namespace RocketSocketsClientTest{
    class Program{
        static void Main(string[] args){
            Client client = new Client("localhost",5000);
            client.Listen(Events.ClientConnected, (data) =>{
                
                client.Emit("hello", "message");
                Console.WriteLine(client.GetID());

                
            });
            client.Listen("serverevent", (data) =>{
                Console.WriteLine(data);
            });

            client.StartAsync().Wait();   
        }
    }
}
```
## Server
```csharp
using System;
using RocketSockets;

namespace RocketSocketsServerTest{
    class Program{
        static void Main(string[] args){
            
            Server server = new Server("localhost", 5000);
            
            Console.WriteLine("Server is Running....");
            
            server.Listen("hello", (data,socket) =>{
                Console.WriteLine(data);
                socket.Emit("serverevent","hello client");

            });
            server.StartAsync().Wait();
            
        }
    }
}

```
