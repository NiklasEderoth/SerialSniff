using System.IO.Ports;
using System.Text;
using Spectre.Console;

namespace Ederoth.SerialSniff.Console.Boards
{
    public class WesterstrandBlue : IScoreboard
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

        // Continuously read bytes from the serial port and process complete messages
        string time = string.Empty;
        public WesterstrandBlue()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://127.0.0.1:8088/API/");
        }

        public ScoreboardDto ParseData(SerialPort serialPort, CancellationToken cancellationToken)
        {
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
                        messageBuilder.Clear();
                        i++; // Skip the second start marker byte
                    }
                    // If the message builder is not empty and the message is complete, process the message
                    else if (messageBuilder.Length > 5 && (char)buffer[i] == 'X')
                    {
                        messageBuilder.Append((char)buffer[i]); // Add the last character to the message
                        try
                        {
                            ReadOnlySpan<char> messageSpan = messageBuilder.ToString();

                            // Extract the data between index 27 and 30 and assign it to the home variable
                            ReadOnlySpan<char> timeSpan = messageSpan.Slice(15, 5).Trim();
                            ReadOnlySpan<char> homeSpan = messageSpan.Slice(27, 3).Trim();
                            ReadOnlySpan<char> awaySpan = messageSpan.Slice(30, 3).Trim();

                            // Process the message and home variable as needed
                            var newTime = timeSpan.ToString();
                            if (time != newTime)
                            {
                                time = newTime;
                                _client.GetAsync($"?Function=SetText&Input=test5.gtzip&SelectedName=Klocka.Text&Value={time}");
                                _client.GetAsync($"?Function=SetText&Input=test5.gtzip&SelectedName=Mål_Hemma.Text&Value&Value={homeSpan}");
                                _client.GetAsync($"?Function=SetText&Input=test5.gtzip&SelectedName=Mål_Borta.Text&Value&Value={awaySpan}");
                                AnsiConsole.MarkupLine($"[green]{homeSpan}[/] [cyan1]{time}[/] [green]{awaySpan}[/]");
                            }
                        }
                        catch(Exception ex)
                        {

                        }

                        messageBuilder.Clear(); // Reset the message builder
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
