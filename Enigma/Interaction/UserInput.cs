/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using Enigma.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Enigma.Utilities;
using static Enigma.Utilities.Validation;

namespace Enigma.Interaction
{
    /// <summary>
    /// Various user interaction screens in the console.
    /// </summary>
    class UserInput : ConsoleOutput
    {
        /// <summary>
        /// Asks a question with only 2 possible answers from the user.
        /// </summary>
        /// <param name="question">The question to be asked.</param>
        /// <param name="answerTrue">The answer that results in return true.</param>
        /// <param name="answerFalse">The answer that results in return false.</param>
        /// <returns>Returns true if <paramref name="answerTrue"/> is input, false if <paramref name="answerFalse"/> is input, and itself (asks again) if neither is input.</returns>
        /// <remarks>This might be a little harder to understand the code, but the other option is 2 (or more) separate, but very similar methods.</remarks>
        public static bool AskQuestion(string question, string answerTrue, string answerFalse)
        {
            Debug.LogMethodStart();

            IndentWriteLine(question);
            InputPromptWrite($"Input {answerTrue} or {answerFalse}");
            string userInput = Console.ReadLine();
            
            // Can't use a switch here since switches need constant case statements
            // Check if first character in answerTrue/answerFalse match
            // If they match, can't check userInput against it, so just use the full strings
            if (answerTrue.Trim().Substring(0, 1) == answerFalse.Trim().Substring(0, 1))
            {
                if (userInput == answerTrue)
                {
                    return true;
                }
                else if (userInput == answerFalse)
                {
                    return false;
                }
            }
            // Since first character of answerTrue/answerFalse don't match, can check userInput against them
            else
            {
                if (userInput == answerTrue || userInput == answerTrue.Trim().Substring(0, 1))
                {
                    return true;
                }
                else if (userInput == answerFalse || userInput == answerFalse.Trim().Substring(0, 1))
                {
                    return false;
                }
            }

            // If user didn't input anything that matches, tell them and try again
            Error.InvalidInput(userInput, $"answer must be {answerTrue} or {answerFalse}");
            return AskQuestion(question, answerTrue, answerFalse);
        }

        /// <summary>
        /// Gets a file matching the given extension from the user's input.
        /// </summary>
        /// <param name="fileType">The type of file to ask the user for (e.g. settings, input, etc).</param>
        /// <param name="fileExtensions">What extension(s) the file should have (e.g. .xml, .log, .txt, etc)</param>
        /// <param name="isInputFile">Is this request for an input file (true) or an output file (false)?</param>
        /// <returns>Returns the user's input (a valid file path), or returns itself (runs again) if input is invalid.</returns>
        public static string GetFilePathFromUser(string fileType, string[] fileExtensions, bool isInputFile)
        {
            Debug.LogMethodStart();
            if (isInputFile)
            {
                return FileSelectMenu(fileExtensions);
            }
            IndentWriteLine($"\nCurrent directory: {Environment.CurrentDirectory}");
            string extensionList = "";
            foreach (string ext in fileExtensions)
            {
                extensionList += ext + ", ";
            }
            extensionList = extensionList.Remove(extensionList.Length - 2);
            IndentWriteLine("File will be written in current directory.");
            InputPromptWrite($"Input a {fileType} file with a valid extension: {extensionList}");
            string userInput = Console.ReadLine();
            try
            {
                if (Path.GetInvalidFileNameChars().Any(c => userInput.Contains(c)))
                {
                    Error.InvalidInput(userInput, $"the input contains invalid characters. (Make sure you don't use :, /, or \\)");
                }
                if (fileExtensions.Any(s => s == Path.GetExtension(userInput)))
                {
                    if (File.Exists(userInput))
                    {
                        IndentWriteLine($"Chosen output path already has an existing file: {userInput}");
                        if (!AskQuestion($"Overwrite {userInput}?", "yes", "no"))
                        {
                            return GetFilePathFromUser(fileType, fileExtensions, isInputFile);
                        }
                    }
                    return userInput;
                }
                Error.InvalidInput(userInput, $"the file extension did not match any of the following: {extensionList}");
            }
            catch (Exception e)
            {
                Error.ShowException(e);
            }
            return GetFilePathFromUser(fileType, fileExtensions, isInputFile);
        }

