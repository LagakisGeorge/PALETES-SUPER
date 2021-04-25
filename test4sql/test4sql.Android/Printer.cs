using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

[assembly:Dependency(typeof(test4sql.Droid.Printer ))]
namespace test4sql.Droid
{
    public class Printer : iPrinter
    {
        public void Print(string ipAddress, int port, IList<string> lineToPrint)
        {
            Socket pSocket = new Socket(SocketType.Stream, ProtocolType.IP);
            pSocket.SendTimeout = 1500;
            pSocket.Connect(ipAddress, port);
            List<byte> outputList = new List<byte>();
            foreach (string txt in lineToPrint)
            {
                outputList.AddRange(System.Text.Encoding.Unicode.GetBytes(txt));
                outputList.Add(0x0A); ;
            }
            pSocket.Send(outputList.ToArray());
            pSocket.Close();


        }
    }
}