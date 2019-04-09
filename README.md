# RocketSockets.NET
A fast, easy, real-time, socket library.

# Mission Statement
This is library is alternative to other socket libraries


## Client
```csharp
using System;
using RocketSockets;
using RocketSockets.SocketEvents;


namespace RocketSocketsClientTest{
    class Program{
        static void Main(string[] args){
            Client client = new Client("127.0.0.1",5000);
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
            
            Server server = new Server("127.0.0.1", 5000);
            
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
## Planned Features
- Rooms
    - Room joining
    - Room combination
- Variable buffer lengths
- Javascript server and client support
- Java server and client support
- C++ server and client support
- Python server and client support
- All versions will be compatible with each other's servers and clients



