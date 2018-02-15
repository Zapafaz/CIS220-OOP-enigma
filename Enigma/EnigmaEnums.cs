/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

namespace Enigma.Enums
{
    /// <summary>
    /// Types of enigma machine input.
    /// </summary>
    public enum InputType
    {
        /// <summary>
        /// No input type - default value, should never be used.
        /// </summary>
        none = 0,
        /// <summary>
        /// Input is .txt file.
        /// </summary>
        txt = 1,
        /// <summary>
        /// Input is .html file.
        /// </summary>
        html = 2,
        /// <summary>
        /// Input is from the keyboard (user input).
        /// </summary>
        keyboard = 3
    }
}
