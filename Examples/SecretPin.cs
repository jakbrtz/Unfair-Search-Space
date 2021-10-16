using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unfair_Search_Space;

namespace Examples
{
    class SecretPin : GameLoop<int[], int[], SecretPinRule>
    {
        protected override List<int[]> GetMasterList()
        {
            List<int[]> list = new List<int[]>();
            for (int i = 0; i < 10000; i++)
            {
                int[] number = new int[4];
                int k = i;
                for (int j = 0; j < 4; j++)
                {
                    number[j] = k % 10;
                    k /= 10;
                }
                list.Add(number);
            }
            return list;
        }

        List<SecretPinRule> appliedRules;
        List<int[]> guesses;

        protected override void ResetGame()
        {
            appliedRules = new List<SecretPinRule>();
            guesses = new List<int[]>();
        }

        protected override List<int[]> GetPossibleGuesses()
        {
            return GetMasterList();
        }

        protected override int[] AskForGuess()
        {
            Console.WriteLine("Please pick a pin:");
            string pin = Console.ReadLine();
            while (!(pin.Length == 4 && pin.All(c => char.IsDigit(c))))
            {
                Console.WriteLine("A pin must be 4 digits long");
                pin = Console.ReadLine();
            }
            return pin.Select(c => c - '0').ToArray();
        }

        protected override SecretPinRule AskForFeedback(int[] guess)
        {
            throw new NotImplementedException();
        }

        protected override void FinishGame(List<int[]> options)
        {
            Console.WriteLine("Thankyou for picking a pin");
            Console.WriteLine("--------------------------");
        }

        protected override SecretPinRule GetFeedback(int[] possibility, int[] guess)
        {
            foreach (var possibleRule in appliedRules.Concat(Rules).Distinct())
            {
                if (possibleRule.allows(possibility) && !possibleRule.allows(guess))
                {
                    return possibleRule;
                }
            }
            return FeedbackThatIsConsideredAWin;
        }

        protected override void GiveFeedback(int[] guess, SecretPinRule feedback)
        {
            if (feedback != FeedbackThatIsConsideredAWin)
            {
                Console.WriteLine(feedback.description);
                appliedRules.Add(feedback);
                guesses.Add(guess);
            }
            else
            {
                Console.WriteLine("That is allowed");
            }
        }

        protected override SecretPinRule FeedbackThatIsConsideredAWin { get; } = new SecretPinRule("Your number is allowed", guess => true);

        private static readonly SecretPinRule[] Rules = new SecretPinRule[]
        {
            new SecretPinRule ("The sum of the first two digits cannot equal the last digit",
                guess => guess[0] + guess[1] != guess[3]),
            new SecretPinRule ("The product of the middle two digits cannot equal the first digit",
                guess => guess[1] * guess[2] != guess[0] ),
            new SecretPinRule ("You can have a maximum of 3 odd numbers",
                guess => guess.Any(d=>d%2==0)),
            new SecretPinRule ("You can have a maximum of 3 even numbers",
                guess => guess.Any(d=>d%2==1)),
            new SecretPinRule ("You need at least two prime numbers",
                guess => guess.Count(d => d==2||d==3||d==5||d==7)>=2),
            new SecretPinRule ("There cannot be a pair of consecutive digits which are consecutive",
                guess => {
                    for (int i = 0; i < 3; i++)
                        if (guess[i] == guess[i + 1] + 1 || guess[i] + 1 == guess[i + 1])
                            return false;
                    return true;
                }
            ),
            new SecretPinRule ("Duplicates are not allowed",
                guess => {
                    for (int i = 0; i < 4; i++)
                        for (int j = 0; j < i; j++)
                            if (guess[i] == guess[j])
                                return false;
                    return true;
                }
            ),
            new SecretPinRule ("The digits cannot be all different",
                guess => {
                    for (int i = 0; i < 4; i++)
                        for (int j = 0; j < i; j++)
                            if (guess[i] == guess[j])
                                return true;
                    return false;
                }
            ),
            new SecretPinRule ("The first number must be larger than the second number",
                guess => guess[0] > guess[1]),
            new SecretPinRule ("The second number must be smaller than the last number",
                guess => guess[1] < guess[3]),
            new SecretPinRule ("At least one number must be greater than 6",
                guess => guess.All(d => d > 6)),
            new SecretPinRule ("7 is not allowed",
                guess => !guess.Contains(7)),
        };
    }

    class SecretPinRule
    {
        internal string description;
        internal Func<int[], bool> allows;

        internal SecretPinRule(string description, Func<int[], bool> allows)
        {
            this.description = description;
            this.allows = allows;
        }
    }
}
