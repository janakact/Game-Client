using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Game_Client
{

    // Define a class to hold custom event info
    public class DataRecieveEventArgs : EventArgs
    {
        public DataRecieveEventArgs(string data)
        {
            message = data;
        }
        private string message;
        private DateTime time;

        public string Data
        {
            get { return message; }
            set { message = value; }
        }

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
    }

    class NetworkClient
    {
        IPAddress ipAddress;
        int sendPort;
        int listenPort;

        //For sending data
        TcpClient tcpClient;


        //For listening 
        TcpListener tcpListener;
        Thread thread;
        bool recieving = false;

        //Instance
        private static NetworkClient instance;

        //When data recieved 'event'
        public event EventHandler<DataRecieveEventArgs> OnRecieve;

        public static NetworkClient getInstance(string ipAddress, int sendPort, int listenPort)
        {
            if(instance==null)
            {
                instance = new NetworkClient(ipAddress, sendPort, listenPort);
            }
            return instance;
        }
        private NetworkClient(string ipAddress, int sendPort, int listenPort)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.sendPort = sendPort;
            this.listenPort = listenPort;
        }

        public void Set(string ipAddress, int sendPort, int listenPort)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.sendPort = sendPort;
            this.listenPort = listenPort;
        }


        // Wrap event invocations inside a protected virtual method
        // to allow derived classes to override the event invocation behavior
        protected virtual void RaiseOnRecieve(DataRecieveEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<DataRecieveEventArgs> handler = OnRecieve;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                // Set the Time of the message
                e.Time = DateTime.Now;

                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

        public void StartListening()
        {

            //start listening to server's broadcast port
            try
            {
                // Set the listener on the local IP address 
                // and specify the port.
                tcpListener = new TcpListener(ipAddress, listenPort);
                tcpListener.Start();
                thread = new Thread(new ThreadStart(Recieve));
                recieving = true;
                thread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to start listener {0}", e.ToString());
                return;
            }
        }

        public void Recieve()
        {
            while (recieving)
            {
                try {
                    // Always use a Sleep call in a while(true) loop 
                    // to avoid locking up your CPU.
                    Thread.Sleep(10);
                    // Create a TCP socket. 
                    // If you ran this server on the desktop, you could use 
                    // Socket socket = tcpListener.AcceptSocket() 
                    // for greater flexibility.
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    // Read the data stream from the client. 
                    byte[] bytes = new byte[256];
                    NetworkStream stream = tcpClient.GetStream();
                    stream.Read(bytes, 0, bytes.Length);
                    string msg = Encoding.ASCII.GetString(bytes);
                    RaiseOnRecieve(new DataRecieveEventArgs(msg));
                }
                catch(Exception e)
                {
                    Console.WriteLine("Recive Error: {0}", e.ToString());
                }
            }
        }

        public void Send(string data)
        {
            try
            {

                tcpClient = new TcpClient();
                tcpClient.Connect(ipAddress, sendPort);

                if (tcpClient.Connected)
                {
                    NetworkStream stream = tcpClient.GetStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Byte[] bytes = Encoding.ASCII.GetBytes(data);
                    writer.Write(bytes);
                    writer.Close();
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Sending failed : {0}", e.ToString());
            }
        }

        public void StopListening()
        {
            try
            {

                recieving = false;
                tcpListener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
    }
}
