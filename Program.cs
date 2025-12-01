using System;

namespace Blackjack
{
    public enum Suit { Clubs, Diamonds, Hearts, Spades }
    public enum Rank
    {
        Two = 2, Three, Four, Five, Six,
        Seven, Eight, Nine, Ten,
        Jack = 10, Queen = 10, King = 10, Ace = 11
    }
    class Program
    {
        static void Main()
        {
            var game = new BlackjackGameEng();

            while (true)
            {
                game.Play();

                System.Console.WriteLine("\n Play again? (y/n): ");
                if(Console.ReadLine().ToLower() != "y")
                {
                    break;
                }
                
            }
        }
    }
}