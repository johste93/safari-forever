using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Utility
{
    public class ClientVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }

        public ClientVersion()
        {
        }

        public ClientVersion(int major, int minor)
        {
            this.Major = major;
            this.Minor = minor;
        }

        public bool IsEqual(ClientVersion other)
        {
            if (Major <= 0 && Minor <= 0)
                return false;

            return Major == other.Major && Minor == other.Minor;
        }

        public bool IsNewerThan(ClientVersion other)
        {
            if (Major <= 0 && Minor <= 0)
                return false;

            return Major > other.Major || (Major == other.Major && Minor > other.Minor);
        }

        public bool IsNewerThanOrEqual(ClientVersion other)
        {
            if (Major <= 0 && Minor <= 0)
                return false;

            return IsNewerThan(other) || IsEqual(other);
        }

        public static bool Parse(string raw, out ClientVersion version)
        {
            version = null;
            if (string.IsNullOrWhiteSpace(raw))
            {
                Console.WriteLine("raw is null or whitespace");
                return false;
            }

            string[] segments = raw.Split('.');
            if (segments.Length != 2)
            {
                Console.WriteLine("Mismatching number of segments");
                return false;
            }
                

            if (!int.TryParse(segments[0], out int _major))
            {
                Console.WriteLine("Unable to parse first segment");
                return false;
            }

            if (!int.TryParse(segments[1], out int _minor))
            {
                Console.WriteLine("Unable to parse second segment");
                return false;
            }


            version = new ClientVersion(_major, _minor);
            return true;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}";
        }
    }
}
