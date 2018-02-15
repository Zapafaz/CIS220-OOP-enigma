/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using Enigma.Enums;

namespace Enigma.Interfaces
{
    /// <summary>
    /// An interface to be used for anything that encrypts and decrypts characters.
    /// </summary>
    public interface ICipher
    {
        /// <summary>
        /// Encipher the <paramref name="input"/> character.
        /// </summary>
        /// <param name="input">The input character.</param>
        /// <returns>Returns the enciphered version of the character.</returns>
        char Encipher(char input);
        /// <summary>
        /// Deciphers the <paramref name="input"/> character.
        /// </summary>
        /// <param name="input">The input character.</param>
        /// <returns>Returns the deciphered version of the character.</returns>
        char Decipher(char input);
    }

    /// <summary>
    /// An interface to be used for any class that contains a Caesar (shift) cipher.
    /// </summary>
    public interface ICaesar
    {
        /// <summary>
        /// The amount to shift characters when ciphering.
        /// </summary>
        int Offset { get; }
    }

    /// <summary>
    /// An interface to be used for any class that rotates. Like a rotor.
    /// </summary>
    public interface IRotate
    {
        /// <summary>
        /// Determine if this rotating object should rotate the next one.
        /// </summary>
        /// <returns>Returns <see langword="true"/>if it should rotate, or false if not.</returns>
        bool ShouldRotateNext();
        /// <summary>
        /// Rotate this object by 1 position.
        /// </summary>
        void Increment();
    }

    /// <summary>
    /// An interface to be used for any class that simulates an enigma machine.
    /// </summary>
    public interface IEnigmaMachine
    {
        /// <summary>
        /// The name of the enigma machine.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Is the enigma machine currently decrypting?
        /// </summary>
        bool IsDecrypting { get; set; }
        /// <summary>
        /// What type of input this enigma machine is set to process.
        /// </summary>
        InputType InputType { get; set; }
        /// <summary>
        /// The extension-less filename for this enigma machine's input.
        /// </summary>
        string FileIn { get; set; }
        /// <summary>
        /// The .html filename for this enigma machine's output.
        /// </summary>
        string FileOut { get; set; }
        /// <summary>
        /// The line length this enigma machine will output text in.
        /// </summary>
        int BlockSize { get; }
        /// <summary>
        /// Encipher or decipher <paramref name="input"/> based on this enigma machine's settings.
        /// </summary>
        /// <param name="input">The character to cipher.</param>
        /// <returns>Returns the enciphered or deciphered version of the character.</returns>
        char Cipher(char input);
    }

    /// <summary>
    /// An interface to be used for any class that directly handles files.
    /// </summary>
    public interface IFileIO
    {
        /// <summary>
        /// The path being used for this instance of input or output.
        /// </summary>
        string Path { get; set; }
    }
}
