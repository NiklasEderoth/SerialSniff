using System.ComponentModel;
using Spectre.Console.Cli;

namespace Ederoth.SerialSniffer.Options;

public class SerialSnifferOptions: CommandSettings
{
    [Description("Output path. Defaults to current directory.")]
    [CommandOption("-o|--output")]
    public string? FilePath { get; set; }

    [Description("Name of output file. Defaults to current DateTime utc.")]
    [CommandOption("-n|--name")]
    public string? FileName { get; set; }

    [Description("Name of SerialPort. If not a list of available ports will be displayed")]
    [CommandOption("-p|--port")]
    public string? SerialPort { get; set; }

    [CommandOption("-v|--version")]
    public bool Version { get; set; }
}