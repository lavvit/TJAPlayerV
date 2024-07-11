using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SeaDrop
{
    public class Udp
    {
        public static string HostIP = "";
        public static int HostPort;
        public static IPAddress? Address;
        public static UdpClient? Listener = null;
        public static List<string> Sends = new List<string>();

        #region Server
        public static void OpenServer(string ip = "127.0.0.1", int port = 3000)
        {
            HostIP = ip;
            HostPort = port;

            Address = IPAddress.Parse(HostIP);
            Listener = new UdpClient(new IPEndPoint(IPAddress.Parse(HostIP), HostPort));
            Console.WriteLine("Udp Listenを開始しました({0}:{1})。",
                HostIP,
                HostPort);
        }
        public static void EndServer()
        {
            Listener?.Close();
            Listener = null;
            Console.WriteLine("Udp Listenerを閉じました。");
        }
        public static void UpdateServer()
        {
            if (Listener == null) return;
            try
            {
                //データを受信する
                IPEndPoint? remoteEP = null;
                byte[] rcvBytes = Listener.Receive(ref remoteEP);

                //データを文字列に変換する
                string rcvMsg = Encoding.UTF8.GetString(rcvBytes);
                //受信したデータと送信者の情報を表示する
                Console.WriteLine($"受信したデータ:{rcvMsg}");
                Console.WriteLine($"送信元アドレス:{remoteEP.Address}:{remoteEP.Port}");

                //末尾の\nを削除
                rcvMsg = rcvMsg.TrimEnd('\n');
                Sends.Add(rcvMsg);
                Console.WriteLine(rcvMsg);
            }
            catch (Exception e) { Console.Write(e.ToString()); }
        }
        #endregion

        #region Client
        public static void SendClient(string str)
        {
            try
            {
                //送信するデータを作成する
                UdpClient udp = new UdpClient(HostIP, HostPort);
                byte[] sendBytes = Encoding.UTF8.GetBytes(str);

                //リモートホストを指定してデータを送信する
                udp.Send(sendBytes);
                udp.Close();
            }
            catch (Exception e) { Console.Write(e.ToString()); }
        }
        #endregion
    }
}
