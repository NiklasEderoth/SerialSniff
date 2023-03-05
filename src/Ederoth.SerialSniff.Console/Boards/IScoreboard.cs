using System.IO.Ports;

namespace Ederoth.SerialSniff.Console.Boards
{
    internal interface IScoreboard
    {
        ScoreboardDto ParseData(SerialPort serialPort, CancellationToken cancellationToken);
    }
}
