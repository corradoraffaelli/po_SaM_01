using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoLines
{
    class Entry
    {
        /// <summary>
        /// The Prefix with source location, id, etc.
        /// </summary>
        public string[] Prefix;

        /// <summary>
        /// The Key of the entry
        /// </summary>
        public string Key;

        /// <summary>
        /// The msgctxt value of the entry
        /// </summary>
        public string Msgctxt;

        /// <summary>
        /// The original English string
        /// </summary>
        public string OriginalString;
        
        /// <summary>
        /// The string translated to the target language
        /// </summary>
        public string TranslatedString;

        public bool bIsTranslationEmpty = false;
    }

    class ParsedPo
    {
        /// <summary>
        /// The Prefix with copyright, creation date, target language, encoding standard, etc.
        /// </summary>
        public string[] Intro;

        /// <summary>
        /// The entries of the .po
        /// </summary>
        public List<Entry> Entries;

        /// <summary>
        /// Read from the lines parsed from an original .po file an fill everything
        /// </summary>
        /// <param name="lines"></param>
        public void ReadFromLines(string[] lines)
        {
            // The first time that we have an empty translation, it's part of the header, not to be considered
            bool bIsHeader = true;

            // A list of string that is used until an empty string is found
            List<string> TempEntry = new List<string>();

            // The original english string
            string TempOriginalString = "";

            // The translated string
            string TempTranslatedString = "";

            // The key
            string TempKey = "";

            // The value msgctxt
            string TempMsgctxt = "";

            foreach (string line in lines)
            {
                if (line.Contains("msgid") && !bIsHeader)
                {
                    TempOriginalString = line.Substring(7, line.Length - 8);
                }
                else if (line.Contains("msgstr") && !bIsHeader)
                {
                    TempTranslatedString = line.Substring(8, line.Length - 9);
                }
                else if (line.Contains("#. Key") && !bIsHeader)
                {
                    TempKey = line.Substring(8, line.Length - 8);
                }
                else if (line.Contains("msgctxt") && !bIsHeader)
                {
                    TempMsgctxt = line.Substring(9, line.Length - 10);
                }
                else if (String.IsNullOrEmpty(line))
                {
                    // If it's the header, save all the temp entry to the intro array
                    if (bIsHeader)
                    {
                        Intro = TempEntry.ToArray();
                        TempEntry.Clear();
                        bIsHeader = false;
                    }
                    // if it's not the header, create a new Entry and fill its values
                    else
                    {
                        Entry FoundEntry = new Entry();

                        FoundEntry.Prefix = TempEntry.ToArray();
                        TempEntry.Clear();

                        FoundEntry.OriginalString = TempOriginalString;

                        FoundEntry.TranslatedString = TempTranslatedString;

                        FoundEntry.Key = TempKey;

                        FoundEntry.Msgctxt = TempMsgctxt;

                        if (TempTranslatedString == "")
                            FoundEntry.bIsTranslationEmpty = true;

                        if (Entries == null)
                            Entries = new List<Entry>();

                        Entries.Add(FoundEntry);
                    }
                }
                // Line is not empty and it's not the original or the translated message
                else
                {
                    TempEntry.Add(line);
                }
            }
        }

        /// <summary>
        /// Convert the Intro and the entries to an array of lines, ready to be written to file
        /// </summary>
        /// <returns></returns>
        public string[] WritePoToLines()
        {
            List<string> OutputStringList = new List<string>();

            for (int i = 0; i < Intro.Length; i++)
            {
                OutputStringList.Add(Intro[i]);
            }

            OutputStringList.Add(String.Empty);

            if (Entries != null)
            {
                for (int EntryIndex = 0; EntryIndex < Entries.Count; EntryIndex++)
                {
                    string[] EntryArrayLines = WriteEntryToLines(Entries[EntryIndex]);

                    for (int ArrayLineIndex = 0; ArrayLineIndex < EntryArrayLines.Length; ArrayLineIndex++)
                        OutputStringList.Add(EntryArrayLines[ArrayLineIndex]);

                    OutputStringList.Add(String.Empty);
                }
            }
            
            return OutputStringList.ToArray();
        }

        /// <summary>
        /// Convert the entry to an array of lines
        /// </summary>
        /// <returns></returns>
        public string[] WriteEntryToLines(Entry InputEntry)
        {
            List<string> OutputStringList = new List<string>();

            string KeyString = "#. Key: " + InputEntry.Key;
            
            OutputStringList.Add(KeyString);

            for (int i = 0; i < InputEntry.Prefix.Length; i++)
            {
                OutputStringList.Add(InputEntry.Prefix[i]);
            }

            string MsgctxtString = "msgctxt \"" + InputEntry.Msgctxt + "\"";

            string OriginalStringString = "msgid \"" + InputEntry.OriginalString + "\"";

            string TranslatedStringString = "msgstr \"" + InputEntry.TranslatedString + "\"";

            OutputStringList.Add(MsgctxtString);

            OutputStringList.Add(OriginalStringString);

            OutputStringList.Add(TranslatedStringString);

            return OutputStringList.ToArray();
        }

        public Entry GetEntryWithKey(string InputKey)
        {
            if (Entries != null)
            {
                for (int EntryIndex = 0; EntryIndex < Entries.Count; EntryIndex++)
                {
                    if (Entries[EntryIndex].Key == InputKey)
                        return Entries[EntryIndex];
                }
            }
            return new Entry();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool goodChoice = false;
            int maxChoice = 5;

            while (!goodChoice && maxChoice > 0)
            {
                Console.WriteLine("E - Extract empty line from po.");
                Console.WriteLine("M - Add to the first po the lines of second po.");

                string choice = Console.ReadLine();

                if (choice.Equals("E", StringComparison.OrdinalIgnoreCase))
                {
                    goodChoice = true;
                    Console.WriteLine("");
                    Extract();
                }
                else if (choice.Equals("M", StringComparison.OrdinalIgnoreCase))
                {
                    goodChoice = true;
                    Console.WriteLine("");
                    Merge();
                }
                else
                {
                    Console.WriteLine("Command not recognized.");
                    maxChoice--;
                }
            }
           

            //Console.WriteLine (path);

            /*
            string text = System.IO.File.ReadAllText(@"C:\TestFile.txt");
            
            System.Console.WriteLine("Contents of WriteText.txt = {0}", text);




            string[] lines = System.IO.File.ReadAllLines(@"C:\TestFile02.txt");
            
            System.Console.WriteLine("Contents of WriteLines2.txt = ");
            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                Console.WriteLine("\t" + line);
            }

            */
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }

        static void Extract()
        {
            Console.WriteLine("Write path or drag .po file with empty lines to extract.");

            string path = Console.ReadLine();
            
            string[] lines = System.IO.File.ReadAllLines(path);

            ParsedPo PoToExtract = new ParsedPo();

            PoToExtract.ReadFromLines(lines);


            // Create a new parsed .po that will contain only empty entries
            ParsedPo PoWithOnlyEmptyEntries = new ParsedPo();

            PoWithOnlyEmptyEntries.Intro = PoToExtract.Intro;

            PoWithOnlyEmptyEntries.Entries = new List<Entry>();

            for (int EntryIndex = 0; EntryIndex < PoToExtract.Entries.Count; EntryIndex++)
            {
                if (PoToExtract.Entries[EntryIndex].bIsTranslationEmpty)
                {
                    Entry newEntry = new Entry();

                    newEntry = PoToExtract.Entries[EntryIndex];

                    PoWithOnlyEmptyEntries.Entries.Add(newEntry);
                }
            }

            string pathSave = path.Remove(path.Length - 3, 3);
            pathSave = pathSave + "_extracted.po";

            System.IO.File.WriteAllLines(pathSave, PoWithOnlyEmptyEntries.WritePoToLines());
        }

        static void Merge()
        {
            // Read original .po
            Console.WriteLine("Write path or drag the original full .po file with empty lines.");

            string OriginalPath = Console.ReadLine();

            string[] OriginalLines = System.IO.File.ReadAllLines(OriginalPath);

            ParsedPo OriginalPo = new ParsedPo();

            OriginalPo.ReadFromLines(OriginalLines);

            // Read translated .po
            Console.WriteLine("Write path or drag the extracted .po file with empty lines filled by translators.");

            string ExtractedPath = Console.ReadLine();

            string[] ExtractedLines = System.IO.File.ReadAllLines(ExtractedPath);

            ParsedPo ExtractedPo = new ParsedPo();

            ExtractedPo.ReadFromLines(ExtractedLines);

            // cycle all original .po entries
            if (OriginalPo.Entries != null)
            {
                for (int OrigPoEntryIndex = 0; OrigPoEntryIndex < OriginalPo.Entries.Count; OrigPoEntryIndex++)
                {
                    if (OriginalPo.Entries[OrigPoEntryIndex].bIsTranslationEmpty)
                    {
                        Entry FoundEntry = ExtractedPo.GetEntryWithKey(OriginalPo.Entries[OrigPoEntryIndex].Key);

                        if (FoundEntry.OriginalString == OriginalPo.Entries[OrigPoEntryIndex].OriginalString)
                        {
                            OriginalPo.Entries[OrigPoEntryIndex].TranslatedString = FoundEntry.TranslatedString;
                        }
                    }
                }
            }

            string pathSave = OriginalPath.Remove(OriginalPath.Length - 3, 3);
            pathSave = pathSave + "_merged.po";

            System.IO.File.WriteAllLines(pathSave, OriginalPo.WritePoToLines());


            /*
            Console.WriteLine("Merge.");

            // These examples assume a "C:\Users\Public\TestFolder" folder on your machine.
            // You can modify the path if necessary.


            // Example #1: Write an array of strings to a file.
            // Create a string array that consists of three lines.
            string[] lines = { "First line", "Second line", "Third line" };
            // WriteAllLines creates a file, writes a collection of strings to the file,
            // and then closes the file.  You do NOT need to call Flush() or Close().
            System.IO.File.WriteAllLines(@"C:\Users\Public\TestFolder\WriteLines.txt", lines);


            // Example #2: Write one string to a text file.
            string text = "A class is the most powerful data type in C#. Like a structure, " +
                           "a class defines the data and behavior of the data type. ";
            // WriteAllText creates a file, writes the specified string to the file,
            // and then closes the file.    You do NOT need to call Flush() or Close().
            System.IO.File.WriteAllText(@"C:\Users\Public\TestFolder\WriteText.txt", text);

            // Example #3: Write only some strings in an array to a file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            // NOTE: do not use FileStream for text files because it writes bytes, but StreamWriter
            // encodes the output as text.
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\Users\Public\TestFolder\WriteLines2.txt"))
            {
                foreach (string line in lines)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    if (!line.Contains("Second"))
                    {
                        file.WriteLine(line);
                    }
                }
            }

            // Example #4: Append new text to an existing file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\Users\Public\TestFolder\WriteLines2.txt", true))
            {
                file.WriteLine("Fourth line");
            }
            */
        }


        static void SaveToFile(string[] lines, string path)
        {

        }
    }
}
