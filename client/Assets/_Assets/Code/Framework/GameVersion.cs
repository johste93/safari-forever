using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameVersion
{
    public int major;
    public int minor;

    public GameVersion(string version)
    {
        string[] splits = version.Split('.');
        if(splits.Length != 2)
        {
            Debug.LogError("Could not parse string!");
            return;
        }

        if(!int.TryParse(splits[0], out int _major))
        {
            Debug.LogError("Could not parse first segment!");
            return;
        }
        major = _major;

        if (!int.TryParse(splits[1], out int _minor))
        {
            Debug.LogError("Could not parse first segment!");
            return;
        }
        minor = _minor;
    }

    public bool IsEqual(GameVersion other)
    {
        return major == other.major && minor == other.minor;
    }

    public bool IsOlderThan(GameVersion other)
    {
        return major < other.major || (major == other.major && minor < other.minor);
    }

    public bool IsNewerThan(GameVersion other)
    {
        return other.major < major || (major == other.major && other.minor < minor);
    }

    public bool IsNewerThanOrEqual(GameVersion other)
    {
        return IsEqual(other) || IsNewerThan(other);
    }

    public bool IsOlderOrEqual(GameVersion other)
    {
        return IsEqual(other) || IsOlderThan(other);
    }

    public override string ToString()
    {
        return $"{major}.{minor}";
    }
}
