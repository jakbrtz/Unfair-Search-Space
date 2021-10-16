# Unfair-Search-Space
Cheats at games where the other player needs to guess combinations

In games like [Hangman](https://en.wikipedia.org/wiki/Hangman_(game)) and [Mastermind](https://en.wikipedia.org/wiki/Mastermind_(board_game)), one of the players pick a word/combination and the other player needs to guess what it is. 
This program acts as the picked, but it waits until the very end of the game before it picks a word/combination, and then it acts like that was its plan all along.

The program has [2 abstract classes](Unfair%20Search%20Space/Game.cs) to make it easier to reuse on other games:
* Game - The core class with the bare minimum required to run the algorithm, along with a `ProcessGuess()` that decides what it should tell the user
* GameLoop - a neat class which helps the programmer create a game by telling them to override many small functions

These classes have 3 Type parameters:
* `Tsolution` - What kind of combination is the solution? For example in Hangman, the solution is a word, so `Tsolution` is a `string`
* `Tguess`    - What does the player us to guess? For example in Hangman, the player guesses individual letters, to `Tguess` is a `char`
* `Tfeedback` - How does the program explain the result of a guess? For example in Hangman, the program needs to list the position of letters that are in the word, so `Tfeedback` is a bit vector (which is an `int`)
