using System.Collections.Generic;

namespace Unfair_Search_Space
{
    abstract class DomainFilter<Tsolution, Tguess, Tfeedback>
    {
        /// <summary>
        /// Suppose the possibility was the actual solution, what feedback would be given?
        /// </summary>
        /// <param name="possibility">A possible solution</param>
        /// <param name="guess">What did the user just guess?</param>
        public abstract Tfeedback SortOption(Tsolution possibility, Tguess guess);

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
                var key = SortOption(possibility, guess);
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
    }

    abstract class GameLoop<Tsolution, Tguess, Tfeedback> : DomainFilter<Tsolution, Tguess, Tfeedback>
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
        /// Detects if the game has finished
        /// </summary>
        /// <param name="options">List of remaining possible solutions</param>
        public abstract bool GameOver(List<Tsolution> options);
        /// <summary>
        /// Gets a guess from the user
        /// </summary>
        public abstract Tguess AskForGuess();
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
        /// Play the game
        /// </summary>
        public void Play()
        {
            var master = GetMasterList();
            while (true)
            {
                ResetGame();
                var options = master;
                while (!GameOver(options))
                {
                    var guess = AskForGuess();
                    var best = ProcessGuess(guess, ref options);
                    GiveFeedback(guess, best);
                }
                FinishGame(options);
            }
        }
    }
}
