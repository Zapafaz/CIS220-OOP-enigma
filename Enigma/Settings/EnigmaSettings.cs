/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using Enigma.Interfaces;
using Enigma.Enums;

namespace Enigma.Settings
{
    /// <summary>
    /// Simple container class that has all the settings needed to create an enigma machine.
    /// </summary>
    public class EnigmaSettings
    {
        /// <summary>
        /// The set of rotors that the enigma machine will use.
        /// </summary>
        public Rotor[] RotorSet { get; }
        /// <summary>
        /// The name of to display to the user for this enigma machine
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Whether the enigma machine is decrypting (true), or encrypting (false)
        /// </summary>
        public bool IsDecrypting { get; }
        /// <summary>
        /// The type of the input (.html, .txt, or keyboard)
        /// </summary>
        public InputType InputType { get; }
        /// <summary>
        /// The extension-less filename to use for file input to this enigma machine
        /// </summary>
        public string FileIn { get; }
        /// <summary>
        /// The .html filename to use for file output from this enigma machine
        /// </summary>
        public string FileOut { get; }
        /// <summary>
        /// The output block size.
        /// </summary>
        public int BlockSize { get; }

        /// <summary>
        /// Default constructor for EnigmaSettings, initializes to some basic settings.
        /// </summary>
        public EnigmaSettings()
        {
            IsDecrypting =false;
            BlockSize = 5;
            RotorSet = new Rotor[] { new Rotor(), new Rotor(), new Rotor() };
            FileIn = "";
            Name ="DEFAULT";
            InputType = InputType.keyboard;
        }

        /// <summary>
        /// Constructor for the EnigmaSettings class. Used to initialize an EnigmaMachine.
        /// </summary>
        /// <param name="rotorSet">The set of rotors that will be used by this enigma machine.</param>
        /// <param name="inputType">What type of input this enigma machine will use.</param>
        /// <param name="isDecrypting">Is this enigma machine decrypting (true) or encrypting (false)?</param>
        /// <param name="blockSize">The block size to be used for encrypted output spacing.</param>
        /// <param name="fileIn">The extensionless input filename that this enigma machine will read from.</param>
        /// <param name="name">The name to display to the user for this enigma machine.</param>
        public EnigmaSettings(Rotor[] rotorSet, bool isDecrypting, int blockSize, string fileIn, string name, InputType inputType)
        {
            IsDecrypting = isDecrypting;
            BlockSize = blockSize;
            RotorSet = rotorSet;
            FileIn = fileIn;
            Name = name;
            InputType = inputType;
        }
    }
}
