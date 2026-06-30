using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highscore
{
    public int Seconds { get; set; } = -1;
    public int Milliseconds { get; set; } = -1;

    public Highscore(int Seconds, int Milliseconds)
    {
        this.Seconds = Seconds;
        this.Milliseconds = Milliseconds;
    }

    public Highscore(double highscore)
    {
        this.Seconds = (int)highscore;
        this.Milliseconds = (int)((highscore - ((int)highscore)) * 100) + 1;
    }

    public bool IsEqual(Highscore other)
    {
        return Seconds == other.Seconds && Milliseconds == other.Milliseconds;
    }

    public bool IsLowerThan(Highscore other)
    {
        if (other.Seconds < 0 || other.Milliseconds < 0) //This exeption makes it easier to override unset scores
            return true;

        if (Seconds < 0 || Milliseconds < 0) //This exeption makes stops us from override with bad data.
            return false;

        return Seconds < other.Seconds || (Seconds == other.Seconds && Milliseconds < other.Milliseconds);
    }

    public bool IsLowerThanOrEqual(Highscore other)
    {
        if (Seconds < 0 || Milliseconds < 0) //This exeption makes stops us from override with bad data.
            return false;

        return IsLowerThan(other) || IsEqual(other);
    }

    public override string ToString()
    {
        return $"{Seconds}.{Milliseconds}";
    }
}