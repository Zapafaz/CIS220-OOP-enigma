/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using Enigma.Enums;
using System;
using System.IO;
using Enigma.Interaction;
using System.Collections;

namespace Enigma.Utilities
{
    /// <summary>
    /// Static methods for user input validation that print errors if they fail.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Check if the program has the necessary IO permissions for <paramref name="path"/> by trying to get the files in it, then creating a subfolder in it. Deletes the subfolder, if created.
        /// </summary>
        /// <param name="path">The path to check for permissions.</param>
        /// <param name="needWrite">Does the program need write access for this path?</param>
        /// <returns>Returns true if the program has read and/or write permissions for <paramref name="path"/>, false if not.</returns>
        public static bool HaveIoPermissions(string path, bool needWrite)
        {
            Debug.LogMethodStart();

            string writeTest = path;
            bool hasExtension = Path.HasExtension(path);
            if (!hasExtension && !EndsWithSeparator(path))
            {
                // Add separator character if needed
                writeTest += Path.DirectorySeparatorChar;
            }

            // Use current system time to add a random string, just to avoid existing directory names
            // Should be more than enough entropy (in theory); no need to instantiate System.Random
            writeTest += DateTime.Now.GetHashCode();
            try
            {
                // Test read access - first, get root if path isn't a directory
                if (!Directory.Exists(path))
                {
                    path = Path.GetPathRoot(path);
                }
                else
                {
                    Directory.GetFiles(path);
                }
                // Test write access if needed, clean up
                if (needWrite)
                {
                    Directory.CreateDirectory(writeTest);
                    Directory.Delete(writeTest);
                }
                return true;
            }
            catch (Exception e)
            {
                Error.ShowException(e);
            }
            return false;
        }

        /// <summary>
        /// Checks whether <paramref name="path"/> is an existing .txt or .html file
        /// </summary>
        /// <param name="path">The path to check (with or without extension)</param>
        /// <returns>Returns InputType.html if <paramref name="path"/> exists as an .html file, InputType.txt if it exists as a .txt file, otherwise returns FileExtension.none.</returns>
        public static InputType GetInputExtensionType(string path)
        {
            Debug.LogMethodStart();

            try
            {
                if (path == "keyboard")
                {
                    return InputType.keyboard;
                }
                if ((Path.HasExtension(path) && Path.GetExtension(path) == ".html")
                    || File.Exists(path + ".html"))
                {
                    return InputType.html;
                }
                else if ((Path.HasExtension(path) && Path.GetExtension(path) == ".txt")
                         || File.Exists(path + ".txt"))
                {
                    return InputType.txt;
                }
            }
            catch (Exception e)
            {
                Error.ShowException(e);
            }
            return InputType.none;
        }

        /// <summary>
        /// Checks if <paramref name="path"/> ends with a valid directory separator.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>Returns true if <paramref name="path"/> ends with a valid directory separator, otherwise false.</returns>
        public static bool EndsWithSeparator(string path)
        {
            Debug.LogMethodStart();

            return path.EndsWith(Path.DirectorySeparatorChar.ToString()) || path.EndsWith(Path.AltDirectorySeparatorChar.ToString());
        }

        /// <summary>
        /// Ensures the <paramref name="input"/> character is an ASCII character from dec 33 (!) to dec 126 (~)
        /// </summary>
        /// <param name="input">The given input characters</param>
        /// <returns>Returns true if <paramref name="input"/> is between 33 and 126 in decimal, otherwise returns false.</returns>
        public static bool IsValid(char input)
        {
            Debug.LogMethodStart();

            return input <= Rotor.END_OF_ASCII && input >= Rotor.START_OF_ASCII;
        }

        /// <summary>
        /// Check if <paramref name="x"/> is prime by checking if x % n == 0, for integers n where (x / 2) > n > 2
        /// </summary>
        /// <param name="x">The number to be checked.</param>
        /// <returns>Returns true if <paramref name="x"/> is a prime number, false if not.</returns>
        public static bool IsPrime(int x)
        {
            Debug.LogMethodStart();

            // 0, 1, and negative numbers are not prime, but are not caught by the next algorithm.
            if (x <= 1)
            {
                return false;
            }
            else
            {
                for (int n = 2; n <= x / 2; n++)
                {
                    if (x % n == 0)
                    {
                        Error.InvalidInput(x, "prime number");
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Check if <paramref name="check"/> is a valid rotation for a rotor.
        /// </summary>
        /// <param name="check">The number to check.</param>
        /// <returns>Returns true if <paramref name="check"/> is valid, otherwise returns false.</returns>
        public static bool IsValidRotation(int check)
        {
            Debug.LogMethodStart();

            if (check <= Rotor.LAST_ROTATION && check >= 0)
            {
                return true;
            }
            Error.InvalidInput(check, "rotor rotation");
            return false;
        }

        /// <summary>
        /// Determines if <paramref name="check"/> is greater than 0.
        /// </summary>
        /// <param name="check">The integer to check.</param>
        /// <returns>Returns true if <paramref name="check"/> is greater than 0, else returns false</returns>
        /// <remarks>This exists so it can be used as a Func with UserInput.GetIntFromUser</remarks>
        public static bool IsGreaterThan0(int check)
        {
            Debug.LogMethodStart();

            return check > 0;
        }

        /// <summary>
        /// Check if <paramref name="check"/> is within <paramref name="range"/> (exclusive).
        /// </summary>
        /// <param name="check">The number to check.</param>
        /// <param name="range">The range to test <paramref name="check"/> against.</param>
        /// <returns>Returns true if check is less than <paramref name="range"/>.Upper and greater than <paramref name="range"/>.Lower, otherwise returns false</returns>
        public static bool IsWithinRange(int check, IntRange range)
        {
            Debug.LogMethodStart();
            if (check < range.Upper && check > range.Lower)
            {
                return true;
            }
            Error.InvalidInput(check, $"a number in range greater than {range.Lower} and less than {range.Upper}");
            return false;
        }

        /// <summary>
        /// A range of integers with lower and upper bounds.
        /// </summary>
        public struct IntRange
        {
            /// <summary>
            /// The lowest number possible for this range.
            /// </summary>
            public int Lower { get; }
            /// <summary>
            /// The highest number possible for this range.
            /// </summary>
            public int Upper { get; }
            /// <summary>
            /// Create a range of integers with a <paramref name="lowerBound"/> and an <paramref name="upperBound"/>
            /// </summary>
            /// <param name="lowerBound">The lowest number possible for the range.</param>
            /// <param name="upperBound">The highest number possible for the range.</param>
            public IntRange(int lowerBound, int upperBound)
            {
                if (lowerBound > upperBound)
                {
                    Upper = lowerBound;
                    Lower = upperBound;
                }
                else if (lowerBound < upperBound)
                {
                    Upper = upperBound;
                    Lower = lowerBound;
                }
                else
                {
                    throw new ArgumentException($"Cannot create IntRange because {lowerBound} equals {upperBound}.");
                }
            }
        }
    }
}
