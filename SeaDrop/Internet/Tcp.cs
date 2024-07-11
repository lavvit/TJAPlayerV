using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SeaDrop
{
    public class Tcp
    {
        public static string? HostIP;
        public static int HostPort;
        public static IPAddress? Address;
        public static TcpListener? Listener = null;
        public static List<string> Sends = new List<string>();

        #region Server
        public static void OpenServer(string ip = "127.0.0.1", int port = 2000)
        {
            if (Listener != null) EndServer();
            HostIP = ip;
            HostPort = port;

            Address = IPAddress.Parse(HostIP);
            Listener = new TcpListener(IPAddress.Parse(HostIP), HostPort);
            //Listenを開始する
            Listener.Start();
            Console.WriteLine("Listenを開始しました({0}:{1})。",
                ((IPEndPoint)Listener.LocalEndpoint).Address,
                ((IPEndPoint)Listener.LocalEndpoint).Port);
        }
        public static void EndServer()
        {
            //リスナを閉じる
            Listener?.Stop();
            Listener = null;
            Console.WriteLine("Listenerを閉じました。");
        }
        public static void UpdateServer()
        {
            if (Listener == null) return;
            try
            {
                //接続要求があったら受け入れる
                TcpClient client = Listener.AcceptTcpClient();
                var point = client.Client.RemoteEndPoint;
                var endpoint = point != null ? (IPEndPoint)point : new IPEndPoint(0, 0);
                Console.WriteLine($"クライアント({endpoint.Address}:{endpoint.Port})と接続しました。");

                //NetworkStreamを取得
                NetworkStream ns = client.GetStream();

                //読み取り、書き込みのタイムアウトを10秒にする
                //デフォルトはInfiniteで、タイムアウトしない
                //(.NET Framework 2.0以上が必要)
                ns.ReadTimeout = 10000;
                ns.WriteTimeout = 10000;

                //クライアントから送られたデータを受信する
                Encoding enc = Encoding.UTF8;
                bool disconnected = false;
                MemoryStream ms = new MemoryStream();
                byte[] resBytes = new byte[256];
                int resSize = 0;
                do
                {
                    //データの一部を受信する
                    resSize = ns.Read(resBytes, 0, resBytes.Length);
                    //Readが0を返した時はクライアントが切断したと判断
                    if (resSize == 0)
                    {
                        disconnected = true;
                        Console.WriteLine("クライアントが切断しました。");
                        break;
                    }
                    //受信したデータを蓄積する
                    ms.Write(resBytes, 0, resSize);
                    //まだ読み取れるデータがあるか、データの最後が\nでない時は、
                    // 受信を続ける
                }
                while (ns.DataAvailable || resBytes[resSize - 1] != '\n');
                //受信したデータを文字列に変換
                string resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                ms.Close();
                //末尾の\nを削除
                resMsg = resMsg.TrimEnd('\n');
                Sends.Add(resMsg);
                Console.WriteLine(resMsg);

                if (!disconnected)
                {
                    //クライアントにデータを送信する
                    //クライアントに送信する文字列を作成
                    string sendMsg = resMsg.Length.ToString();
                    //文字列をByte型配列に変換
                    byte[] sendBytes = enc.GetBytes(sendMsg + '\n');
                    //データを送信する
                    ns.Write(sendBytes, 0, sendBytes.Length);
                    Console.WriteLine(sendMsg);
                }

                //閉じる
                ns.Close();
                client.Close();
            }
            catch (Exception e) { Console.Write(e.ToString()); }
            Console.WriteLine("クライアントとの接続を閉じました。");
        }
        #endregion

        #region Client
        public static void SendClient(string str)
        {
            Console.WriteLine(str);
            try
            {
                TcpClient tcp = new TcpClient(HostIP, HostPort);
                var epoint = tcp.Client.RemoteEndPoint;
                var endpoint = epoint != null ? (IPEndPoint)epoint : new IPEndPoint(0, 0);
                var lpoint = tcp.Client.RemoteEndPoint;
                var localpoint = lpoint != null ? (IPEndPoint)lpoint : new IPEndPoint(0, 0);
                Console.WriteLine($"サーバー({endpoint.Address}:{endpoint.Port})と接続しました({localpoint.Address}:{localpoint.Port})。");

                //NetworkStreamを取得する
                NetworkStream ns = tcp.GetStream();

                //読み取り、書き込みのタイムアウトを10秒にする
                //デフォルトはInfiniteで、タイムアウトしない
                //(.NET Framework 2.0以上が必要)
                ns.ReadTimeout = 10000;
                ns.WriteTimeout = 10000;

                //サーバーにデータを送信する
                //文字列をByte型配列に変換
                Encoding enc = Encoding.UTF8;
                byte[] sendBytes = enc.GetBytes(str + '\n');
                //データを送信する
                ns.Write(sendBytes, 0, sendBytes.Length);

                //サーバーから送られたデータを受信する
                MemoryStream ms = new MemoryStream();
                byte[] resBytes = new byte[256];
                int resSize = 0;
                do
                {
                    //データの一部を受信する
                    resSize = ns.Read(resBytes, 0, resBytes.Length);
                    //Readが0を返した時はサーバーが切断したと判断
                    if (resSize == 0)
                    {
                        Console.WriteLine("サーバーが切断しました。");
                        break;
                    }
                    //受信したデータを蓄積する
                    ms.Write(resBytes, 0, resSize);
                    //まだ読み取れるデータがあるか、データの最後が\nでない時は、
                    // 受信を続ける
                } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');
                //受信したデータを文字列に変換
                string resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                ms.Close();
                //末尾の\nを削除
                resMsg = resMsg.TrimEnd('\n');
                Console.WriteLine(resMsg);

                //閉じる
                ns.Close();
                tcp.Close();
            }
            catch (Exception e) { Console.Write(e.ToString()); }
        }
        #endregion
    }
}
