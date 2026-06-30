using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;


namespace SFServer.Utility
{
    public class IdGenerator
    {
        private const string _alphabet = "abcdefghijkmnopqrstuvwxyzACDEFHJKLMNPQRTUVWXY3479";
        private const int _length = 30;

        public static string Generate()
        {
            return Nanoid.Nanoid.Generate(_alphabet, _length);
        }

        public static string Generate(int length)
        {
            return Nanoid.Nanoid.Generate(_alphabet, length);
        }
    }
}
