using System;
using System.Windows.Forms;

namespace Unfair_Search_Space
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pick a game to play:");
            Console.WriteLine();
            Console.WriteLine("1. Hangman");
            Console.WriteLine("2. Mastermind");

            switch (Console.ReadKey().KeyChar)
            {
                case '1':
                    Console.WriteLine("A simple example of a game that can be made using this code.");
                    Console.WriteLine("To play, enter one letter at a time");
                    Console.ReadKey();
                    new Hangman().Play();
                    break;
                case '2':
                    Console.WriteLine("An example of using this program in a form");
                    Console.WriteLine("Pick a combination by selecting from the dropdown menus");
                    Console.ReadKey();
                    Application.EnableVisualStyles();
                    Application.Run(new Mastermind());
                    break;
            }
        }
    }
}
