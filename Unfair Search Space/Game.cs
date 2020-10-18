using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unfair_Search_Space
{
    abstract class Game<Tsolution, Tguess, Tfeedback>
    {
        /// <summary>
        /// Create a list of every possible guess that the computer could make
        /// </summary>
        public abstract List<Tguess> GetPossibleGuesses();
        /// <summary>
        /// Suppose the possibility was the actual solution, what feedback would be given?
        /// </summary>
        /// <param name="possibility">A possible solution</param>
        /// <param name="guess">What did the user just guess?</param>
        public abstract Tfeedback GetFeedback(Tsolution possibility, Tguess guess);

        /// <summary>
        /// Gives feedback to the user's guess, and updates the list of possible solutions
        /// </summary>
        /// <param name="guess">The guess that the user just made</param>
        /// <param name="domain">The list of possible solutions</param>
        public Tfeedback ProcessGuess(Tguess guess, ref List<Tsolution> domain)
        {
            var possibleOptions = new Dictionary<Tfeedback, List<Tsolution>>();
            foreach (var possibility in domain)
            {
                var key = GetFeedback(possibility, guess);
                if (!possibleOptions.ContainsKey(key))
                    possibleOptions[key] = new List<Tsolution>();
                possibleOptions[key].Add(possibility);
            }
            bool found = false;
            Tfeedback best = default;
            foreach (var kvp in possibleOptions)
            {
                if (!found || kvp.Value.Count > possibleOptions[best].Count)
                {
                    best = kvp.Key;
                    found = true;
                }
            }
            domain = possibleOptions[best];
            return best;
        }
        /// <summary>
        /// Figure out which guess would yield the most useful information
        /// </summary>
        public Tguess MakeGuess(List<Tsolution> options)
        {
            bool found = false;
            Tguess bestGuess = default;
            int bestScore = 0;

            object threadLocker = new object();
            Parallel.ForEach(GetPossibleGuesses(), guess =>
            {
                Dictionary<Tfeedback, int> feedbackCounts = new Dictionary<Tfeedback, int>();
                foreach (var option in options)
                {
                    Tfeedback feedback = GetFeedback(option, guess);
                    feedbackCounts.TryGetValue(feedback, out int count);
                    feedbackCounts[feedback] = count + 1;
                }
                int score = feedbackCounts.Values.Max();
                lock (threadLocker)
                {
                    if (!found || bestScore > score)
                    {
                        bestScore = score;
                        bestGuess = guess;
                        found = true;
                    }
                }
            });

            return bestGuess;
        }
    }

    abstract class GameLoop<Tsolution, Tguess, Tfeedback> : Game<Tsolution, Tguess, Tfeedback>
    {
        /// <summary>
        /// Prepares a master list of all possible solutions
        /// </summary>
        public abstract List<Tsolution> GetMasterList();
        /// <summary>
        /// Prepare variables (except for master list) that need to be set at the start of the game
        /// </summary>
        public abstract void ResetGame();
        /// <summary>
        /// Gets a guess from the user
        /// </summary>
        public abstract Tguess AskForGuess();
        /// <summary>
        /// Ask the user for information based on the computer's guess
        /// </summary>
        public abstract Tfeedback AskForFeedback(Tguess guess);
        /// <summary>
        /// Translates feedback so the user can understand it
        /// </summary>
        /// <param name="guess">The user's recent guess</param>
        /// <param name="feedback">The feedback from the guess</param>
        public abstract void GiveFeedback(Tguess guess, Tfeedback feedback);
        /// <summary>
        /// Action to be performed when the game is over
        /// </summary>
        /// <param name="options">Remaining possible solutions</param>
        public abstract void FinishGame(List<Tsolution> options);

        /// <summary>
        /// Play the game, where the computer picks the combination
        /// </summary>
        public void PlayCheater()
        {
            var master = GetMasterList();
            while (true)
            {
                ResetGame();
                var options = master;
                while (options.Count > 1)
                {
                    var guess = AskForGuess();
                    var best = ProcessGuess(guess, ref options);
                    GiveFeedback(guess, best);
                }
                FinishGame(options);
            }
        }
        /// <summary>
        /// Play the game, where the computer guesses the combination
        /// </summary>
        public void PlayerSolver()
        {
            var master = GetMasterList();
            while (true)
            {
                ResetGame();
                var options = master;
                while (options.Count > 1)
                {
                    Tguess bestGuess = MakeGuess(options);
                    var relevantFeedback = AskForFeedback(bestGuess);
                    List<Tsolution> remaining = new List<Tsolution>();
                    foreach (Tsolution possibility in options)
                        if (GetFeedback(possibility, bestGuess).Equals(relevantFeedback))
                            remaining.Add(possibility);
                    options = remaining;
                }
                FinishGame(options);
            }
        }
    }
}
