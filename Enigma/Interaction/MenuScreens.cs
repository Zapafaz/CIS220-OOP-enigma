/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using System;
using System.Collections.Generic;
using Enigma.Utilities;
using System.IO;
using System.Linq;

namespace Enigma.Interaction
{
    class MenuScreens : ConsoleOutput
    {
        private static Menu Main { get; set; }
        private static Menu Selection { get; set; }
        private static Menu Settings { get;set;}
        private static bool shouldGenerateOutputPath = true;

        /// <summary>
        /// Update, then show the Main Menu.
        /// </summary>
        public static void MainMenu(bool shouldGreet)
        {
            Debug.LogMethodStart();
            UpdateMainMenu();

            Main.DisplayItems("Main Menu", shouldGreet);
            
            int choice = Main.ItemSelect();
            switch (choice)
            {
                case 0: // Choice was cipher input (encrypt/decrypt)
                    EnigmaMachine.Current.StartCipher();
                    // Re-initialize rotors on machine that was just used to cipher since they are on different rotations now.
                    EnigmaMachine.Current.ResetRotors();
                    break;
                case 1: // Choice was settings menu
                    SettingsMenu();
                    break;
                case 2: // Choice was help
                    Help();
                    break;
                // Final choice (3 here) is always exit for Menu instances - included as part of Menu.ItemSelect
                default: // Choice was invalid (shouldn't be possible since Menu.ItemSelect includes validation)
                    break;
            }
            // Show menu again (without greeting) regardless of choice
            MainMenu(false);
        }

        /// <summary>
        /// Update the main Menu.
        /// </summary>
        public static void UpdateMainMenu()
        {
            Debug.LogMethodStart();
            
            string inputType = EnigmaMachine.Current.InputType == Enums.InputType.keyboard ? "keyboard input" : "file: " + Path.GetFileName(EnigmaMachine.Current.FileIn);
            string cipherType = EnigmaMachine.Current.IsDecrypting ? "Decrypt" : "Encrypt";
            string helpDesc = "View the help menu";
            var menuItems = new List<MenuItem>
            {
              new MenuItem($"{cipherType} Text", $"{cipherType} from {inputType}")
            , new MenuItem($"Change Settings", $"Change Enigma and file settings")
            , new MenuItem("Help", helpDesc)
            };
            Main = new Menu(menuItems);
        }

        /// <summary>
        /// Update, then show the settings Menu.
        /// </summary>
        public static void SettingsMenu()
        {
            Debug.LogMethodStart();
            UpdateSettingsMenu();
            
            Settings.DisplayItems("Settings Menu", false);
            int choice = Settings.ItemSelect();
            switch (choice)
            {
                case 0: // Choice was change folder
                    Environment.CurrentDirectory = UserInput.GetFolderFromUser();
                    break;
                case 1: // Choice was change log file
                    Debug.LogPath = UserInput.GetFilePathFromUser("log", new string[] { ".log" }, false);
                    Utility.isLoggingOn = true;
                    break;
                case 2: // Choice was change enigma machine
                    EnigmaSelectionMenu();
                    break;
                case 3: // Choice was change input
                    string input = UserInput.GetFilePathFromUser("text input", new string[] { ".txt", ".html" }, true);
                    if (input == null)
                    {
                        input = "keyboard";
                    }
                    EnigmaMachine.Current.InputType = Validation.GetInputExtensionType(input);
                    EnigmaMachine.Current.FileIn = Path.ChangeExtension(input, null);
                    break;
                case 4: // Choice was change output
                    EnigmaMachine.Current.FileOut = UserInput.GetFilePathFromUser("text output", new string[] { ".html" }, false);
                    // assume that if user sets output that they don't want auto-generated output paths anymore
                    shouldGenerateOutputPath = false;
                    break;
                case 5: // Choice was change cipher type
                    EnigmaMachine.Current.IsDecrypting = !EnigmaMachine.Current.IsDecrypting;
                    break;
                case 6: // Choice was return to main menu
                    MainMenu(false);
                    break;
                // Final choice (7) is always exit program
                default: // Choice was invalid (shouldn't be possible since Menu.ItemSelect includes validation)
                    break;
            }

            // Generate a new output path for the current enigma machine, unless user has manually set output path
            if (shouldGenerateOutputPath)
            {
                EnigmaMachine.Current.FileOut = EnigmaMachine.GetOutputPath();
            }

            // Display settings menu again if choice was not main menu
            SettingsMenu();
        }

