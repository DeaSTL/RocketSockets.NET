using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            String _message = "";
            for (int i = 0; i < 65000; i++) {
                _message += "6";

            }
            byte[] bytes = new byte[0x2];
            bytes[0] = (byte)(_message.Length & 0xff);
            bytes[1] = (byte)((_message.Length & 0xff00 ) >> 8);
            
            Console.WriteLine((bytes[0] + (bytes[1] << 8)));
            
            Console.ReadLine();
            
        }

    }
}
