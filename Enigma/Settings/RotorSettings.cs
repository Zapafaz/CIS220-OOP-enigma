/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

namespace Enigma.Settings
{
    /// <summary>
    /// Simple container class that has all the settings needed to create a rotor.
    /// </summary>
    public class RotorSettings
    {
        /// <summary>
        /// The prime number to be used by the rotor.
        /// </summary>
        public int Offset { get; }
        /// <summary>
        /// The rotation the rotor should start at.
        /// </summary>
        public int Rotation { get; }
        /// <summary>
        /// The rotation values the rotor should rotate the next rotor at.
        /// </summary>
        public int[] ToRotateNext { get; }

        /// <summary>
        /// Constructor for the RotorSettings class. Used to initialize a Rotor.
        /// </summary>
        /// <param name="offset">The offset used by the rotor's Caeser cipher.</param>
        /// <param name="rotation">The rotation the rotor should start at.</param>
        /// <param name="toRotateNext">At what rotation(s) should the rotor rotate?</param>
        public RotorSettings(int offset, int rotation, int[] toRotateNext)
        {
            Offset = offset;
            Rotation = rotation;
            ToRotateNext = toRotateNext;
        }
    }
}
