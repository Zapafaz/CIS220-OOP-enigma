/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using System;
using Enigma.Interaction;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Enigma.Utilities
{
    /// <summary>
    /// A few static helper methods, various constants and global flags.
    /// </summary>
    static class Utility
    {
        public const string DEFAULT_LOG = @"debug.log";
        public const string XML_NAMESPACE = "43ea4a85-f507-47ef-a3e8-659b1ef2dcec";
        public const string XML_NAMESPACE_PREFIX = "aw";
        /// <summary>
        /// Replaces unknown characters in cipher. Hex: 0xFFFD
        /// </summary>
        public const char REPLACE_UNKNOWN = '�';

        /// <summary>
        /// Currently unused.
        /// </summary>
        public static char[] Linebreaks { get; } = new char[]
                                                            {
                                                            '\x000a', // line feed
                                                            '\x000c', // form feed
                                                            '\x000d', // carriage return
                                                            '\x0085'  // next line
                                                            };
        /// <summary>
        /// Replaces line feed characters (NOT carriage return) in cipher. Hex: 0x25BC
        /// </summary>
        public const char REPLACE_BREAK = '▼';

        /// <summary>
        /// Replaces blocksize line breaks in cipher. Hex: 0x25B2
        /// </summary>
        public const char REPLACE_BLOCK = '▲';

        /// <summary>
        /// Currently unused.
        /// </summary>
        public static char[] Whitespaces { get; } = new char[]
                                                            {
                                                            '\x0009', // tab
                                                            '\x0020', // space
                                                            '\x00A0', // no-break space
                                                            '\x2000', // en quad
                                                            '\x2001', // em quad
                                                            '\x2002', // en space
                                                            '\x2003', // em space
                                                            '\x2004', // three-per-em space
                                                            '\x2005', // four-per-em space
                                                            '\x2006', // six-per-em space
                                                            '\x2007', // figure space
                                                            '\x2008', // punctuation space
                                                            '\x2009', // thin space
                                                            '\x200A', // hair space
                                                            '\x202F', // narrow no-break space
                                                            '\x205F', // medium mathematical space
                                                            '\x3000', // ideographic space
                                                            '\x180E', // mongolian vowel separator
                                                            '\x200B', // zero width space
                                                            '\x200C', // zero width non-joiner
                                                            '\x200D', // zero width joiner
                                                            '\x2060'  // word joiner
                                                            };
        /// <summary>
        /// Replaces whitespace characters in cipher. Hex: 0x25A0
        /// </summary>
        public const char REPLACE_SPACE = '■';
        public static bool isDebugOn = false;
        public static bool isLoggingOn = false;

        /// <summary>
        /// Closes the program; writes log file from current session first.
        /// </summary>
        /// <param name="shouldPrompt">Whether the program should prompt the user to continue before exiting.</param>
        public static void CloseProgram(bool shouldPrompt)
        {
            Debug.LogMethodStart();

            if (isLoggingOn)
            {
                Debug.ToFile();
            }

            if (shouldPrompt)
            {
                ConsoleOutput.InputPromptWrite("Press Enter to exit the program...");
                Console.ReadLine();
            }
            Environment.Exit(0);
        }

        /// <summary>
        /// Prompts the user to press enter to close the program; writes log file from current session first.
        /// </summary>
        public static void CloseProgram()
        {
            CloseProgram(true);
        }

        /// <summary>
        /// Converts the <paramref name="input"/> to a string that represents it as hex.
        /// </summary>
        /// <param name="input">The character to be converted.</param>
        /// <returns>Returns a string representation of the hex value of the input. e.g. input: 'd' returns "0x0064"</returns>
        public static string ToHex(char input)
        {
            return $"0x{ Convert.ToInt32(input):X}";
        }

        /// <summary>
        /// Converts the <paramref name="input"/> to a string that represents it as hex.
        /// </summary>
        /// <param name="input">The character to be converted.</param>
        /// <returns>Returns a string representation of the hex value of the input. e.g. input: '100' returns "0x0064"</returns>
        public static string ToHex(int input)
        {
            return $"0x{ Convert.ToInt32(input):X}";
        }

        /// <summary>
        /// Removes all instances of <paramref name="remove"/> from the start of <paramref name="input"/>
        /// </summary>
        /// <param name="input">The input to check for instances of <paramref name="remove"/>.</param>
        /// <param name="remove">The string to remove from the start of <paramref name="input"/>.</param>
        /// <returns>Returns <paramref name="input"/> without any instances of <paramref name="remove"/> at the start of it.</returns>
        public static string RemoveAllAtStart(string input, string remove)
        {
            while (input.StartsWith(remove))
            {
                input = input.Remove(0, remove.Length);
            }
            return input;
        }

        /// <summary>
        /// Converts <paramref name="source"/> to valid HTML with <paramref name="title"/> in title tags and h1 tags.
        /// </summary>
        /// <param name="source">The source to convert to html, with br tags for line breaks.</param>
        /// <param name="title">A basic title for the converted text (e.g. what it's from).</param>
        /// <returns>Returns valid HTML with the <paramref name="source"/> enclosed in a p element.</returns>
        public static string TextToHtml(string source, string title)
        {
            var textBuilder = new StringBuilder();
            var text = source;
            try
            {
                text = text.Replace("\n", "<br>");
                // not sure how valid our HTML has to be but this shouldn't trigger any warnings in HTML validators at least
                // source includes <br> tags for line breaks
                textBuilder.Append("<!DOCTYPE html>\n");
                textBuilder.Append("<html>\n");
                textBuilder.Append("<head>\n");
                textBuilder.Append($"<title>{title}</title>\n");
                textBuilder.Append("<meta charset=\"utf-8\">\n");
                textBuilder.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />\n");
                textBuilder.Append("</head>\n");
                textBuilder.Append("<body>\n");
                textBuilder.Append($"<h1>{title}</h1>\n");
                textBuilder.Append($"<p>{text}</p>\n");
                textBuilder.Append("</body>\n");
                textBuilder.Append("</html>\n");
            }
            catch (Exception e)
            {
                Error.ShowException(e);
            }
            return textBuilder.ToString();
        }

        /// <summary>
        /// Parses HTML <paramref name="source"/> to plaintext by stripping every tag and replacing br tags with \n.
        /// </summary>
        /// <param name="source">The text to convert to plaintext.</param>
        /// <returns>Returns the <paramref name="source"/> in plaintext.</returns>
        public static string HtmlToText(string source)
        {
            var textBuilder = new StringBuilder();
            try
            {
                string line = "";
                using (var reader = new StringReader(source))
                {
                    while (reader.Peek() > -1)
                    {
                        line = reader.ReadLine();
                        if (line.Contains("<p>"))
                        {
                            line = RemoveHtmlTag(line, "<p>");
                            line = line.Replace("<br>", "\n");
                            line = line.Replace("<br />", "");
                            textBuilder.Append(line);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error.ShowException(e);
            }
            return textBuilder.ToString();
        }

        /// <summary>
        /// Remove all <paramref name="tagToRemove"/> and its closing counterpart from <paramref name="source"/> text.
        /// </summary>
        /// <param name="source">The source text.</param>
        /// <param name="tagToRemove">The tag to search for in the <paramref name="source"/>.</param>
        /// <returns>Returns <paramref name="source"/> without any <paramref name="tagToRemove"/> or its closing counterpart (e.g. for <paramref name="source"/>=&lt;p&gt;, also removes &lt;/p&gt;</returns>
        public static string RemoveHtmlTag(string source, string tagToRemove)
        {
            string closingTagToRemove = tagToRemove.Insert(1, "/");
            string output = source;
            output = output.Replace(tagToRemove, "");
            output = output.Replace(closingTagToRemove, "");
            return output;
        }
    }
}
