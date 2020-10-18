using System;
using System.Collections.Generic;

namespace Unfair_Search_Space
{
    class Hangman : GameLoop<string, char, int>
    {
        const int WordSize = 6;
        const char unknownLetter = '?';

        public override List<string> GetMasterList()
        {
            Console.WriteLine("Downloading words...");
            string contents;
            using (var wc = new System.Net.WebClient())
                contents = wc.DownloadString(@"https://raw.githubusercontent.com/first20hours/google-10000-english/master/20k.txt");
            List<string> master = new List<string>();
            foreach (string line in contents.Split('\n'))
                if (line.Length == WordSize)
                    master.Add(line);
            return master;
        }

        List<char> mistakes;
        string guessedWord;

        public override void ResetGame()
        {
            guessedWord = "";
            for (int i = 0; i < WordSize; i++) guessedWord += unknownLetter;
            mistakes = new List<char>();
        }

        public override List<char> GetPossibleGuesses()
        {
            List<char> guesses = new List<char>();
            for (char c = 'a'; c <= 'z'; c++)
                guesses.Add(c);
            return guesses;
        }

        public override char AskForGuess()
        {
            Console.WriteLine();
            Console.WriteLine("" + guessedWord);
            if (mistakes.Count > 0)
                Console.WriteLine("Incorrect letters: " + string.Join(", ", mistakes));
            char letter = '\0';
            while ((mistakes.Contains(letter) || letter < 'a' || letter > 'z'))
            {
                Console.Write("Pick a letter: ");
                letter = Console.ReadKey().KeyChar;
            }
            Console.WriteLine(); Console.WriteLine();
            return letter;
        }

        public override int AskForFeedback(char guess)
        {
            Console.WriteLine(guessedWord);
            Console.WriteLine($"My guess is '{guess}'");
            string nextWord = "";
            int result = 0;
            for (int i = 0; i < WordSize; i++)
            {
                bool foundChar = false;
                if (guessedWord[i] == unknownLetter)
                {
                    Console.WriteLine($"Is it at index {i}? (y/n)");
                    foundChar = Console.ReadKey().KeyChar == 'y';
                }
                if (foundChar)
                {
                    result |= 1 << i;
                    nextWord += guess;
                }
                else
                {
                    nextWord += guessedWord[i];
                }
                Console.WriteLine();
            }
            guessedWord = nextWord;
            return result;
        }

        public override int GetFeedback(string option, char guess)
        {
            int position = 0;
            for (int i = 0; i < WordSize; i++)
                if (option[i] == guess)
                    position |= 1 << i;
            return position;
        }

        public override void GiveFeedback(char letter, int feedback)
        {
            string nextGuess = "";
            for (int i = 0; i < WordSize; i++)
                if ((feedback & (1 << i)) != 0)
                    nextGuess += letter;
                else
                    nextGuess += guessedWord[i];
            guessedWord = nextGuess;
            if (feedback == 0)
            {
                int i = 0;
                while (i < mistakes.Count && mistakes[i] < letter) i++;
                mistakes.Insert(i, letter);
            }
        }

        public override void FinishGame(List<string> options)
        {
            Console.WriteLine("The word was " + options[0]);
            Console.WriteLine("---------------------------");
        }
    }
}
