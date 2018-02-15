/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using Enigma.Interfaces;
using Enigma.Utilities;
using Enigma.Settings;
using Enigma.Enums;
using Enigma.Interaction;
using System.Text;
using System.Collections.Generic;
using System;
using System.Net;

namespace Enigma
{
    /// <summary>
    /// Contains the parts of the virtual enigma machine.
    /// </summary>
    public class EnigmaMachine : IEnigmaMachine
    {
        /// <summary>
        /// The currently used enigma machine.
        /// </summary>
        public static EnigmaMachine Current { get; set; }
        /// <summary>
        /// The set of enigma machines not currently in use.
        /// </summary>
        public static List<EnigmaMachine> OtherMachines { get; set; } = new List<EnigmaMachine>();

        /// <summary>
        /// The name of to display to the user for this enigma machine
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Whether this enigma machine is decrypting (true), or encrypting (false)
        /// </summary>
        public bool IsDecrypting { get; set; }
        /// <summary>
        /// The type of the input (.html, .txt, or keyboard)
        /// </summary>
        public InputType InputType { get; set; }
        /// <summary>
        /// The extension-less filename to use for file input to this enigma machine
        /// </summary>
        public string FileIn { get; set; }
        /// <summary>
        /// The .html filename to use for file output from this enigma machine
        /// </summary>
        public string FileOut { get; set; }
        /// <summary>
        /// The output block size.
        /// </summary>
        public int BlockSize { get; }

        private Rotor[] rotorSet;

        /// <summary>
        /// Constructor for the EnigmaMachine class.
        /// </summary>
        /// <param name="settings">The settings that will be used to initialize this EnigmaMachine.</param>
        public EnigmaMachine(EnigmaSettings settings)
        {
            rotorSet = settings.RotorSet;
            IsDecrypting = settings.IsDecrypting;
            BlockSize = settings.BlockSize;
            Name = settings.Name;
            FileIn = settings.FileIn;
            FileOut = GetOutputFilename(settings.FileIn, settings.Name, settings.IsDecrypting);
            InputType = settings.InputType;
        }

        /// <summary>
        /// Resets the rotors in this enigma machine to their original rotation.
        /// </summary>
        public void ResetRotors()
        {
            foreach (Rotor rotor in rotorSet)
            {
                rotor.ResetRotation();
            }
        }

        /// <summary>
        /// Encrypts or decrypts text using the current Enigma machine.
        /// </summary>
        public void StartCipher()
        {
            Debug.LogMethodStart();
            string type;
            FileInput inputFile;
            if (IsDecrypting)
            {
                type = "Decrypt";
            }
            else
            {
                type = "Encrypt";
            }
            ConsoleOutput.IndentWriteLine($"Now starting {type.ToLower()}ion with Enigma Machine: {Current.Name}");
            ConsoleOutput.ContinuePrompt();
            string toCipher = "";
            switch (InputType)
            {
                case InputType.html:
                    inputFile = new FileInput(FileIn + ".html");
                    toCipher = Utility.HtmlToText(inputFile.Read());
                    break;
                case InputType.txt:
                    inputFile = new FileInput(FileIn + ".txt");
                    toCipher = inputFile.Read();
                    break;
                case InputType.keyboard:
                    toCipher = UserInput.GetStringFromUser($"Enter text to be {type.ToLower()}ed");
                    inputFile = null;
                    break;
                default:
                    InputType = InputType.keyboard;
                    inputFile = null;
                    StartCipher();
                    break;
            }

            if (toCipher?.Length > 0)
            {
                Debug.Log(false, $"INPUT:\n {toCipher}");
                var outputFile = new FileOutput(FileOut);
                var builder = new StringBuilder();
                string output = Cipher(toCipher.ToCharArray(), inputFile);
                builder.Append(Utility.TextToHtml(output, $"{type}ion output from Enigma Machine: {Current.Name}"));
                outputFile.Write(builder, ($"Wrote {type.ToLower()}ed output to file {outputFile.Path}."));
                ConsoleOutput.IndentWriteLine($"{type}ion complete. Resetting {Current.Name}'s rotors to initial settings.");
            }
            else
            {
                ConsoleOutput.IndentWriteLine($"Error: could not find anything to {type.ToLower()}");
            }
            ConsoleOutput.ContinuePrompt();
        }

