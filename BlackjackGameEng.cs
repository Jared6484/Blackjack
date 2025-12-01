using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Win32.SafeHandles;

namespace Blackjack
{
    public class BlackjackGameEng
    {
        Deck ? deck;
        //public List<Player> players;
        Hand ? player;
        Hand ? dealer;

        public int RunningCount {get; private set;} = 0;
    
        public BlackjackGameEng(){
            deck = new Deck();
            deck.Shuffle();
        }

        public void UpdateCount(Card card)
        {
            int value = (int)card.Rank;

            if(value >= 2 && value <= 6)
            {
                RunningCount +=1;
            }
            else if(value>=10 || card.Rank == Rank.Ace)
            {
                RunningCount -=1;
            }
        }

        public void Play(){

            if(deck.Count < 10)
            {
                System.Console.WriteLine("**** Deck is low -- reshuffling ****");
                deck = new Deck();
                deck.Shuffle();
            }
            player = new Hand();
            dealer = new Hand();

            var c1 = deck.Deal();
            UpdateCount(c1);           
            player.AddCard(c1);
            //player.AddCard(deck.Deal());

            var c2 = deck.Deal();
            UpdateCount(c2);  
            player.AddCard(c2);

            var c3 = deck.Deal();
            UpdateCount(c3);  
            dealer.AddCard(c3);
            //don't put this last card in the count since it's hidden.
            dealer.AddCard(deck.Deal());

            Console.WriteLine($"Dealer shows: {dealer.Cards[0]}");
            Console.WriteLine($"Player shows: {player}");

            System.Console.WriteLine($"\n*** Running Count is: {RunningCount} ***\n");

            while (true)
            {
                Console.Write("Hit or Stand? (h/s)");
                string? choice = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(choice))
                    continue;   

                choice = choice.ToLower();

                if(choice == "h")
                {
                    var cntdCard = deck.Deal();
                    UpdateCount(cntdCard);
                    player.AddCard(cntdCard);
                    Console.WriteLine($"\nYour hand: {player}");
                    
                    Console.WriteLine($"\n*** Running Count is: {RunningCount} ***\n");

                    if(player.GetValue() > 21)
                    {
                        Console.WriteLine("Buuuusted!  Dealer Wins");
                        UpdateCount(dealer.Cards[1]);
                        Console.WriteLine($"\n*** Running Count is: {RunningCount} (Dealer shows bottom card) ***\n");
                        return;
                    }
                }
                else if (choice == "s")
                {
                    break;
                }
            }

            Console.WriteLine($"Dealers' hand: {dealer}");
            UpdateCount(dealer.Cards[1]);
            Console.WriteLine($"\n*** Running Count is: {RunningCount} ***");

            while(dealer.GetValue() < 17)
            {
                var cntdCard = deck.Deal();
                UpdateCount(cntdCard);
                dealer.AddCard(cntdCard);
                Console.WriteLine($"Dealer hits: {dealer}");
                Console.WriteLine($"\n*** Running Count is: {RunningCount} ***\n");
            }
            if(dealer.GetValue()> 21)
            {
                System.Console.WriteLine("Dealer busts.  You WIN!");
                return;
            }

            int p = player.GetValue();
            int d = dealer.GetValue();

            Console.WriteLine($"\nFinal Hands:");
            Console.WriteLine($"Player: {player}");
            Console.WriteLine($"Dealer: {dealer}");

            if (p > d) Console.WriteLine("You win!");
            else if (p < d) Console.WriteLine("Dealer wins.");
            else Console.WriteLine("Push (tie).");            
        }

    }
}
