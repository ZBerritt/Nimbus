using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SaveDataSync
{
    // Represents an oauth token where the user needs to login on their browser. Calls back to BASE_ADDRESS
    internal class OAuthToken
    {
        private string code;
        public OAuthToken(string url)
        {
            // Start callback server
            byte[] bytes = new Byte[1024];

            IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 1235);

            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                Process.Start(url); // Open browser
                Socket handler = listener.Accept();
                Console.WriteLine("Got connection!");
                int bytesRec = handler.Receive(bytes);
                string data = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                // Get url query
                string query = data
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0]
                    .Split(new string[] { " " }, StringSplitOptions.None)[1]
                    .Substring(2 + "callback".Length);

                string[] pairs = query.Split(new string[] { "&" }, StringSplitOptions.None);
                foreach (string pair in pairs)
                {
                    var key = pair.Split(new string[] { "=" }, StringSplitOptions.None)[0];
                    if (key == "code")
                    {
                        code = pair.Split(new string[] { "=" }, StringSplitOptions.None)[1];
                        break;
                    }
                }
                byte[] msg = Encoding.ASCII.GetBytes("Token recieved! You can now close this tab.");
                handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }

        public string GetCode()
        {
            return code;
        }
    }
}
