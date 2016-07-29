namespace ConnectedLibrary
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public static partial class Connected
    {
        private const int _redisDefaultPort = 6379;

        private const string _redisLineTerminator = "\r\n";

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// Assumes localhost as host and 6379 as port.
        /// </summary>
        /// <returns>True if the server responded with success, false otherwise</returns>
        public static bool Redis()
        {
            return _Redis(new IPEndPoint(IPAddress.Loopback, _redisDefaultPort), null);
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// Assumes localhost as host and 6379 as port.
        /// </summary>
        /// <param name="auth">Redis password</param>
        /// <returns>True if the server responded with success, false otherwise</returns>
        public static bool Redis(string auth)
        {
            if (auth == null)
                throw new ArgumentNullException("auth");

            return _Redis(new IPEndPoint(IPAddress.Loopback, _redisDefaultPort), auth);
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// </summary>
        /// <param name="endpoint">Server endpoint</param>
        /// <returns>True if the server responded with success, false otherwise</returns>
        public static bool Redis(EndPoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            return _Redis(endpoint, null);
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// </summary>
        /// <param name="host">Server host</param>
        /// <param name="port">Server port</param>
        /// <returns>True if the server responded with success, false otherwise</returns>
        public static bool Redis(string host, int port)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            IPAddress ip;

            if (!IPAddress.TryParse(host, out ip))
                ip = Dns.GetHostAddresses(host).First();

            return _Redis(new IPEndPoint(ip, port), null);
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// </summary>
        /// <param name="endpoint">Server endpoint</param>
        /// <param name="auth">Redis password</param>
        /// <returns>True if the server responded with success, false otherwise</returns>
        public static bool Redis(EndPoint endpoint, string auth)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            if (auth == null)
                throw new ArgumentNullException("auth");

            return _Redis(endpoint, auth);
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// </summary>
        /// <param name="host">Server host</param>
        /// <param name="port">Server port</param>
        /// <param name="auth">Redis password</param>
        /// <returns>True if the server responded with success, false otherwise</returns>
        public static bool Redis(string host, int port, string auth)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            if (auth == null)
                throw new ArgumentNullException("auth");

            IPAddress ip;

            if (!IPAddress.TryParse(host, out ip))
                ip = Dns.GetHostAddresses(host).First();

            return _Redis(new IPEndPoint(ip, port), auth);
        }

        private static bool _Redis(EndPoint endPoint, string auth)
        {
            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Connect(endPoint);

                    var buffer = new byte[1024];

                    if (auth != null)
                    {
                        var authCmd = string.Format(
                            CultureInfo.InvariantCulture, "AUTH {0}{1}", auth, _redisLineTerminator);

                        socket.Send(Encoding.UTF8.GetBytes(authCmd));
                        socket.Receive(buffer);

                        if (Encoding.UTF8.GetString(buffer).TrimEnd('\0') != ("+OK" + _redisLineTerminator))
                            return false;
                    }

                    var pingCmd = string.Format(CultureInfo.InvariantCulture, "PING{0}", _redisLineTerminator);

                    socket.Send(Encoding.UTF8.GetBytes(pingCmd));
                    socket.Receive(buffer);

                    return Encoding.UTF8.GetString(buffer).TrimEnd('\0') ==
                        string.Format(CultureInfo.InvariantCulture, "+PONG{0}", _redisLineTerminator);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
