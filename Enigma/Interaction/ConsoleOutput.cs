using Enigma.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigma.Interaction
{
    class ConsoleOutput
    {
        private static string info = "Adam Wight's Enigma Machine simulator for Fall 2017\nCIS220M Object Oriented Programming (Fall 2017) at Manchester Community College.\nInstructor: Ed Cauthorn.";
        private static string debugOn = $"Debug mode is on. Log file will be extra verbose.";

        /// <summary>
        /// Greets the user at the start of the program.
        /// </summary>
        public static void Greet()
        {
            Debug.LogMethodStart();

            IndentWriteLine($"\nWelcome to {info}");
            if (Utility.isDebugOn)
            {
                IndentWriteLine($"\n{debugOn}");
            }
        }

        /// <summary>
        /// A list of basic instructions for the Enigma machine program.
        /// </summary>
        public static void Help()
        {
            Debug.LogMethodStart();

            HeaderWrite("Help & About");

            string debug = Utility.isDebugOn ? $"{debugOn}\n\n" : "";
            IndentWriteLine($"\nThis is {info}\n\n" +
                     debug +
                    "This program ciphers text by simulating a sort of Enigma machine composed\n" +
                    "of 1 or more rotors, which are used to cipher text using a Caesar cipher.\n" +
                    $"Each rotor has a rotation (0 to {Rotor.LAST_ROTATION}), a prime number offset, and a set\n" +
                    "of rotation values at which to trigger the next rotor to rotate (once).\n" +
                    "The first rotor always rotates immediately when a character is input.\n\n"
                    +
                    "Characters input to the enigma machine are sent through each rotor and\n" +
                    "ciphered based on each rotor's current rotation and offset.\n" +
                    "The next rotors only rotate when the first rotor triggers a rotation.\n" +
                    "The enigma machine also has a block size for the output, which breaks output\n" +
                    "into lines of block size; e.g. for size 5, output is 5 chars per line");
            Console.Write("\n");
            ContinuePrompt();
        }



        /// <summary>
        /// Writes <paramref name="screen"/> with the date, project, and program info
        /// </summary>
        /// <param name="screen">The text to be written alongside the other information.</param>
        public static void HeaderWrite(string screen)
        {
            Console.Clear();
            LeftRightSplitWrite($"By Adam Wight", $"{DateTime.Now.ToString("MMM dd, yyy")}");
            LeftRightSplitWrite($"Enigma Machine Simulator", screen);
        }

        /// <summary>
        /// Displays <paramref name="output"/> indented towards the center of the console with a colon after it.
        /// </summary>
        /// <param name="output">The string to output.</param>
        public static void InputPromptWrite(string output)
        {
            Console.Write(CenterAlign($"{output}: "));
        }

        /// <summary>
        /// Aligns <paramref name="desc"/> slightly to the right and under <paramref name="item"/>, near the center of the console.
        /// </summary>
        /// <param name="item">The string that should be center aligned.</param>
        /// <param name="desc">The string that should under <paramref name="item"/>.</param>
        /// <returns>Returns a string formatted with <paramref name="item"/> positioned above and to the left of <paramref name="desc"/>.</returns>
        public static string MenuItemFormat(string item, string desc)
        {
            int width = Console.WindowWidth;
            int count = width / 4;
            string padding = GetPadding(count, " ");
            if (desc.Length > 1)
            {
                return $"{padding}{item}\n{padding}   └─> {desc}";
            }
            return $"{padding}{item}";
        }

        /// <summary>
        /// Gets a string with <paramref name="count"/> instances of <paramref name="padder"/>.
        /// </summary>
        /// <param name="count">The number of <paramref name="padder"/> instances desired.</param>
        /// <param name="padder">The string or character to write #<paramref name="count"/> of.</param>
        /// <returns>Returns <paramref name="count"/> number of <paramref name="padder"/> as a string.</returns>
        public static string GetPadding(int count, string padder)
        {
            var padding = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                padding.Append(padder);
            }

            return padding.ToString();
        }

        /// <summary>
        /// Writes a line with <paramref name="left"/> at the left of the screen and <paramref name="right"/> at the right.
        /// </summary>
        /// <param name="left">The text to write at the left.</param>
        /// <param name="right">The text to write at the right.</param>
        public static void LeftRightSplitWrite(string left, string right)
        {
            int padLeft = Console.WindowWidth - left.Length - 4;
            // This is so String.PadLeft doesn't cause an exception if window width is very small
            if (padLeft < 0)
            {
                Console.WriteLine($"  {left}\t{right}");
            }
            else
            {
                Console.WriteLine($"  {left}{right.PadLeft(padLeft)}");
            }
        }

        /// <summary>
        /// Writes <paramref name="text"/> in the center of the console.
        /// </summary>
        /// <param name="text">The text to write in the center of the console.</param>
        public static string CenterAlign(string text)
        {
            var centered = ((Console.WindowWidth / 2) + (text.Length / 2));
            return text.PadLeft(centered);
        }

        /// <summary>
        /// Writes <paramref name="output"/> with <paramref name="indentSize"/> spaces before and after it.
        /// </summary>
        /// <param name="output">The output to write.</param>
        /// <param name="indentSize">The amount of spaces to write before and after each line of output.</param>
        /// <remarks>Not sending <paramref name="indentSize"/> from anywhere right now.</remarks>
        public static void IndentWriteLine(string output, int indentSize = 2)
        {
            string padding = GetPadding(indentSize, " ");
            using (var reader = new StringReader(output))
            {
                while (reader.Peek() > -1)
                {
                    string line = $"{padding}{reader.ReadLine()}";
                    int maxWidth = Console.WindowWidth - indentSize * 2;
                    if (line.Length > maxWidth)
                    {
                        char[] letters = line.ToCharArray();
                        for (int i = 0; i < letters.Length; i++)
                        {
                            if (i > 0 && i % maxWidth == 0)
                            {
                                Console.Write($"\n{padding}");
                            }
                            Console.Write(letters[i]);
                        }
                        Console.Write("\n");
                    }
                    else
                    {
                        Console.WriteLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// Prompts the user to continue by pressing enter.
        /// </summary>
        public static void ContinuePrompt()
        {
            InputPromptWrite(Error.PRESS_ENTER);
            Console.ReadLine();
        }
    }
}
