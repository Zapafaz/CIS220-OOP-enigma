/*
 * Student: Adam Wight
 * Class: CIS220M Object Oriented Programming (Fall 2017)
 * Instructor: Ed Cauthorn
 * Due date: Sunday, December 10th
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Enigma.Interfaces;
using Enigma.Settings;
using System.Xml;
using Enigma.Enums;
using System.Text;
using Enigma.Interaction;

namespace Enigma.Utilities
{
    /// <summary>
    /// File input handling.
    /// </summary>
    public class FileInput : IFileIO
    {
        /// <summary>
        /// The file path that will be used by this instance of FileInput
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Create a new instance of file input, given a file path to read from.
        /// </summary>
        /// <param name="filePath">The path to the text file or xml file to be read.</param>
        public FileInput(string filePath)
        {
            Path = filePath;
        }

        /// <summary>
        /// Gets Enigma settings from an xml file.
        /// </summary>
        /// <returns>Returns Enigma settings from an xml file, or null if the settings are invalid.</returns>
        public EnigmaSettings GetEnigmaSettings()
        {
            Debug.LogMethodStart();

            if (System.IO.Path.GetExtension(Path) != ".xml")
            {
                Error.InvalidSettings(Path);
                return null;
            }

            var rotors = new List<Rotor>();
            RotorSettings rotorSettings;
            bool isDecrypting = false;
            int blockSize = 0;
            string fileIn = "";
            string name = "";
            bool readDecrypting = false;
            bool readBlockSize = false;
            bool readFileIn = false;
            bool readName = false;
            InputType extension = InputType.none;

            try
            {
                using (var reader = XmlReader.Create(Path))
                {
                    reader.MoveToContent();
                    if (reader.LookupNamespace("") == Utility.XML_NAMESPACE)
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                switch (reader.Name)
                                {
                                    case "rotor":
                                        using (var sub = reader.ReadSubtree())
                                        {
                                            rotorSettings = GetRotorSettings(sub);
                                        }
                                        if (rotorSettings != null)
                                        {
                                            rotors.Add(new Rotor(rotorSettings));
                                        }
                                        break;
                                    case "isDecrypting":
                                        readDecrypting = true;
                                        isDecrypting = reader.ReadElementContentAsBoolean();
                                        break;
                                    case "blockSize":
                                        blockSize = reader.ReadElementContentAsInt();
                                        readBlockSize = Validation.IsGreaterThan0(blockSize);
                                        break;
                                    // optional: a .txt or .html file (with extension)
                                    // defaults to keyboard input if not found
                                    case "fileIn": 
                                        fileIn = reader.ReadElementContentAsString();
                                        readFileIn = Validation.HaveIoPermissions(fileIn, false);
                                        extension = Validation.GetInputExtensionType(fileIn);
                                        readFileIn = (extension == InputType.none) ? false : true;
                                        break;
                                    case "name":
                                        readName = true;
                                        name = reader.ReadElementContentAsString();
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error.ShowException(e);
            }
            if (readBlockSize && readDecrypting && readName && rotors.Count > 0)
            {
                if (readFileIn)
                {
                    return new EnigmaSettings(rotors.ToArray(), isDecrypting, blockSize, System.IO.Path.ChangeExtension(fileIn, null), name, extension);
                }
                return new EnigmaSettings(rotors.ToArray(), isDecrypting, blockSize, "", name, InputType.keyboard);
            }
            return null;
        }

        /// <summary>
        /// Gets rotor settings from XML file.
        /// </summary>
        /// <param name="reader">An XmlReader already positioned on a rotor node.</param>
        /// <returns>Returns rotor settings based on the elements in the file, or null if invalid settings.</returns>
        private RotorSettings GetRotorSettings(XmlReader reader)
        {
            Debug.LogMethodStart();

            int rotation = 0;
            int offset = 0;
            int toRotateNext = 0;
            var rotateNexts = new List<int>();
            bool rotSuccess = false;
            bool offSuccess = false;

            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "toRotateNext":
                                toRotateNext = reader.ReadElementContentAsInt();
                                if (Validation.IsValidRotation(toRotateNext))
                                {
                                    rotateNexts.Add(toRotateNext);
                                }
                                break;
                            case "rotation":
                                rotation = reader.ReadElementContentAsInt();
                                rotSuccess = Validation.IsValidRotation(rotation);
                                break;
                            case "offset":
                                offset = reader.ReadElementContentAsInt();
                                offSuccess = Validation.IsValidRotation(offset);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error.ShowException(e);
            }

            if (rotSuccess && offSuccess && rotateNexts.Count > 0)
            {
                return new RotorSettings(offset, rotation, rotateNexts.ToArray());
            }
            return null;
        }

        /// <summary>
        /// Reads in the text from a text file and returns it as a string.
        /// </summary>
        /// <returns>Returns the text from the file.</returns>
        public string Read()
        {
            Debug.LogMethodStart();

            string text = "";
            try
            {
                using (var reader = new StreamReader(Path))
                {
                    text = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Error.ShowException(e);
            }
            return text;
        }
    }
}
