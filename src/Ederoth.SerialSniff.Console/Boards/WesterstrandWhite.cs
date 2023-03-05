using System;
using System.IO.Ports;
using System.Text;
using Ederoth.SerialSniffer;
using Spectre.Console;

namespace Ederoth.SerialSniff.Console.Boards
{
    public class WesterstrandWhite : IScoreboard
    {

        // Define the start marker that marks the beginning of the message
        const string StartMarker = "??";

        // Define the length of the message
        const int MessageLength = 90;

        // Define the size of the buffer
        const int BufferSize = 1024;
        private readonly HttpClient _client;

        // Create a buffer to store incoming bytes
        byte[] buffer = new byte[BufferSize];

        // Create a StringBuilder to build the message
        StringBuilder messageBuilder = new StringBuilder(MessageLength);

        private string time = string.Empty;
        private string shotClock = "24";
        private string homeScore = "0";
        private string awayScore = "0";
        public WesterstrandWhite()
        {
            _client = new HttpClient();
            _client.BaseAddress =new Uri("http://127.0.0.1:8088/API/");
        }

        public ScoreboardDto ParseData(SerialPort serialPort, CancellationToken cancellationToken)
        {
            bool stateHasChanged = false;
            while (!cancellationToken.IsCancellationRequested)
            {
                int bytesRead = 0;
                // Read bytes from the serial port into the buffer
                try
                {
                    bytesRead = serialPort.Read(buffer, 0, BufferSize);
                } 
                catch 
                {
                    continue;
                }
                // Loop through the buffer and process complete messages
                for (int i = 0; i < bytesRead; i++)
                {
                    // If the buffer contains the start marker, reset the message builder
                    if (buffer[i] == StartMarker[0] && i + 1 < bytesRead && buffer[i + 1] == StartMarker[1])
                    {
                        ReadOnlySpan<char> messageSpan = messageBuilder.ToString();

                        if (messageSpan.StartsWith("RD!"))
                        {
                            time = messageSpan.Slice(4, 4).Trim().ToGameClock();
                            _client.GetAsync($"?Function=SetText&Input=test5.gtzip&SelectedName=Klocka.Text&Value={time}");
                            stateHasChanged = true;
                        }
                        else if (messageSpan.StartsWith("RD$"))
                        {
                            shotClock = messageSpan.Slice(3, 4).Trim().ToShotClock();                            
                            stateHasChanged = true;
                        }
                        else if (messageSpan.StartsWith("RD&!"))
                        {
                            homeScore = messageSpan.Slice(4, 3).Trim().ToString();
                            _client.GetAsync($"?Function=SetText&Input=test5.gtzip&SelectedName=Mål_Hemma.Text&Value&Value={homeScore}");
                            stateHasChanged = true;
                        }
                        else if (messageSpan.StartsWith("RD&)"))
                        {
                            awayScore = messageSpan.Slice(4, 3).Trim().ToString();
                            _client.GetAsync($"?Function=SetText&Input=test5.gtzip&SelectedName=Mål_Borta.Text&Value&Value={awayScore}");
                            stateHasChanged = true;
                        }

                        if (stateHasChanged)
                        {
                            stateHasChanged = false;
                            AnsiConsole.MarkupLine($"[cyan1]{time}[/] [green]{homeScore} - {awayScore}[/]   [yellow]{shotClock}[/]");
                        }
                        messageBuilder.Clear();
                        i++; // Skip the second start marker byte
                    }

                    // If the message builder is not empty and the message is not complete, add the character to the message
                    else if (messageBuilder.Length + 1 < MessageLength)
                    {
                        messageBuilder.Append((char)buffer[i]); // Add the character to the message
                    }
                }
            }

            return null;
        }
    }
}