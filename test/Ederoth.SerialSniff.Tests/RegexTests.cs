using Xunit;
using System.Text.RegularExpressions;

namespace Ederoth.SerialSniff;

public class RegexTests
{
    [Theory]
    [InlineData(1, ".e..X??2 3B47       @ 3:47    347.22  16 4    5    6    7    8    9   10   $ 3@@           ", "")]
    [InlineData(2, ".e..X??2 3B47       @ 3:47    347.22  16 4    5    6    7    8    9   10   $ 3@@           .e..X??2 3B47       @ 3:47    347.22  16 4    5    6    7    8    9   10   $ 3@@           ", "")]
    [InlineData(1, ".j..X??2 3B48       @ 3:48    348.22  16", " 4    5    6    7    8    9   10   $ 3@@           ")]
    [InlineData(2, ".e..X??2 3B47       @ 3:47    347.22  16 4    5    6    7    8    9   10   $ 3@@           .j..X??2 3B48       @ 3:48    348.22  16", " 4    5    6    7    8    9   10   $ 3@@           ")]
    [InlineData(2, ".j..X??2 3B48       @ 3:48    348.22  16", " 4    5    6    7    8    9   10   $ 3@@           .e..X??2 3B47       @ 3:47    347.22  16 4    5    6    7    8    9   10   $ 3@@           ")]
    [InlineData(3, ".e..X??2 3B47       @ 3:47    347.22  16 4    5    6    7    8    9   10   $ 3@@           .j..X??2 3B48       @ 3:48    348.22  16", " 4    5    6    7    8    9   10   $ 3@@           .e..X??2 3B47       @ 3:47    347.22  16 4    5    6    7    8    9   10   $ 3@@           ")]
    public void Test(int count, string data1, string data2)
    {
        int hits = 0;
        var arr = new List<string>(){data1, data2};
        string trunk = string.Empty;
        foreach(var data in arr)
        {
            if(data is null)
                continue;

            
            trunk += data;

        }
        
    }
}