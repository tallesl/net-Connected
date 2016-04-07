namespace ConnectedLibrary
{
    using System;
    using System.Net;

    public static partial class Connected
    {
        /// <summary>
        /// Issues a GET request to the given URL.
        /// </summary>
        /// <param name="url">URL to request</param>
        /// <returns>True if the server responded with success, false otherwise</returns>
        public static bool Http(string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return Http(new Uri(url));
        }

        /// <summary>
        /// Issues a GET request to the given URI.
        /// </summary>
        /// <param name="uri">URI to request</param>
        /// <returns>True if the server responded with success, false otherwise</returns>
        public static bool Http(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
