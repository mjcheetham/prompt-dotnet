using System.IO;
using System.Text;

namespace Mjcheetham.PromptToolkit;

public static class TextReaderExtensions
{
    public static string ReadToFirst(this TextReader reader, char character)
    {
        var sb = new StringBuilder();

        int c;
        do
        {
            c = reader.Read();

            if (c == -1)
            {
                break;
            }

            sb.Append(c);

        } while (c != character);

        return sb.ToString();
    }
}
