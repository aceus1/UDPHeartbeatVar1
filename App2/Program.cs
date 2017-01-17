using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibHeartbeat;
using System.Threading;

namespace App2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "APP2";
            bool issend = false;
            Heartbeat hb = new Heartbeat(9051);
            HearthbeatSend hbsend = new HearthbeatSend(9050);
            hbsend.message = "running";
            int eingabe = 0;
            Thread empfangThread = new Thread(new ThreadStart(hb.DoWork));
            Thread sendThread = new Thread(new ThreadStart(hbsend.DoWork));
            empfangThread.Start();
            do
            {
                Console.Clear();
                eingabe = menu();
                switch (eingabe)
                {
                    case 1:
                        if (!issend && !sendThread.IsAlive)
                        {
                            sendThread = new Thread(new ThreadStart(hbsend.DoWork));
                            sendThread.Start();
                            Console.WriteLine("Starte");
                            issend = true;
                            hb.lastmsg = "running";
                        }
                        else
                        {
                            Console.WriteLine("Läuft bereits");
                        }
                        Thread.Sleep(500);
                        break;
                    case 2:
                        hbsend.RequestStop();
                        issend = false;
                        Console.WriteLine("Stoppe");
                        hbsend.SendUDPMulticastMessage("idle");
                        Thread.Sleep(500);
                        break;
                    case 3:
                        if ((hb.lastmsg != "shutdown" || hb.lastmsg != "idle") && ((DateTime.Now - hb.lastrecieve).TotalSeconds) > 20)
                        {
                            Console.WriteLine("missing");
                        }
                        else
                        {
                            Console.WriteLine(hb.lastmsg);
                        }
                        Thread.Sleep(500);
                        break;
                    case 4:
                        break;
                    default:
                        Console.WriteLine("Falsche eingabe: " + eingabe);
                        Thread.Sleep(500);
                        break;
                }
            } while (eingabe != 4);
            hb.RequestStop();
            hbsend.RequestStop();
            hbsend.SendUDPMulticastMessage("Shutdown");
        }
        static int menu()
        {
            Console.WriteLine("1. Heartbeat einschalten");
            Console.WriteLine("2. Heartbeat auschalten");
            Console.WriteLine("3. Heartbeat Zustand anzeigen");
            Console.WriteLine("4. Exit");
            try
            {
                return int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                return -1;
            }
        }
    }
}

