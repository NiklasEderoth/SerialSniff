using Ederoth.SerialSniffer.Commands;
using Spectre.Console.Cli;

var app = new CommandApp<SerialSnifferCommand>();
return app.Run(args);