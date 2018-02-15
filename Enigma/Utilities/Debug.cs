/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using Enigma.Interaction;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Enigma.Utilities
{

    class Debug
    {
        /// <summary>
        /// The log path currently being used.
        /// </summary>
        public static string LogPath { get; set; } = Utility.DEFAULT_LOG;
        /// <summary>
        /// Collects debug output, in order to log it to a file when the program is closed.
        /// </summary>
        private static StringBuilder logger = new StringBuilder();

        /// <summary>
        /// Debug method that writes output to console and/or log.
        /// </summary>
        /// <param name="writeToConsole">Whether or not to write to console if debugging is off.</param>
        /// <param name="output">The output to be written to the console.</param>
        public static void Log(bool writeToConsole, string output)
        {
            if (Utility.isDebugOn || writeToConsole)
            {
                ConsoleOutput.IndentWriteLine(output);
            }
            if (Utility.isLoggingOn)
            {
                string text = output + "\n";
                logger.Append(text);
                ToFile(text);
            }
        }

        /// <summary>
        /// Debug method that adds information about the method that called it to the log file.
        /// </summary>
        /// <param name="memberName">The name of the calling member.</param>
        /// <param name="filePath">The file path of the calling member.</param>
        /// <param name="lineNumber">The line number of the calling member.</param>
        public static void LogMethodStart([CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (Utility.isDebugOn && Utility.isLoggingOn)
            {
                string text = $"Executing method {memberName} at line {lineNumber} in file {filePath}\n";
                logger.Append(text);
                ToFile(text);
            }
        }

        /// <summary>
        /// Debug method that writes the debug log to a file.
        /// </summary>
        public static void ToFile()
        {
            if (Utility.isLoggingOn)
            {
                try
                {
                    string add = Utility.isDebugOn ? " (verbose)" : "";
                    new FileOutput(LogPath).Write(logger, $"Full debug log{add} written to {LogPath}.");
                }
                catch (Exception e)
                {
                    Error.ShowException(e);
                }
            }
        }

        /// <summary>
        /// Writes <paramref name="message"/> to the current LogPath.
        /// </summary>
        /// <param name="message">The text to append to LogPath.</param>
        public static void ToFile(string message)
        {
            if (Utility.isLoggingOn)
            {
                try
                {
                    new FileOutput(LogPath).Write(message, "", true);
                }
                catch (Exception e)
                {
                    Error.ShowException(e);
                }
            }
        }
    }
}
