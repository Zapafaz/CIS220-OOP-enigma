using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigma.util
{
    public static class RewriteCiphers
    {
        public static List<string> GetInputFromFile()
        {
            string nextLine;
            string includePunctuation;
            var lines = new List<string>();
            using (var file = new System.IO.StreamReader(@"H:\Projects\School\TextFiles\Ciphers.txt"))
            {
                while ((nextLine = file.ReadLine()) != null)
                {
                    includePunctuation = nextLine.Aggregate(string.Empty, (s, c) => s + c + "\',\'");
                    includePunctuation = "\'" + includePunctuation;
                    includePunctuation = includePunctuation.Remove(103, 2);
                    lines.Add(includePunctuation);
                }
            }
            return lines;
        }
        public static void WriteToFile(List<string> lines)
        {
            using (var file = new System.IO.StreamWriter(@"H:\Projects\School\TextFiles\CipherOutput.txt"))
            {
                lines.ForEach(file.WriteLine);
            }
        }
    }
}
