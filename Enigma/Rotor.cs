/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using System;
using System.Linq;
using Enigma.Interfaces;
using System.Text;
using Enigma.Settings;
using Enigma.Utilities;

namespace Enigma
{
    /// <summary>
    /// The Rotor class; encrypts using a caesar cipher.
    /// </summary>
    /// <remarks>Real Enigma machines have 3 rotors, and use a substitution cipher to encrypt.</remarks>
    public class Rotor : ICipher, ICaesar, IRotate
    {
        /// <summary>
        /// The total number of rotations that rotors have (0-indexed)
        /// </summary>
        public const int SIZE = 93;
        /// <summary>
        /// The last possible rotation (0-indexed) value of rotors.
        /// </summary>
        public const int LAST_ROTATION = 92;
        /// <summary>
        /// The first valid character, !, in decimal ASCII.
        /// </summary>
        public const int START_OF_ASCII = 33;
        /// <summary>
        /// The last valid character, ~, in decimal ASCII.
        /// </summary>
        public const int END_OF_ASCII = 126;

        private int rotation;
        private int[] toRotateNext;

        /// <summary>
        /// The prime number offset being used by this rotor.
        /// </summary>
        public int Offset { get; }

        private int originalRotation;

        /// <summary>
        /// Default constructor for the Rotor class. Initializes rotor to basic settings: rotation 0, toRotateNext 92, offset 3
        /// </summary>
        public Rotor()
        {
            rotation = 0;
            originalRotation = 0;
            toRotateNext = new int[] { LAST_ROTATION };
            Offset = 3;
        }

        /// <summary>
        /// Constructor for the Rotor class. Uses the RotorSettings class to initialize.
        /// </summary>
        /// <param name="settings">The set of rotor settings this rotor should use.</param>
        public Rotor(RotorSettings settings)
        {
            Offset = settings.Offset;
            rotation = settings.Rotation;
            originalRotation = settings.Rotation;
            toRotateNext = settings.ToRotateNext;
        }

        /// <summary>
        /// Resets the rotation for this rotor to its original rotation.
        /// </summary>
        public void ResetRotation()
        {
            rotation = originalRotation;
        }

        /// <summary>
        /// Takes the input character and enciphers it based on the offset of this rotor and the current rotation.
        /// </summary>
        /// <param name="input">The character to be enciphered.</param>
        /// <returns>Returns the enciphered character.</returns>
        public char Encipher(char input)
        {
            Debug.LogMethodStart();

            // Makes the input such that 33 (!) is 0
            int zeroIndexedAscii = input - START_OF_ASCII;

            // Get the "offset" by taking the remainder of Offset and SIZE
            // e.g. Offset 101 % SIZE 93 = 7 (went around rotor once)
            // e.g. Offset 277 % SIZE 93 = 92 (went around rotor twice)
            // e.g. Offset 887 % SIZE 93 = 41 (went around rotor nine times)
            int remainder = Offset % SIZE;

            // Applies the offset to that zero indexed number
            int offsetApplied = zeroIndexedAscii + remainder;

            // Applies the current rotation to the offset integer
            int rotated = offsetApplied + rotation;

            int enciphered = 0;

            // If the rotated number is smaller than the size of the rotor, then that's the number (character) to return.
            if (rotated < SIZE)
            {
                enciphered = rotated;
            }

            // If the rotated + offset number is smaller than the size of the rotor times 2, return the number after subtracting the rotor size.
            // i.e. it rotated through the character set once
            else if (rotated < SIZE * 2)
            {
                enciphered = rotated - SIZE;
            }

            // Lastly, if the rotated + offset number is too large for either of the other cases, return the number after subtracting the rotor's size times 2
            // i.e. it rotated through the character set twice
            else
            {
                enciphered = rotated - (SIZE * 2);
            }

            // Add the start back so it returns the correct character in ASCII
            // e.g. if enciphered = 2, return 2 + 33 = 35 (char: #)
            return (char)(enciphered + START_OF_ASCII);
        }

        /// <summary>
        /// Deciphers the given input character.
        /// </summary>
        /// <param name="input">The character to be deciphered.</param>
        /// <returns>Returns the deciphered character.</returns>
        /// <remarks>See Rotor.Encipher for detailed comments; this works virtually the same but
        /// in reverse by subtracting remainder and rotation instead of adding them.</remarks>
        public char Decipher(char input)
        {
            Debug.LogMethodStart();
            int zeroIndexedAscii = input - START_OF_ASCII;
            int remainder = Offset % SIZE;
            int offsetApplied = zeroIndexedAscii - remainder;
            int rotated = offsetApplied - rotation;
            int enciphered = 0;

            if (rotated + SIZE <= 0)
            {
                enciphered = rotated + (SIZE * 2);
            }
            else if (rotated <= 0)
            {
                enciphered = rotated + SIZE;
            }
            else
            {
                enciphered = rotated;
            }
            return (char)(enciphered + START_OF_ASCII);
        }

        /// <summary>
        /// Increments the rotor's rotation by 1. Since the rotor has 93 rotation positions (incl 0), if rotation == 92 (i.e. SIZE - 1), rotation resets to 0.
        /// </summary>
        public void Increment()
        {
            Debug.LogMethodStart();

            if (rotation < LAST_ROTATION)
            {
                rotation++;
            }
            else
            {
                rotation = 0;
            }
        }

        /// <summary>
        /// Checks the rotor to see if it should rotate the next rotor in line.
        /// </summary>
        /// <returns>Returns true if the next rotor should be rotated, false if not.</returns>
        public bool ShouldRotateNext()
        {
            Debug.LogMethodStart();

            return toRotateNext.Contains(rotation);
        }

        /// <summary>
        /// Debugging method to show status of the rotor.
        /// </summary>
        /// <param name="rotorNum">The number of the rotor to show status of (i.e. which order it's in, in the Enigma)</param>
        public string ShowRotationProgress(int rotorNum)
        {
            Debug.LogMethodStart();

            var builder = new StringBuilder();
            for (int i = 0; i < toRotateNext.Length; i++)
            {
                builder.Append($"Rotor #{rotorNum + 1} rotation: {rotation}; rotate next at: {toRotateNext[i]} (char: {(char)(toRotateNext[i] + START_OF_ASCII)})\n");
            }
            return builder.ToString();
        }
    }
}
