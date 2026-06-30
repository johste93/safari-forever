using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Utility
{
    public class Highscore
    {
        public int Seconds { get; set; }
        public int Milliseconds { get; set; }

        public Highscore(int Seconds, int Milliseconds)
        {
            this.Seconds = Seconds;
            this.Milliseconds = Milliseconds;
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
            string time = $"{Seconds:00}.{Milliseconds:00}";
            return time.Substring(0, Math.Min(time.Length, 5));
        }

        public Highscore GetScoreSpan(Highscore other)
        {
            int a_score = (Seconds * 100) + Milliseconds;
            int b_score = (other.Seconds * 100) + other.Milliseconds;
            int difference = a_score - b_score;

            double time = ((double)difference / 100d);

            int seconds = (int)time;
            int milliseconds = (int)((time - ((int)time)) * 100);

            return new Highscore(seconds, milliseconds);
        }
    }
}
