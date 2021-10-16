using System;
using System.Windows.Forms;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pick a game to play:");
            Console.WriteLine();
            Console.WriteLine("1. Hangman    - computer picks word");
            Console.WriteLine("2. Mastermind - computer picks combination");
            Console.WriteLine("3. Hangman    - computer solves word");
            Console.WriteLine("4. Mastermind - computer solves combination");

            switch (Console.ReadKey().KeyChar)
            {
                case '1':
                    Console.WriteLine("A simple example of a game that can be made using this code.");
                    Console.WriteLine("To play, enter one letter at a time");
                    Console.ReadKey();
                    new Hangman().PlayCheater();
                    break;
                case '2':
                    Console.WriteLine("An example of using this program in a form");
                    Console.WriteLine("Pick a combination by selecting from the dropdown menus");
                    Console.ReadKey();
                    Application.EnableVisualStyles();
                    Application.Run(new MastermindForm());
                    break;
                case '3':
                    Console.WriteLine("An example of a solver that should be able to beat the cheater at hangman");
                    Console.WriteLine("Start by picking a 6-letter word, then answer the questions with (y)es or (n)o");
                    Console.ReadKey();
                    new Hangman().PlayerSolver();
                    break;
                case '4':
                    Console.WriteLine("An example of a solver that should be able to beat the cheater at mastermind");
                    Console.WriteLine("Pick a combination, then answer the questions by typing in digits 0-4");
                    Console.ReadKey();
                    new MastermindConsole().PlayerSolver();
                    break;
            }
        }
    }
}
