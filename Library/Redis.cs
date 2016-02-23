namespace ConnectedLibrary
{
    using SocketLibrary;
    using System;
    using System.Net;
    using System.Net.Sockets;

    public static partial class Connected
    {
        private const int _redisDefaultPort = 6379;

        private const string _redisLineTerminator = "\r\n";

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// Assumes localhost as host and 6379 as port.
        /// </summary>
        /// <returns>True if the Redis server responded with success, false otherwise.</returns>
        public static bool Redis()
        {
            return Redis("localhost", _redisDefaultPort);
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// Assumes localhost as host and 6379 as port.
        /// </summary>
        /// <param name="auth">Redis password</param>
        /// <returns>True if the Redis server responded with success, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If the given auth is null</exception>
        public static bool Redis(string auth)
        {
            if (auth == null)
                throw new ArgumentNullException("auth");

            using (var socket = new ConnectedSocket("localhost", _redisDefaultPort))
            {
                return Redis(socket, auth);
            }
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// </summary>
        /// <param name="endpoint">Redis endpoint</param>
        /// <returns>True if the Redis server responded with success, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If the given endpoint is null</exception>
        public static bool Redis(EndPoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            using (var socket = new ConnectedSocket(endpoint))
            {
                return Redis(socket, null);
            }
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// </summary>
        /// <param name="host">Redis server host</param>
        /// <param name="port">Redis server port</param>
        /// <returns>True if the Redis server responded with success, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If the given host is null</exception>
        public static bool Redis(string host, int port)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            using (var socket = new ConnectedSocket(host, port))
            {
                return Redis(socket, null);
            }
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// </summary>
        /// <param name="endpoint">Redis endpoint</param>
        /// <param name="auth">Redis password</param>
        /// <returns>True if the Redis server responded with success, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If the given endpoint or auth is null</exception>
        public static bool Redis(EndPoint endpoint, string auth)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            if (auth == null)
                throw new ArgumentNullException("auth");

            using (var socket = new ConnectedSocket(endpoint))
            {
                return Redis(socket, auth);
            }
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// </summary>
        /// <param name="host">Redis server host</param>
        /// <param name="port">Redis server port</param>
        /// <param name="auth">Redis password</param>
        /// <returns>True if the Redis server responded with success, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If the given host or auth is null</exception>
        public static bool Redis(string host, int port, string auth)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            if (auth == null)
                throw new ArgumentNullException("auth");

            using (var socket = new ConnectedSocket(host, port))
            {
                return Redis(socket, auth);
            }
        }

        private static bool Redis(ConnectedSocket socket, string auth)
        {
            try
            {
                if (auth != null)
                {
                    socket.Send("AUTH " + auth + _redisLineTerminator);
                    if (socket.Receive() != ("+OK" + _redisLineTerminator))
                        return false;
                }

                socket.Send("PING" + _redisLineTerminator);
                return socket.Receive() == ("+PONG" + _redisLineTerminator);
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}