        /// <summary>
        /// Update the settings Menu.
        /// </summary>
        public static void UpdateSettingsMenu()
        {
            Debug.LogMethodStart();

            string current = "Current: ";
            string logDesc = current;
            logDesc += Utility.isLoggingOn ? $"{Path.GetFileName(Debug.LogPath)}" : "OFF";
            string enigmaDesc = current + EnigmaMachine.Current.Name;
            string inputType = EnigmaMachine.Current.InputType == Enums.InputType.keyboard ? "keyboard input" : "file: " + Path.GetFileName(EnigmaMachine.Current.FileIn);
            string inputDesc = current + inputType;
            string extension = "." + EnigmaMachine.Current.InputType.ToString();
            if (EnigmaMachine.Current.InputType == Enums.InputType.html || EnigmaMachine.Current.InputType == Enums.InputType.txt)
            {
                inputDesc += extension;
                inputType += extension;
            }
            string outputDesc = current + Path.GetFileName(EnigmaMachine.Current.FileOut);
            string cipherType = EnigmaMachine.Current.IsDecrypting ? "Decrypt" : "Encrypt";
            string cipherTypeDesc = current + cipherType + "ing";

            var menuItems = new List<MenuItem>
            {
                  new MenuItem("Change Folder", "Change current folder")
                , new MenuItem("Change Log File", logDesc)
                , new MenuItem("Change Enigma Machine", enigmaDesc)
                , new MenuItem("Change Input", inputDesc)
                , new MenuItem("Change Output File", outputDesc)
                , new MenuItem("Change Cipher Type", cipherTypeDesc)
                , new MenuItem("Return to Main Menu", "Use these settings")
            };
            Settings = new Menu(menuItems);
        }

        /// <summary>
        /// Update, then show the Enigma Selection menu.
        /// </summary>
        public static void EnigmaSelectionMenu()
        {
            Debug.LogMethodStart();
            UpdateEnigmaSelectionMenu();
            
            Selection.DisplayItems("Enigma Selection Menu", false);
            int choice = Selection.ItemSelect();
            EnigmaMachine old = null;
            // using if/else instead of switch, because switch needs constant values
            if (choice == 0) // Choice was new from settings
            {
                string settings = UserInput.GetFilePathFromUser("enigma settings file", new string[] { ".xml" }, true);
                if (settings != null)
                {
                    old = EnigmaMachine.Current;
                    EnigmaMachine.Current = new EnigmaMachine(new FileInput(settings).GetEnigmaSettings());
                }
            }
            else if (choice == 1) // Choice was new from manual input
            {
                old = EnigmaMachine.Current;
                EnigmaMachine.Current = UserInput.ManualEnigmaCreation();
            }
            // (Items.Count - 3) because the last item is always Exit in Menu instances and 2nd to last in this case is To Main Menu
            else if (choice <= Selection.Items.Count - 3 && choice >= 2) // choice was from list of other machines
            {
                old = EnigmaMachine.Current;
                // Using choice - 2 as index because the first 2 choices in the menu are not other machines
                // e.g. to select the first enigma machine in OtherMachines we need index 0, but choice would be 2 here
                EnigmaMachine.Current = EnigmaMachine.OtherMachines[choice - 2];
            }

            // This check and the next is probably a good bit slower than using a hashtable (e.g. System.Collections.Generic.Dictionary)
            // but that shouldn't matter since OtherMachines.Count should be small-ish (i.e. iterating over it for Contains & Any should be fast)
            // I can always change it if necessary.
            if (old != null && !EnigmaMachine.OtherMachines.Contains(old))
            {
                EnigmaMachine.OtherMachines.Add(old);
            }
            if (EnigmaMachine.OtherMachines.Any(em => em == EnigmaMachine.Current))
            {
                EnigmaMachine.OtherMachines.Remove(EnigmaMachine.Current);
            }

            // All choices lead here (the "To Main Menu" choice goes straight here) excluding the Exit choice (which is the final choice in all Menu instances).
            MainMenu(false);
        }

        /// <summary>
        /// Update the Enigma Selection menu.
        /// </summary>
        public static void UpdateEnigmaSelectionMenu()
        {
            Debug.LogMethodStart();
            string change = "Change to:";
            var items = new List<MenuItem>
            {
                new MenuItem("New Enigma (settings)", "Create from XML file")
              , new MenuItem("New Enigma (manual)", "Create from manual input")
            };
            if (EnigmaMachine.OtherMachines != null && EnigmaMachine.OtherMachines.Count > 0)
            {
                foreach (EnigmaMachine machine in EnigmaMachine.OtherMachines)
                {
                    items.Add(new MenuItem(change, machine.Name));
                }
            }
            items.Add(new MenuItem("Return to Main Menu", $"Keep using: {EnigmaMachine.Current.Name}"));
            Selection = new Menu(items);
        }
    }
}
