using Ederoth.SerialSniffer.Commands;
using Spectre.Console.Cli;

public class Program
{
    public static CancellationTokenSource Cts = new CancellationTokenSource();
    public static void Main(string[] args)
    {
        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("canceling...");
            Cts.Cancel();
            e.Cancel = true;
        };

        var app = new CommandApp<SerialSnifferCommand>();
        app.Run(args);
    }
}