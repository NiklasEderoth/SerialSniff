using System.Diagnostics.CodeAnalysis;
using Ederoth.SerialSniffer.Options;
using Spectre.Console;
using Spectre.Console.Cli;
using System.IO.Ports;
using System.Reflection;

namespace Ederoth.SerialSniffer.Commands;

public class SerialSnifferCommand : Command<SerialSnifferOptions>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] SerialSnifferOptions settings)
    {
        if (settings.Version)
        {
            string versionString = Assembly.GetEntryAssembly()?
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
                .ToString() ?? string.Empty;
            AnsiConsole.WriteLine(versionString);
            return 0;
        }

        AnsiConsole.Write(
            new FigletText("Serial Sniff")
                .Centered()
                .Color(Color.Cyan1));
        var rule = new Rule("By Ederoth corp.");
        rule.Justification = Justify.Center;
        rule.Style = new Style(Color.Cyan1);

        AnsiConsole.Write(rule);

        var fileName = settings.FileName ?? DateTime.UtcNow.ToString();
        var outputPath = settings.FilePath ?? Directory.GetCurrentDirectory();
        var filePath = Path.GetFullPath(Path.Combine(outputPath, fileName + ".txt"));
        Directory.CreateDirectory(outputPath);
        string[] availablePorts = SerialPort.GetPortNames();

        string? serialPortName = settings.SerialPort;
        if (serialPortName.IsNullOrEmpty())
        {
            serialPortName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [green]serial port[/]?")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more ports)[/]")
                    .AddChoices(availablePorts));
        }
        else if (!availablePorts.Contains(serialPortName))
        {
            AnsiConsole.MarkupLine($"Serial port [yellow]{serialPortName}[/] not found!");
            var rowStyle = new Style(Color.Green);
            var rows = availablePorts.Select(t => new Text(t, rowStyle));
            var panel = new Panel(new Rows(rows));
            panel.Header = new PanelHeader("available ports:");
            panel.Expand = true;
            AnsiConsole.Write(panel);
            AnsiConsole.MarkupLine("[red]Exit program[/]");
            return 1;
        }
        AnsiConsole.Write(new Panel(serialPortName)
        {
            Header = new PanelHeader("Selected port:")
        });
        
        var path = new TextPath(filePath)
            .RootColor(Color.Cyan1)
            .SeparatorColor(Color.LightCyan1)
            .StemColor(Color.Yellow)
            .LeafColor(Color.White);

        var pathPanel = new Panel(path);
        pathPanel.Header("Writing to:");
        AnsiConsole.Write( pathPanel);


        var serialPort = new SerialPort(serialPortName);
        // Synchronous
        var status = AnsiConsole.Status();
        status.Spinner = Spinner.Known.Runner;
        status.Start("Recording... press any key to stop.", ctx =>
            {
                serialPort.Open();
                using StreamWriter file = new(filePath, append: true);
                serialPort.DataReceived += async (sender, e) =>
                {
                    await file.WriteLineAsync("Fourth line");
                    var data = ((SerialPort)sender).ReadExisting();
                    AnsiConsole.MarkupLine($"[grey]{data}[/]");
                };
                Console.ReadKey();
                serialPort.Close();
            });
        AnsiConsole.MarkupLine($"[cyan1]I'm done, Good by![/]");
        return 0;
    }
}