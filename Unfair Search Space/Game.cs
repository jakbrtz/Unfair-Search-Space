using System.Collections.Generic;

namespace Unfair_Search_Space
{
    abstract class DomainFilter<Tsolution, Tguess, Tfeedback>
    {
        public abstract Tfeedback SortOption(Tsolution possibility, Tguess guess);
        protected Tfeedback BestIndex(Dictionary<Tfeedback, List<Tsolution>> sortedOptions)
        {
            bool found = false;
            Tfeedback best = default;
            foreach (var kvp in sortedOptions)
            {
                if (!found || kvp.Value.Count > sortedOptions[best].Count)
                {
                    best = kvp.Key;
                    found = true;
                }
            }
            return best;
        }

        public Tfeedback ProcessGuess(Tguess guess, ref List<Tsolution> options)
        {
            var possibleOptions = new Dictionary<Tfeedback, List<Tsolution>>();
            foreach (var possibility in options)
            {
                var key = SortOption(possibility, guess);
                if (!possibleOptions.ContainsKey(key))
                    possibleOptions[key] = new List<Tsolution>();
                possibleOptions[key].Add(possibility);
            }
            var best = BestIndex(possibleOptions);
            options = possibleOptions[best];
            return best;
        }
    }

    abstract class GameLoop<Tsolution, Tguess, Tfeedback> : DomainFilter<Tsolution, Tguess, Tfeedback>
    {
        public abstract void ResetGame();
        public abstract List<Tsolution> GetMasterList();
        public abstract bool GameOver(List<Tsolution> options);
        public abstract Tguess AskForGuess();
        public abstract void GiveFeedback(Tguess guess, Tfeedback feedback);
        public abstract void FinishGame(List<Tsolution> options);

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
