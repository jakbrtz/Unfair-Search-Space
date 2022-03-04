using System;
using System.Collections.Generic;
using System.Linq;
using Unfair_Search_Space;

namespace Examples
{
    class Wordle : GameLoop<string, string, WordleFeedback>
    {
        List<string> masterList;
        internal int WordSize = 5;
        const char unknownLetter = '?';

        protected override List<string> GetMasterList()
        {
            Console.WriteLine("Downloading words...");
            string contents;
            using (var wc = new System.Net.WebClient())
                contents = wc.DownloadString(@"https://raw.githubusercontent.com/tabatkins/wordle-list/main/words");
            List<string> master = new List<string>();
            foreach (string line in contents.Split('\n'))
                master.Add(line);
            masterList = master.ToList();
            return master;
        }

        int[] minLetters = new int[26];
        int[] maxLetters = new int[26];
        string knownLetters = "";

        protected override WordleFeedback FeedbackThatIsConsideredAWin
        {
            get
            {
                WordleFeedback result = new WordleFeedback(WordSize);
                for (int i = 0; i < WordSize; i++)
                {
                    result[i] = WordleFeedback.Correct;
                }
                return result;
            }
        }

        protected override void ResetGame()
        {
            for (int i = 0; i < 26; i++)
            {
                minLetters[i] = 0;
                maxLetters[i] = WordSize;
            }
            knownLetters = "";
            for (int i = 0; i < WordSize; i++)
            {
                knownLetters += unknownLetter;
            }
        }

        protected override List<string> GetPossibleGuesses()
        {
            return masterList;
        }

        protected override string AskForGuess()
        {
            Console.WriteLine();
            string input;
            do
            {
                Console.Write("Pick a 5-letter word: ");
                input = Console.ReadLine();
            }
            while (input.Length != 5 || !input.All(char.IsLetter));
            Console.WriteLine();
            return input.ToLower();
        }

        protected override WordleFeedback AskForFeedback(string guess)
        {
            Console.WriteLine($"My guess is '{guess}'");
            WordleFeedback result = new WordleFeedback(WordSize);
            for (int i = 0; i < WordSize; i++)
            {
                if (knownLetters[i] == guess[i])
                {
                    result[i] = WordleFeedback.Correct;
                }
                else
                {
                    Console.WriteLine($"Feedback for index {i}? (y = correct, k = wrong place, n = wrong)");
                    switch (Console.ReadKey().KeyChar)
                    {
                        case 'y': result[i] = WordleFeedback.Correct;     break;
                        case 'k': result[i] = WordleFeedback.WrongPlace;  break;
                        default:  result[i] = WordleFeedback.WrongLetter; break;
                    }
                }
                Console.WriteLine();
            }
            HandleFeedback(guess, result); // todo: refactor so this call doesn't need to be made
            return result;
        }

        protected override WordleFeedback GetFeedback(string option, string guess)
        {
            WordleFeedback result = new WordleFeedback(WordSize);
            for (int i = 0; i < WordSize; i++)
            {
                if (guess[i] == option[i])
                {
                    result[i] = WordleFeedback.Correct;
                }
                else
                {
                    int countLetterInOption = 0;
                    for (int j = 0; j < WordSize; j++)
                    {
                        if (option[j] == guess[i] && option[j] != guess[j])
                        {
                            countLetterInOption++;
                        }
                    }
                    int countLetterInGuess = 1;
                    for (int j = 0; j < i; j++)
                    {
                        if (guess[j] == guess[i] && guess[j] != option[j])
                        {
                            countLetterInGuess++;
                        }
                    }
                    if (countLetterInGuess <= countLetterInOption)
                    {
                        result[i] = WordleFeedback.WrongPlace;
                    }
                    else
                    {
                        result[i] = WordleFeedback.WrongLetter;
                    }
                }
            }
            return result;
        }

        void HandleFeedback(string guess, WordleFeedback feedback)
        {
            int[] countLetters = new int[26];
            int[] countWrongLetters = new int[26];
            for (int i = 0; i < WordSize; i++)
            {
                if (feedback[i] == WordleFeedback.Correct)
                {
                    knownLetters = knownLetters.Remove(i) + guess[i] + (i < WordSize - 1 ? knownLetters.Substring(i + 1) : "");
                }
                if (feedback[i] == WordleFeedback.WrongLetter)
                {
                    countWrongLetters[guess[i] - 'a']++;
                }
                else
                {
                    countLetters[guess[i] - 'a']++;
                }
            }
            for (int i = 0; i < 26; i++)
            {
                if (minLetters[i] < countLetters[i])
                {
                    minLetters[i] = countLetters[i];
                }
                if (countWrongLetters[i] > 0 && maxLetters[i] > countLetters[i])
                {
                    maxLetters[i] = countLetters[i];
                }
            }
        }

        protected override void GiveFeedback(string guess, WordleFeedback feedback)
        {
            HandleFeedback(guess, feedback);
            Console.Write("                                                  ");
            for (int i = 0; i < WordSize; i++)
            {
                switch(feedback[i])
                {
                    case WordleFeedback.Correct: Console.BackgroundColor = ConsoleColor.DarkGreen; break;
                    case WordleFeedback.WrongPlace: Console.BackgroundColor = ConsoleColor.DarkYellow; break;
                    default: Console.BackgroundColor = ConsoleColor.Black; break;
                }
                Console.Write(guess[i]);
            }
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Black;
        }

        protected override void FinishGame(List<string> options)
        {
            Console.WriteLine("The word was " + options[0]);
            Console.WriteLine("---------------------------");
        }


    }

    struct WordleFeedback
    {
        internal enum CellFeedback { Correct, WrongPlace, WrongLetter }

        internal CellFeedback[] result;

        internal WordleFeedback(int wordSize)
        {
            result = new CellFeedback[wordSize];
        }

        internal CellFeedback this[int index]
        {
            get => result[index];
            set => result[index] = value;
        }

        internal const CellFeedback Correct = CellFeedback.Correct;
        internal const CellFeedback WrongPlace = CellFeedback.WrongPlace;
        internal const CellFeedback WrongLetter = CellFeedback.WrongLetter;

        public override bool Equals(object obj)
        {
            if (!(obj is WordleFeedback other)) return false;
            for (int i = 0; i < result.Length; i++)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < result.Length; i++)
            {
                hash += (int)result[i];
                hash <<= 2;
            }
            return hash;
        }

        public override string ToString()
        {
            return string.Join(" ", result);
        }
    }
}
