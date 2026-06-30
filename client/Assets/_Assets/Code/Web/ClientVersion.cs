using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            return false;

        string[] segments = raw.Split('.');
        if (segments.Length != 2)
            return false;

        if (!int.TryParse(segments[0], out int _major))
            return false;

        if (!int.TryParse(segments[1], out int _minor))
            return false;


        version = new ClientVersion(_major, _minor);
        return true;
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}";
    }
}