        /// <summary>
        /// Gets an output filename (.html) based on the given values.
        /// </summary>
        /// <param name="input">The input filename for the enigma machine. If empty, defaults to "kb"</param>
        /// <param name="name">The name of the enigma machine this filename is for.</param>
        /// <param name="isDecrypting">Whether the enigma machine is currently in decryption mode.</param>
        /// <returns>Returns an output filename with extension .html.</returns>
        public static string GetOutputFilename(string input, string name, bool isDecrypting)
        {
            Debug.LogMethodStart();
            if (input == null || input.Length == 0)
            {
                input = "kb";
            }
            string output;
            output = input;
            output += isDecrypting ? $"_dec_" : $"_enc_";
            output += name + ".html";
            return output;
        }

        /// <summary>
        /// Shortcut for GetOutputPath with current enigma machine values as params.
        /// </summary>
        /// <returns>Returns an output filename with extension .html.</returns>
        public static string GetOutputPath()
        {
            return GetOutputFilename(Current.FileIn, Current.Name, Current.IsDecrypting);
        }

        /// <summary>
        /// Encipher or decipher the given input character using the Enigma Machine's series of substitution ciphers.
        /// </summary>
        /// <param name="input">The letter to be ciphered.</param>
        /// <returns>Returns a ciphered character.</returns>
        public char Cipher(char input)
        {
            Debug.LogMethodStart();

            EnigmaRotorRotation();
            char output = input;
            for (int i = 0; i < rotorSet.Length; i++)
            {
                input = output;
                if (IsDecrypting)
                {
                    output = rotorSet[i].Decipher(input);
                }
                else
                {
                    output = rotorSet[i].Encipher(input);
                }

                Debug.Log(false, $"Rotor #{i + 1} input: {input}, output: {output}");
            }
            return output;
        }

        /// <summary>
        /// Enciphers or deciphers the given character array.
        /// </summary>
        /// <param name="input">The character input given by the user.</param>
        /// <param name="original">The file that the input text was originally from.</param>
        /// <returns>Returns a string with the ciphered input.</returns>
        public string Cipher(char[] input, FileInput original)
        {
            Debug.LogMethodStart();

            string source = original == null ? "keyboard input" : original.Path;
            var textWithoutBr = new StringBuilder();
            var text = new StringBuilder();
            int count = 0;
            char letter;
            for (int i = 0; i < input.Length; i++)
            {
                // ignore carriage returns (\r) - only line feed (\n) counted for line breaks
                // this is so windows style line breaks (\r\n) don't get counted twice
                // does not catch line breaks from macOS older than macOS X, which use \r alone.
                // but I figure that's far beyond the scope of this program anyway
                if (input[i] == '\r')
                {
                    continue;
                }
                if (Validation.IsValid(input[i]))
                {
                    letter = Cipher(input[i]);
                }
                else
                {
                    letter = Error.Replacing(input[i], i, source);
                }
                text.Append(letter);
                textWithoutBr.Append(letter);
                count++;
                if (count % BlockSize == 0 && !IsDecrypting)
                {
                    text.Append("<br />");
                    textWithoutBr.Append("\n");
                }
            }
            ConsoleOutput.IndentWriteLine("OUTPUT START\n");
            ConsoleOutput.IndentWriteLine(textWithoutBr.ToString());
            ConsoleOutput.IndentWriteLine("\nOUTPUT END");
            return text.ToString();
        }

        /// <summary>
        /// Increments rotors. Called before input is sent through the rotors.
        /// </summary>
        private void EnigmaRotorRotation()
        {
            Debug.LogMethodStart();
            // First rotor is always incremented
            rotorSet[0].Increment();
            for (int i = 0; i < rotorSet.Length; i++)
            {
                // Other rotors are only incremented if current rotor should rotate them and is not the last rotor
                if (i + 1 < rotorSet.Length && rotorSet[i].ShouldRotateNext())
                {
                    rotorSet[i + 1].Increment();
                }
                Debug.Log(false, rotorSet[i].ShowRotationProgress(i));
            }
        }
    }
}
