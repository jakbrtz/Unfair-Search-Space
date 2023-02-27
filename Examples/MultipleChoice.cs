using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unfair_Search_Space;

namespace Examples
{
    class MultipleChoice : GameLoop<string, string, int>
    {
        const int numQuestions = 5;
        const int numOptions = 4;

        protected override int FeedbackThatIsConsideredAWin => numQuestions;

        protected override int AskForFeedback(string guess)
        {
            Console.WriteLine("My guess is: " + guess);

            while (true)
            {
                Console.WriteLine("How many were correct?");
                if (int.TryParse(Console.ReadLine(), out int response) && response >= 0 && response <= numQuestions)
                {
                    return response;
                }
            }
        }

        protected override string AskForGuess()
        {
            while (true)
            {
                Console.WriteLine("Take a guess");
                string guess = Console.ReadLine();
                if (guess.Length == numQuestions && guess.All(c => c >= 'a' && c < 'a' + numOptions))
                {
                    return guess;
                }
            }
        }

        protected override void FinishGame(List<string> options)
        {
            Console.WriteLine("The solution was " + options[0]);
            Console.WriteLine("-------------");
        }

        protected override int GetFeedback(string possibility, string guess)
        {
            int result = 0;
            for (int i = 0; i < numQuestions; i++)
            {
                if (possibility[i] == guess[i])
                {
                    result++;
                }
            }
            return result;
        }

        protected override List<string> GetMasterList()
        {
            List<string> result = new List<string>();
            int combinations = (int)Math.Pow(numOptions, numQuestions);
            for (int i = 0; i < combinations; i++)
            {
                int number = i;
                int[] answers = new int[numQuestions];
                for (int question = 0; question < numQuestions; question++)
                {
                    answers[question] = number % numOptions;
                    number /= numOptions;
                }
                result.Add(new string(answers.Select(n => (char)('a' + n)).ToArray()));
            }
            return result;
        }

        protected override List<string> GetPossibleGuesses()
        {
            return GetMasterList();
        }

        protected override void GiveFeedback(string guess, int feedback)
        {
            Console.WriteLine(feedback + " of your answers were correct");
        }

        protected override void ResetGame()
        {

        }
    }
}
