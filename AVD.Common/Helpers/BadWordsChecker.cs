using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AVD.Common.Exceptions;
using AVD.Common.Logging;

namespace AVD.Common.Helpers
{
    /// <summary>
    /// Takes an ADX reference number and checks if it is a bad word or a curse word. Uses a list of bad words in english from
    /// https://github.com/shutterstock/List-of-Dirty-Naughty-Obscene-and-Otherwise-Bad-Words
    /// </summary>
    public class BadWordsChecker
    {
        private static volatile BadWordsChecker _checkerInstance;
        private static object syncRoot = new Object();

        private static string file = "enBadWords.txt";

        private List<string> badWords;
        /// <summary>
        /// Loads a list of bad words from disk and caches them. This class is a singleton, so only one instance exist
        /// </summary>
        private BadWordsChecker()
        {
            badWords = new List<string>();
            // check if the list of bad words is in cache
            // if not in cache, load from resource file
           
            var asm = Assembly.GetAssembly(this.GetType());
            var resourceStreamName = this.GetType().Namespace + "." + file;

            var resourceStream = asm.GetManifestResourceStream(resourceStreamName);
            if (resourceStream != null)
            {
                Logger.Instance.Debug(this.GetType().Name, "Constructor", resourceStream.Length);

                StreamReader badWordsFile = new StreamReader(resourceStream);
                try
                {
                    while (badWordsFile.Peek() != -1)
                    {
                        string badword = badWordsFile.ReadLine();
                        if (badword != null)
                        {
                            badword = Regex.Replace(badword, @"[\p{P}\s]", ""); //replace all punctuation and spaces
                            badWords.Add(badword);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(this.GetType().Name, "constructor", ex);
                    throw new TravelEdgeException("bad words txt file couldd not located by the assembly", ex);
                }
            }
            // add to cache - No need as the list is not a private member of this class
        }

        /// <summary>
        /// Concurrency safe implementation of signleton instantiation for the BadWordsChecker.
        /// </summary>
        public static BadWordsChecker Instance
        {
            get
            {
                if (_checkerInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (_checkerInstance == null)
                        {
                            _checkerInstance = new BadWordsChecker();
                        }
                    }
                }
                return _checkerInstance;
            }
        }

        /// <summary>
        /// Takes in a word and does a substring match to see if parts of this word match any of the curse words
        /// It does so by replacing the numbers with characters if the numbers can represent any characters, otherwise it strips the 
        /// numbers out, and then removing any punctuation and then comparing with our list of bad words
        /// </summary>
        /// <param name="word">The word to be checked</param>
        /// <returns>true if the word supplied has a bad word in it, false otherwise</returns>
        public bool IsBadWord(string word)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "isBadWord");
            Logger.Instance.Debug(this.GetType().Name, "isBadWord", "Word being checked: " + word);
            // replace numbers with some letters in the input word
            // 4 = A, 0 = O, 1 = I for now
            string cmpWord = word.Replace("1", "I").Replace("4","A").Replace("0","O").ToLower();

             // compare with our list
            foreach (string item in badWords) {
                if(Regex.IsMatch(cmpWord, @"("+item+")") == true) {
                    Logger.Instance.Debug(this.GetType().Name, "isBadWord", word + " is NOT OK! It matched " + item);
                    Logger.Instance.LogFunctionExit(this.GetType().Name, "isBadWord");
                    return true; // it did match
                }
            }
            Logger.Instance.Debug(this.GetType().Name, "isBadWord", word + " is Ok!");
            // return as supplied word did not match our english list
            Logger.Instance.LogFunctionExit(this.GetType().Name, "isBadWord");
            return false;
        }
    }
}
