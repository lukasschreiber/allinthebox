using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace allinthebox
{
    public class RemoteServer
    {
        public static int PORT = 13000;
        public static string CODE = "";

        private static readonly Random random = new Random();

        public byte[] ANSWER;

        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();

        private string recieved, sent;
        private string res = "";

        public RemoteServer(Main main)
        {
            CODE = GenerateNewCode(16);

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;

            backgroundWorker.DoWork += ServerWorking;
            backgroundWorker.RunWorkerCompleted += ServerCompleted;
        }

        public void StartServer()
        {
            if (backgroundWorker.IsBusy != true) backgroundWorker.RunWorkerAsync();
        }

        public void StopServer()
        {
            if (backgroundWorker.WorkerSupportsCancellation) backgroundWorker.CancelAsync();
        }

        private void ServerWorking(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            TcpListener server = null;

            try
            {
                server = new TcpListener(IPAddress.Parse(GetIP4Address()), PORT);
                server.Start();

                var bytes = new byte[25600000];
                string data = null;


                while (true)
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        Console.Write("Waiting for a connection.... ");
                        var client = server.AcceptTcpClient();
                        Console.WriteLine("Connected!");

                        data = null;

                        var stream = client.GetStream();

                        int i;

                        try
                        {
                            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                data = Encoding.UTF8.GetString(bytes, 0, i);
                                Console.WriteLine("Received: {0}", data);

                                recieved = data;

                                OnMessageRecieved(new ServerEventArgs(recieved));

                                var byteList = new List<byte>();

                                var enc = new UTF8Encoding();
                                var ans = enc.GetString(ANSWER);

                                //byteList.AddRange(BitConverter.GetBytes(0));
                                byteList.AddRange(BitConverter.GetBytes(ans.Length));
                                byteList.AddRange(ANSWER);
                                var submitData = byteList.ToArray();
                                stream.Write(submitData, 0, submitData.Length);

                                Console.WriteLine("Sent: {0} Length: {1}", ans, ans.Length);
                            }
                        }
                        catch (Exception exc)
                        {
                            Program.logger.error(exc.Message, exc.StackTrace);
                        }

                        client.Close();
                        Console.WriteLine("Client closed");
                    }
            }
            catch (SocketException exc)
            {
                Console.Write("SocketException: {0}", exc.StackTrace);
            }
            finally
            {
                server.Stop();
            }
        }


        private void ServerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                res = "Canceled!";
            else if (e.Error != null)
                res = "Error: " + e.Error.Message;
            else
                res = "Done!";
        }

        //listener


        public event EventHandler<ServerEventArgs> RecievedMessage;

        protected virtual void OnMessageRecieved(ServerEventArgs e)
        {
            var handler = RecievedMessage;

            if (handler != null)
            {
                e.Message = e.Message;
                handler(this, e);
            }
        }

        public static string GetIP4Address()
        {
            var IP4Address = string.Empty;

            foreach (var IPA in Dns.GetHostAddresses(Dns.GetHostName()))
                if (IPA.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP4Address = IPA.ToString();
                    break;
                }

            return IP4Address;
        }

        public static string GetExtIP4Address()
        {
            var externalip = new WebClient().DownloadString("http://ipinfo.io/ip");

            return externalip.Trim();
        }

        public static string GenerateNewCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static class CODES
        {
            public static int CONNECTED = 0;
            public static int BARCODE = 1;
            public static int CANCEL = 2;
            public static int SAVE = 3;
            public static int ADD = 4;
            public static int NOT_IN_DB = 5;
            public static int CURRENTLY_IN_USE = 100;
        }
    }

    public class ServerEventArgs : EventArgs
    {
        public ServerEventArgs(string s)
        {
            Message = s;
        }

        public string Message { get; set; }
    }
}