using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ClassLibHeartbeat
{
    public class HearthbeatSend
    {
        private volatile bool _shouldStop;
        public volatile string message = "beat";
        public volatile int waitinms = 500;
        public void DoWork()
        {
            while (!_shouldStop)
            {
                //SendUDPMessage(message);
                SendUDPMulticastMessage(message);
                Thread.Sleep(waitinms);
            }
        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        int port;
        public HearthbeatSend(int sendport)
        {
            port = sendport;
        }
        public void SendUDPMulticastMessage(string nachricht)
        {
            Socket mSocket1 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint mIPEP = new IPEndPoint(IPAddress.Parse("224.100.0.1"), port); //Multicast sendet an alle Mitglieder der Gruppe
            System.Threading.Thread.Sleep(3000);
            byte[] mSendBuffer = Encoding.ASCII.GetBytes(string.Format(nachricht));

            //SocketOption sind damit da, dass man über die Router kommt, Default-Wert von TTL wäre 1, wir setzen ihn auf 50
            mSocket1.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse("224.100.0.1")));
            mSocket1.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 50);
            mSocket1.SendTo(mSendBuffer, mIPEP);

            mSocket1.Close();
        }
        public void SendUDPMessage(string nachricht)
        {
            Socket mSocket = null;
            IPEndPoint mEPGlobal = null;
            //IPEndPoint mEPLocal = null;
            byte[] mSendBufferGlobal = null;
            //byte[] mSendBufferLocal = null;

            //Findet die erste IPv4 Adresse meines Rechners mit LINQ
            IPAddress mLocalIPAddress = Dns.GetHostEntry(IPAddress.Parse("127.0.0.1")).AddressList.Where(p => p.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();

            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mEPGlobal = new IPEndPoint(IPAddress.Broadcast, 9050); //IPAddress.Broadcast = 255.255.255.255

            mSendBufferGlobal = Encoding.ASCII.GetBytes(string.Format(nachricht));
            //mSendBufferLocal = Encoding.ASCII.GetBytes(string.Format("This is a local broadcast from {0}", Dns.GetHostName()));

            mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true); //Muss sein, schaltet Broadcast ein (also dass wir es WIRKLICH WOLLEN)

            //Gibt dem Empfänger eine Chance zu starten/hochzufahren. Andersfalls geht die Nachricht nur ins Leere
            //System.Threading.Thread.Sleep(3000);
            mSocket.SendTo(mSendBufferGlobal, mEPGlobal);

            mSocket.Close();
        }
    }
}
