/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using System;
using System.Collections.Generic;
using System.Text;
using Enigma.Utilities;

namespace Enigma.Interaction
{
    /// <summary>
    /// An individual menu item for user interactions
    /// </summary>
    public struct MenuItem
    {
        /// <summary>
        /// The name of the menu item to display.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// A short description of the menu item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A user-selectable menu item with a name and short description.
        /// </summary>
        /// <param name="name">The name of the menu item.</param>
        /// <param name="desc">A short description of the menu item.</param>
        public MenuItem(string name, string desc)
        {
            Name = name;
            Description = desc;
        }
    }

    /// <summary>
    /// A basic numbered menu for user interaction
    /// </summary>
    class Menu : UserInput
    {
        /// <summary>
        /// The items in the menu.
        /// </summary>
        public List<MenuItem> Items { get; }

        /// <summary>
        /// Creates a menu. The last item is always the exit program option.
        /// </summary>
        /// <param name="items"></param>
        public Menu(List<MenuItem> items)
        {
            items.Add(new MenuItem("Shut Down", "Exit the program and write the log file."));
            Items = items;
        }

        /// <summary>
        /// Displays the menu items and screen header.
        /// </summary>
        /// <param name="menuScreen">The menu being displayed.</param>
        /// <param name="shouldGreet">Should the user be greeted?</param>
        public void DisplayItems(string menuScreen, bool shouldGreet)
        {
            Debug.LogMethodStart();
            HeaderWrite(menuScreen);
            IndentWriteLine($"\nCurrent folder: {Environment.CurrentDirectory}");

            if (shouldGreet)
            {
                Greet();
            }

            Console.WriteLine();
            for (int i = 0; i < Items.Count; i++)
            {
                Console.WriteLine(MenuItemFormat($"{i + 1}: {Items[i].Name}", $"{Items[i].Description}"));
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Prompts the user to select a menu item. Return is zero indexed.
        /// </summary>
        /// <returns>Returns the user's selected input minus one (zero indexed)</returns>
        public int ItemSelect()
        {
            Debug.LogMethodStart();
            
            int selection = GetIntFromUser(
                  $"a choice from 1 to {Items.Count}"
                , Validation.IsWithinRange
                , new Validation.IntRange(0, Items.Count + 1));
            // Last menu item is always exit program and user's selection is one-indexed
            if (selection == Items.Count)
            {
                if (AskQuestion("Are you sure you want to exit?", "yes", "no"))
                {
                    Utility.CloseProgram(false);
                }
            }
            // zero index the user's selection after they've made it to avoid programming errors
            return selection - 1;
        }
    }
}
