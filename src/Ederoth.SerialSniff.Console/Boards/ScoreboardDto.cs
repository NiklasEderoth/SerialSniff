using System.Diagnostics.CodeAnalysis;

namespace Ederoth.SerialSniff.Console.Boards
{
    public class ScoreboardDto
    {
        [NotNull]
        public string Time { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public int ShotClock { get; set; }
    }
}
