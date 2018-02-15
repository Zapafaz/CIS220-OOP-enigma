/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using System;
using System.IO;
using System.Text;
using Enigma.Interfaces;
using Enigma.Interaction;

namespace Enigma.Utilities
{
    /// <summary>
    /// File output handling.
    /// </summary>
    public class FileOutput : IFileIO
    {
        /// <summary>
        /// The file path that will be used by this instance of FileOutput
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Create a new instance of file output, given a file path to be written to.
        /// </summary>
        /// <param name="filePath">The file path that this instance will be writing to or appending to.</param>
        public FileOutput(string filePath)
        {
            Path = filePath;
        }

        /// <summary>
        /// Write to file at the outputPath, given a StringBuilder.
        /// </summary>
        /// <param name="builder">The StringBuilder containing the text to write.</param>
        /// <param name="successMessage">The message to be displayed after successfully writing to file.</param>
        /// <param name="shouldAppend">Should the file be appended to, or overwritten, if it already exists.</param>
        public void Write(StringBuilder builder, string successMessage, bool shouldAppend = false)
        {
            Debug.LogMethodStart();

            try
            {
                using (var writer = new StreamWriter(Path, shouldAppend))
                {
                    writer.WriteLine(builder.ToString());
                    if (successMessage.Length > 0)
                    {
                        ConsoleOutput.IndentWriteLine($"{successMessage}");
                    }
                }
            }
            catch (Exception e)
            {
                Error.ShowException(e);
            }
        }

        /// <summary>
        /// Write to file at the outputPath, from <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="successMessage">The message to be displayed after successfully writing to file.</param>
        /// <param name="shouldAppend">Should the file be appended to, or overwritten, if it already exists.</param>
        public void Write(string text, string successMessage, bool shouldAppend = false)
        {
            Write(new StringBuilder(text), successMessage, shouldAppend);
        }
    }
}
