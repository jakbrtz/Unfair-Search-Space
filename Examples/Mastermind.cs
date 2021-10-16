using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Unfair_Search_Space;

namespace Examples
{
    public partial class MastermindForm : Form
    {
        Player player = new Player();

        public MastermindForm()
        {
            InitializeComponent();
        }

        List<Color[]> options;

        private void Mastermind_Load(object sender, EventArgs e)
        {
            Random rnd = new Random();
            options = new List<Color[]>();
            int combinations = (int)Math.Pow(allColours.Length, holes);
            for (int i = 0; i < combinations; i++)
            {
                int number = i;
                int[] colourIndecies = new int[holes];
                for (int holeIndex = 0; holeIndex < holes; holeIndex++)
                {
                    colourIndecies[holeIndex] = number % allColours.Length;
                    number /= allColours.Length;
                }
                options.Add(colourIndecies.Select(n => allColours[n]).ToArray());
            }

            NextRow();
        }

        void NextRow()
        {
            MastermindRow row = new MastermindRow();
            row.MakeGuess += Row_MakeGuess;
            flowLayoutPanel1.Controls.Add(row);
            flowLayoutPanel1.ScrollControlIntoView(row);
        }

        private void Row_MakeGuess(object sender, EventArgs e)
        {
            var guess = (sender as MastermindRow).Guess;
            var feedback = player.ProcessGuess(guess, ref options);
            (sender as MastermindRow).SendFeedback(feedback.correctPosition, feedback.incorrectPosition);
            guesses++;
            if (feedback.correctPosition == holes)
            {
                MessageBox.Show($"You win in {guesses} guesses!");
            }
            else if (guesses < attempts)
            {
                NextRow();
            }
            else
            {
                MessageBox.Show("The solution was" + string.Join(", ", options.First()));
            }
        }

        int guesses = 0;
        const int holes = 4;
        const int attempts = 20;
        static readonly Color[] allColours = MastermindRow.colours;

        class Player : Game<Color[], Color[], (int correctPosition, int incorrectPosition)>
        {
            public override List<Color[]> GetPossibleGuesses()
            {
                throw new NotImplementedException();
            }

            public override (int, int) GetFeedback(Color[] possibility, Color[] guess)
            {
                int correctPosition = 0;
                for (int i = 0; i < holes; i++)
                    if (guess[i].Equals(possibility[i]))
                        correctPosition++;
                int incorrectPosition = -correctPosition;
                foreach (var colour in allColours)
                    incorrectPosition += Math.Min(guess.Count(c => c.Equals(colour)), possibility.Count(c => c.Equals(colour)));
                return (correctPosition, incorrectPosition);
            }

            
        }
    }

    class MastermindConsole : GameLoop<string[], string[], (int, int)>
    {
        const int holes = 4;
        static string[] allColours = { "red", "orange", "yellow", "green", "blue", "purple", "pink", "white" };

        public override (int, int) AskForFeedback(string[] guess)
        {
            Console.WriteLine("My guess is " + string.Join(" ", guess));
            Console.Write("How many were correct and in the right position?\t");
            int correctPosition = Console.ReadKey().KeyChar - '0';
            Console.WriteLine();
            Console.Write("How many were correct but in the wrong position?\t");
            int incorrectPosition = Console.ReadKey().KeyChar - '0';
            Console.WriteLine();
            return (correctPosition, incorrectPosition);
        }

        public override string[] AskForGuess()
        {
            throw new NotImplementedException();
        }

        public override void FinishGame(List<string[]> options)
        {
            Console.WriteLine("The solution was " + string.Join(" ", options[0]));
        }

        public override (int, int) GetFeedback(string[] possibility, string[] guess)
        {
            int correctPosition = 0;
            for (int i = 0; i < holes; i++)
                if (guess[i].Equals(possibility[i]))
                    correctPosition++;
            int incorrectPosition = -correctPosition;
            foreach (var colour in allColours)
                incorrectPosition += Math.Min(guess.Count(c => c.Equals(colour)), possibility.Count(c => c.Equals(colour)));
            return (correctPosition, incorrectPosition);
        }

        public override List<string[]> GetMasterList()
        {
            Random rnd = new Random();
            List<string[]> masterlist = new List<string[]>();
            int combinations = (int)Math.Pow(allColours.Length, holes);
            for (int i = 0; i < combinations; i++)
            {
                int number = i;
                int[] colourIndecies = new int[holes];
                for (int holeIndex = 0; holeIndex < holes; holeIndex++)
                {
                    colourIndecies[holeIndex] = number % allColours.Length;
                    number /= allColours.Length;
                }
                bool allUnique = true;
                for (int x = 0; x < holes; x++)
                    for (int y = 0; y < x; y++)
                        if (colourIndecies[x] == colourIndecies[y])
                            allUnique = false;
                if (allUnique)
                    masterlist.Add(colourIndecies.Select(n => allColours[n]).ToArray());
            }
            return masterlist;
        }

        public override List<string[]> GetPossibleGuesses()
        {
            return GetMasterList();
        }

        public override void GiveFeedback(string[] guess, (int, int) feedback)
        {
            Console.WriteLine($"{feedback.Item1} are in the correct position");
            Console.WriteLine($"{feedback.Item2} are correct but in the wrong position");
        }

        public override void ResetGame()
        {
            Console.WriteLine("Thinking...");
        }
    }
}
