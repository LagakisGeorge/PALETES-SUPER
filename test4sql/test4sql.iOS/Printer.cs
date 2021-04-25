using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly :Dependency(typeof(test4sql.iOS.Printer ))]
namespace test4sql.iOS
{
    class Printer:iPrinter 
    {

        public void Print(string ipAddress, int port, IList<string> lineToPrint)
        {
            Socket pSocket = new Socket(SocketType.Stream, ProtocolType.IP);
            pSocket.SendTimeout = 1500;
            pSocket.Connect(ipAddress, port);
            List<byte> outputList = new List<byte>();
            foreach (string txt in lineToPrint)
            {
                outputList.AddRange(System.Text.Encoding.UTF8.GetBytes(txt));
                outputList.Add(0x0A); ;
            }
            pSocket.Send(outputList.ToArray());
            pSocket.Close();


        }
    }
}