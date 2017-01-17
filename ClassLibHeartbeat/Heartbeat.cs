using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace ClassLibHeartbeat
{
    public class Heartbeat
    {
        public string lastmsg = "gibs nix";
        int port;
        private volatile bool _shouldStop;
        public DateTime lastrecieve = DateTime.Now;
        public void DoWork()
        {
            while (!_shouldStop)
            {
                //lastmsg = WaitForUDPMessage();
                lastmsg = WaitForMulticastUDPMessage();
                lastrecieve = DateTime.Now;
            }
        }
        public Heartbeat(int receiveport)
        {
            port = receiveport;
        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        public string WaitForMulticastUDPMessage()
        {
            Socket mSocket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint mIPEP2 = new IPEndPoint(IPAddress.Any, port);
            EndPoint mEP2 = (EndPoint)mIPEP2;

            int count = 0;

            byte[] mReceiveBuffer = new byte[1024];

            mSocket2.Bind(mIPEP2); //für das Hören auf einen port
            mSocket2.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse("224.100.0.1")));
            count = mSocket2.ReceiveFrom(mReceiveBuffer, ref mEP2);
            mSocket2.Close();
            return Encoding.ASCII.GetString(mReceiveBuffer, 0, count);
        }
        public string WaitForUDPMessage()
        {
            Socket mSocket = null;
            IPEndPoint mIPEP = null;
            EndPoint mEP = null;
            byte[] recvBuffer = new byte[1024];
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mIPEP = new IPEndPoint(IPAddress.Any, 9050); //ALLE meine IP Adressen, Port so wie bei Sender
            //Socket und EP über bind Verbinden
            mSocket.Bind(mIPEP); //für das Listening notwendig
            mEP = (EndPoint)mIPEP; //nicht notwendig explizit zu casten
            //mSocket.Receive(recvBuffer);
            int counter = 0;
            counter = mSocket.ReceiveFrom(recvBuffer, ref mEP);  //jeder kann uns was senden!
            mSocket.Close();
            return Encoding.ASCII.GetString(recvBuffer);
        }
       
    }
}
