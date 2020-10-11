using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Unfair_Search_Space
{
    public partial class Mastermind : Form
    {
        Player player = new Player();

        public Mastermind()
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

        class Player : DomainFilter<Color[], Color[], Player.Feedback>
        {
            public override Feedback SortOption(Color[] possibility, Color[] guess)
            {
                return new Feedback(possibility, guess);
            }

            public struct Feedback
            {
                public int correctPosition;
                public int incorrectPosition;

                public Feedback(Color[] actual, Color[] guess)
                {
                    correctPosition = 0;
                    for (int i = 0; i < holes; i++)
                        if (guess[i] == actual[i])
                            correctPosition++;
                    incorrectPosition = -correctPosition;
                    foreach (var colour in allColours)
                        incorrectPosition += Math.Min(guess.Count(c => c == colour), actual.Count(c => c == colour));
                }
            }
        }
    }
}
