

using System.Diagnostics;

namespace AutoSearch.Tools
{
    public class Watch
    {
        public Stopwatch watch = new();
        public void Start()
        {
            watch.Start();
        }

        public void Stop()
        {
            watch.Stop();
        }

        public string getTotalTime()
        {
            var elapsedMs = watch.ElapsedMilliseconds;
            TimeSpan t = TimeSpan.FromMilliseconds(elapsedMs);
            string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds);

            return answer;
        }
    }
}
