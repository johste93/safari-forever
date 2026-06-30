using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrotliSharpLib;

namespace SFServer.Utility
{
    public class BrotliHelper
    {
        public static bool Decompress(byte[] input, ref byte[] output)
        {
            try
            {
                output = Brotli.DecompressBuffer(input, 0, input.Length);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