        /// <summary>
        /// Gets a list of files in the current directory that match an extension in <paramref name="fileExtensions"/>
        /// </summary>
        /// <param name="fileExtensions">A set of file extensions that the current directory will be checked for.</param>
        /// <returns>Returns null if no matches are found, or the user's chosen file path if one is found.</returns>
        public static string FileSelectMenu(string[] fileExtensions)
        {
            Debug.LogMethodStart();

            // get files in path where file extension matches any extension in fileExtensions
            List<string> paths = Directory.GetFiles(Environment.CurrentDirectory).Where(p => fileExtensions.Any(e => e == Path.GetExtension(p))).ToList();
            if (paths.Count < 1)
            {
                Error.NotFound(fileExtensions);
                return null;
            }
            string menuName = "";
            string cipherType = EnigmaMachine.Current.IsDecrypting ? "decrypt" : "encrypt";
            bool addedKeyboardOption = false;

            // Add an option for keyboard input if this is not file select for settings file
            if (fileExtensions[0] != ".xml")
            {
                addedKeyboardOption = true;
                menuName = "Input File Menu";
                paths.Insert(0, $"Use keyboard for {cipherType}ion input");
            }
            else
            {
                menuName = "Settings File Menu";
            }
            var menuItems = new List<MenuItem>();

            // Check if added keyboard option since settings file menu doesn't include that for first option
            // If we did, need to start at index 1 and manually add keyboard option
            // Otherwise first option in settings menu shows full path instead of relative
            int startIndex = 0;
            if (addedKeyboardOption)
            {
                startIndex = 1;
                menuItems.Add(new MenuItem(paths[0], ""));
            }
            for (int i = startIndex; i < paths.Count; i++)
            {
                menuItems.Add(new MenuItem(Path.GetFileName(paths[i]), ""));
            }
            var menu = new Menu(menuItems);
            menu.DisplayItems(menuName, false);
            int choice = menu.ItemSelect();
            // 0 means user chose keyboard input if this was not settings file select, since we inserted "Keyboard input" at index 0 above.
            if (choice == 0 && addedKeyboardOption)
            {
                return null;
            }
            return paths[choice];
        }

        /// <summary>
        /// Gets the directory to use for this program from the user.
        /// </summary>
        /// <returns>Returns a valid directory.</returns>
        public static string GetFolderFromUser()
        {
            Debug.LogMethodStart();

            InputPromptWrite("Input the directory to use (or .. to navigate up)");
            string userInput = Console.ReadLine();
            string navigator;
            try
            {
                // Get parent/grandparent/grand-grandparent/etc based on how many .. were input
                // There's probably an easier way to do this section...
                if (userInput.StartsWith(".."))
                {
                    int periods = userInput.Count(c => c == '.');
                    // go up one ahead of time since otherwise we're calling GetParent(CurrentDirectory) in the loop and it doesn't work right
                    navigator = Directory.GetParent(Environment.CurrentDirectory).ToString();
                    // Remove 2 leading periods each time we call GetParent
                    userInput = userInput.Remove(0, 2);
                    userInput = Utility.RemoveAllAtStart(userInput, "/");
                    userInput = Utility.RemoveAllAtStart(userInput, "\\");
                    // subtract 2 from periods since we already called GetParent once
                    periods -= 2;
                    // start at 1 so accidentally typing in "..." doesn't go up 2 directories
                    for (int i = 1; i < periods; i+=2)
                    {
                        navigator = Directory.GetParent(navigator).ToString();
                        userInput = userInput.Remove(0, 2);
                        userInput = Utility.RemoveAllAtStart(userInput, "/");
                        userInput = Utility.RemoveAllAtStart(userInput, "\\");
                    }
                    // Remove any leftover leading . before re-combining path, so user can input e.g. ../Folder to navigate to parent -> subfolder of parent
                    userInput = Path.Combine(navigator, Utility.RemoveAllAtStart(userInput, "."));
                }
                // Get parent of user input if they input a file path
                if (Path.HasExtension(userInput))
                {
                    userInput = Directory.GetParent(userInput).ToString();
                }
                
                if (!Directory.Exists(userInput))
                {
                    if (AskQuestion($"Directory does not exist. Create new directory {Path.GetFullPath(userInput)}?", "yes", "no"))
                    {
                        Directory.CreateDirectory(userInput);
                        return userInput;
                    }
                    return GetFolderFromUser();
                }
            }
            catch (NullReferenceException)
            {
                Error.ShowException(new ArgumentException($"{userInput} is already the root drive."));
                return GetFolderFromUser();
            }
            catch (Exception e)
            {
                Error.ShowException(e);
                return GetFolderFromUser();
            }

            // Don't need the rest in try/catch because any further exceptions should be caught by this method
            if (!HaveIoPermissions(userInput, true))
            {
                return GetFolderFromUser();
            }
            
            if (Directory.GetParent(userInput) != null && EndsWithSeparator(userInput))
            {
                // Remove separator character for consistency with defaults
                userInput = userInput.Remove(userInput.Length - 1);
            }
            return userInput;
        }

