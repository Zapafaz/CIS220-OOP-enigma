/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using System;
using System.Text;
using Enigma.Utilities;

namespace Enigma.Interaction
{
    /// <summary>
    /// Error messages to be displayed via the console.
    /// </summary>
    class Error : ConsoleOutput
    {
        public const string PRESS_ENTER = "Press enter to continue";

        /// <summary>
        /// An indicator that the program has found a non-standard character - whitespace, line break, or unknown - in the source being read, and is replacing it.
        /// </summary>
        /// <param name="letter">The letter that is being replaced.</param>
        /// <param name="position">The position in the line the letter was found on.</param>
        /// <param name="path">The path to the file the letter was found in.</param>
        /// <returns>Returns the replacement character.</returns>
        /// <remarks>This is a combination of error and main encryption/decryption functionality, so I'm not sure where to put it.
        /// I might have designed things differently if I had known we had to replace whitespace and
        /// line breaks during encryption/decryption when I was first working on this project.</remarks>
        public static char Replacing(char letter, int position, string path)
        {
            Debug.LogMethodStart();
            // initializing this char variable just so I can use it in Debug.Log below - its value is never used
            char replacement = '◉';
            string replacing = "";
            bool unknownReplaced = false;
            switch (letter)
            {
                // whitespace (I may have gone overboard)
                case '\x0009': // tab
                case '\x0020': // space
                case '\x00A0': // no-break space
                case '\x2000': // en quad
                case '\x2001': // em quad
                case '\x2002': // en space
                case '\x2003': // em space
                case '\x2004': // three-per-em space
                case '\x2005': // four-per-em space
                case '\x2006': // six-per-em space
                case '\x2007': // figure space
                case '\x2008': // punctuation space
                case '\x2009': // thin space
                case '\x200A': // hair space
                case '\x202F': // narrow no-break space
                case '\x205F': // medium mathematical space
                case '\x3000': // ideographic space
                case '\x180E': // mongolian vowel separator
                case '\x200B': // zero width space
                case '\x200C': // zero width non-joiner
                case '\x200D': // zero width joiner
                case '\x2060': // word joiner
                    replacement = Utility.REPLACE_SPACE;
                    replacing = "whitespace";
                    break;
                // line and page breaks
                case '\x000A': // line feed
                case '\x000C': // form feed
                case '\x000D': // carriage return
                    replacement = Utility.REPLACE_BREAK;
                    replacing = "line feed";
                    break;
                // next line
                case '\x0085': 
                    replacement = Utility.REPLACE_BLOCK;
                    replacing = "next line";
                    break;
                // encrypted space character (■, 0x25A0)
                case Utility.REPLACE_SPACE:
                    replacement = '\x0020';
                    replacing = "encrypted whitespace";
                    break;
                // encrypted line break character (▼, 0x25BC)
                case Utility.REPLACE_BREAK:
                    replacement = '\x000A';
                    replacing = "encrypted line break";
                    break;
                // encrypted next line character (▲, 0x25B2)
                case Utility.REPLACE_BLOCK:
                    replacement = '\x0085';
                    replacing = "encrypted block size line break";
                    break;
                // everything else
                default:
                    replacement = Utility.REPLACE_UNKNOWN;
                    replacing = "unknown character";
                    unknownReplaced = true;
                    break;
            }
            Debug.Log(true, $"Replacing {replacing}: '{letter}' (hex: {Utility.ToHex(letter)}) with {replacement} (hex: {Utility.ToHex(replacement)}) in output. Original location: character # {position} in: {path}");

            // prompt if an unknown character was found because it should be rare
            if (unknownReplaced)
            {
                ContinuePrompt();
            }
            return replacement;
        }

        /// <summary>
        /// An error to display when the program cannot find any of the <paramref name="extensions"/> in the current directory.
        /// </summary>
        /// <param name="extensions">The extensions that were searched for.</param>
        public static void NotFound(string[] extensions)
        {
            string all = "";
            foreach (string ext in extensions)
            {
                all += ext + ", ";
            }
            all = all.Remove(all.Length - 2);
            Debug.Log(true, $"Could not find any files that match any of these extension(s): {all}\nin current folder: {Environment.CurrentDirectory}");
            ContinuePrompt();
        }

        /// <summary>
        /// An error indicating that the user has input an invalid string.
        /// </summary>
        /// <param name="invalid">The invalid string.</param>
        /// <param name="why">A string explaining why the first string is invalid.</param>
        public static void InvalidInput(string invalid, string why)
        {
            Debug.Log(true, $"The string you entered: '{invalid}' is not valid because {why}");
            ContinuePrompt();
        }

        /// <summary>
        /// An error indicated that the user has input an invalid integer.
        /// </summary>
        /// <param name="invalid">The invalid integer.</param>
        /// <param name="whatIsIt">The context the integer was to be used in.</param>
        public static void InvalidInput(int invalid, string whatIsIt)
        {
            Debug.Log(true, $"The number you entered: '{invalid}' for {whatIsIt} is not valid.");
            ContinuePrompt();
        }

        /// <summary>
        /// An error that displays basic information about an exception to the user, then prompts them to continue.
        /// </summary>
        /// <param name="exception">The exception to show to the user.</param>
        public static void ShowException(Exception exception)
        {
            Debug.Log(true, $"{exception.GetType().Name}:\n{exception.Message}");
            ContinuePrompt();
        }

        /// <summary>
        /// An error indicating that the settings file is not properly formatted.
        /// </summary>
        /// <param name="filePath">The settings file path.</param>
        public static void InvalidSettings(string filePath)
        {
            Debug.Log(true, $"The file {filePath} is not in a valid settings file format.");
            ContinuePrompt();
        }
    }
}
