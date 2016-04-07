namespace ConnectedLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Configuration;
    using System.Net.Sockets;
    using System.Text;

    public static partial class Connected
    {
        private const string _smtpLineTerminator = "\r\n";

        private static readonly int[] _smtpValidReplyCodes = {
            // Non standard for "success"
            200,

            // Service ready
            220,

            // Requested mail action okay, completed
            250
        };

        /// <summary>
        /// Issues a HELO to a SMTP server thus testing its connection.
        /// Reads the SMTP server settings (host and port) from &lt;smtp&gt; in config.
        /// </summary>
        /// <returns>True if the SMTP server responded with success, false otherwise</returns>
        public static bool Smtp()
        {
            var cfg = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;

            if (cfg == null)
                throw new ConfigurationErrorsException("There's no SMTP configuration in .config.");

            return Smtp(cfg.Network.Host, cfg.Network.Port);
        }

        /// <summary>
        /// Issues a HELO to a SMTP server thus testing its connection.
        /// </summary>
        /// <param name="host">Server host</param>
        /// <param name="port">Server port</param>
        /// <returns>True if the SMTP server responded with success, false otherwise</returns>
        public static bool Smtp(string host, int port)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            return Smtp(new IPEndPoint(IPAddress.Parse(host), port));
        }

        /// <summary>
        /// Issues a HELO to a SMTP server thus testing its connection.
        /// </summary>
        /// <param name="endPoint">Server endpoint</param>
        /// <returns>True if the SMTP server responded with success, false otherwise</returns>
        public static bool Smtp(EndPoint endPoint)
        {
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");

            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Connect(endPoint);

                    var cmd = string.Format(CultureInfo.InvariantCulture, "HELO {0}{1}", Dns.GetHostName(), _smtpLineTerminator);
                    socket.Send(Encoding.UTF8.GetBytes(cmd));

                    var received = new byte[1024];
                    socket.Receive(received);

                    return SanitizeSmtpAnswer(Encoding.UTF8.GetString(received)).Any(ValidSmtpReplyCode);
                }
            }
            catch
            {
                return false;
            }
        }

        private static IEnumerable<string> SanitizeSmtpAnswer(string answer)
        {
            return answer
                .Split(new [] { _smtpLineTerminator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToUpperInvariant());
        }

        private static bool ValidSmtpReplyCode(string answer)
        {
            return _smtpValidReplyCodes.Any(
                x => x.ToString(CultureInfo.InvariantCulture) == answer.Split(new[] { ' ' }).First()
            );
        }
    }
}
