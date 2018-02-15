/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using System;
using System.Text;
using Enigma.Utilities;
using Enigma.Settings;
using System.Reflection;
using System.IO;
using System.Security.Permissions;
using System.Collections.Generic;
using Enigma.Enums;
using Enigma.Interaction;

namespace Enigma
{
    /// <summary>
    /// The intro to the program and some user input handling.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Creates basic enigma machine, sets debug flag if debug is on, and calls MainMenu with greeting.
        /// </summary>
        /// <param name="args">Command line arguments. None implemented.</param>
        public static void Main(string[] args)
        {
            Debug.LogMethodStart();
            // Set current to default so program can be used immediately
            EnigmaMachine.Current = new EnigmaMachine(new EnigmaSettings());
#if DEBUG
            Utility.isDebugOn = true;
#endif
            MenuScreens.MainMenu(shouldGreet: true);
            Utility.CloseProgram();
        }
    }
}
