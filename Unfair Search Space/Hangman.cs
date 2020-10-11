using System;
using System.Collections.Generic;

namespace Unfair_Search_Space
{
    class Hangman : GameLoop<string, char, int>
    {
        const int WordSize = 5;
        const int Attempts = 15;

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
        string guessedWords;

        public override void ResetGame()
        {
            guessedWords = "";
            for (int i = 0; i < WordSize; i++) guessedWords += "?";
            mistakes = new List<char>();
        }

        public override bool GameOver(List<string> options)
        {
            return mistakes.Count == Attempts || !guessedWords.Contains("?");
        }

        public override char AskForGuess()
        {
            Console.WriteLine();
            Console.WriteLine("" + guessedWords);
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

        public override int SortOption(string option, char guess)
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
                    nextGuess += guessedWords[i];
            guessedWords = nextGuess;
            if (feedback == 0)
            {
                int i = 0;
                while (i < mistakes.Count && mistakes[i] < letter) i++;
                mistakes.Insert(i, letter);
            }
        }

        public override void FinishGame(List<string> options)
        {
            Console.WriteLine(mistakes.Count == Attempts ? "You lose" : "You win!");
            Console.WriteLine("The word was " + options[0]);
            Console.WriteLine("---------------------------");
        }
    }
}
