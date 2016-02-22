namespace ConnectedLibrary
{
    using SocketThat;
    using System.Net.Sockets;

    public static partial class Connected
    {
        private const string _redisLineTerminator = "\r\n";

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// Assumes localhost as host and 6379 as port.
        /// </summary>
        /// <returns>True if the Redis server responded with success, false otherwise.</returns>
        public static bool Redis()
        {
            return Redis("localhost", 6379);
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// Assumes localhost as host and 6379 as port.
        /// </summary>
        /// <param name="auth">Redis password</param>
        /// <returns>True if the Redis server responded with success, false otherwise.</returns>
        public static bool Redis(string auth)
        {
            return Redis("localhost", 6379, auth);
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// </summary>
        /// <param name="host">Redis server host</param>
        /// <param name="port">Redis server port</param>
        /// <returns>True if the Redis server responded with success, false otherwise.</returns>
        public static bool Redis(string host, int port)
        {
            return Redis(host, port, null);
        }

        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// </summary>
        /// <param name="host">Redis server host</param>
        /// <param name="port">Redis server port</param>
        /// <param name="auth">Redis password</param>
        /// <returns>True if the Redis server responded with success, false otherwise.</returns>
        public static bool Redis(string host, int port, string auth)
        {
            try
            {
                using (var socket = new ConnectedSocket(host, port))
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
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}
