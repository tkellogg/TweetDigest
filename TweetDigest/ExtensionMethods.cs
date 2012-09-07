using System.IO;
using System.Text;

namespace TweetDigest
{
    public static class ExtensionMethods
    {
        public static string ReadToEnd(this Stream stream)
        {
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }
    }
}