        /// <summary>
        /// Directs user through manual Enigma machine creation.
        /// </summary>
        /// <returns>Returns the user-created Enigma machine.</returns>
        public static EnigmaMachine ManualEnigmaCreation()
        {
            Debug.LogMethodStart();
            string name = GetStringFromUser("Enter a short name for this enigma machine.");
            var rotorSet = new Rotor[3];
            for (int i = 0; i < rotorSet.Length; i++)
            {
                rotorSet[i] = RotorCreation((i + 1), (i == rotorSet.Length - 1));
            }
            int blockSize = GetIntFromUser("What block size should the enigma machine output in?", "an integer greater than 0", Validation.IsGreaterThan0);
            IndentWriteLine("Enigma machine created from user-input settings.");
            // Assume encryption and keyboard input for new enigma machine - can be changed from EnigmaMenus.MainMenu anyway
            return new EnigmaMachine(new EnigmaSettings(rotorSet, false, blockSize, "", name, Enums.InputType.keyboard));
        }

        /// <summary>
        /// Guides the user through creating a rotor.
        /// </summary>
        /// <param name="rotorNum">The (1-indexed) order of the rotor in the Enigma machine.</param>
        /// <param name="isLastRotor">Is this the last rotor in the Enigma machine?</param>
        /// <returns>Returns a rotor for use in an Enigma machine.</returns>
        public static Rotor RotorCreation(int rotorNum, bool isLastRotor)
        {
            Debug.LogMethodStart();
            
            int rotation = GetIntFromUser($"What rotation should rotor #{rotorNum} start at?", $"an integer from 0 to {Rotor.LAST_ROTATION}", Validation.IsValidRotation);
            int offset = GetIntFromUser($"What offset should rotor #{rotorNum} use?", "a prime number", Validation.IsPrime);
            int next = Rotor.LAST_ROTATION;
            if (!isLastRotor)
            {
                next = GetIntFromUser($"At what rotation should rotor #{rotorNum} rotate the next rotor?", $"an integer from 0 to {Rotor.LAST_ROTATION}", Validation.IsValidRotation);
            }

            // Creates rotor with single toRotateNext. If user wants a set, they can use a settings file.
            // Asking for input for every possible toRotateNext value for each rotor is far too clumsy.
            return new Rotor(new RotorSettings(offset, rotation, new int[] { next }));
        }

        /// <summary>
        /// Gets a valid integer from the user, based on the given parameters.
        /// </summary>
        /// <param name="question">The question (i.e. what the int will be used for) to ask the user.</param>
        /// <param name="validInput">An indication of what valid input is, for the user.</param>
        /// <param name="validator">A method to ensure the input matches the requested validInput.</param>
        /// <returns>Returns a validated integer from the user's input.</returns>
        public static int GetIntFromUser(string question, string validInput, Func<int, bool> validator)
        {
            Debug.LogMethodStart();

            bool isValidNumber;
            string userInput;
            int num;
            do
            {
                IndentWriteLine(question);
                InputPromptWrite("Enter " + validInput);
                userInput = Console.ReadLine();
                isValidNumber = Int32.TryParse(userInput, out num);
            } while (!isValidNumber
                     || !validator(num));
            return num;
        }

        /// <summary>
        /// Gets a valid integer from the user, based on the given parameters.
        /// </summary>
        /// <param name="validInput">An indication of what valid input is, for the user.</param>
        /// <param name="validator">A method to ensure the input matches the requested validInput.</param>
        /// <param name="range">The range to check the user's input against.</param>
        /// <returns>Returns a validated integer from the user's input.</returns>
        public static int GetIntFromUser(string validInput, Func<int, IntRange, bool> validator, IntRange range)
        {
            Debug.LogMethodStart();

            bool isValidNumber;
            string userInput;
            int num;
            do
            {
                InputPromptWrite("Enter " + validInput);
                userInput = Console.ReadLine();
                isValidNumber = Int32.TryParse(userInput, out num);
            } while (!isValidNumber
                     || !validator(num, range));
            return num;
        }

        /// <summary>
        /// Gets a string from the user, ensuring it is not empty, and does not start with whitespace or control characters.
        /// </summary>
        /// <param name="request">An prompt with the sort of thing they should input.</param>
        /// <returns>Returns a string from the user that does not start with white space or control characters.</returns>
        public static string GetStringFromUser(string request)
        {
            Debug.LogMethodStart();
            string userInput = "";
            char[] test;
            do
            {
                InputPromptWrite(request);
                userInput = Console.ReadLine();
                test = userInput.ToCharArray();
            } while (userInput == String.Empty || char.IsWhiteSpace(test[0]) || char.IsControl(test[0]));
            return userInput;
        }
    }
}
