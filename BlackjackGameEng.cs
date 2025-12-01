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
        public int BankRoll {get; private set;} = 0;
        private int BaseBet = 0;
        private int CurrentBet = 0;
    
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

        public void AskForStartingBankroll()
        {
            while (true)
            {
                System.Console.WriteLine("Enter your Bankroll: ");

                string? input = Console.ReadLine();

                if(int.TryParse(input, out int amount) && amount > 0)
                {
                    BankRoll = amount;
                    //System.Console.WriteLine($"Your bankroll is set to {BankRoll}");
                    return;
                }
                System.Console.WriteLine("Invalid amount.  Try again! ");
            }
        }
        public void AddToBankroll()
        {
            while (true)
            {
                System.Console.WriteLine("Enter the amount you would like to add: ");

                string? input = Console.ReadLine();

                if(int.TryParse(input, out int amount) && amount > 0)
                {
                    BankRoll += amount;
                    System.Console.WriteLine($"Your bankroll is set to {BankRoll}");
                    return;
                }
                System.Console.WriteLine("Invalid amount.  Try again! ");
            }
        }
        public void AskForBetSetting()
        {
            while (true)
            {       
                System.Console.WriteLine($"Your Bankroll is {BankRoll}");
                System.Console.WriteLine("What would you like to set your bet amount to? ");
                System.Console.WriteLine("Must be between $25 and $3000");
                string? input = Console.ReadLine();
                if(int.TryParse(input, out int amount) && amount > BankRoll)
                {
                    System.Console.WriteLine("You can't bet more than your bank");
                    continue;
                }
                else if(int.TryParse(input, out int amount1) && amount1 >= 25 && amount1 <= 3000)
                {
                    BaseBet = amount1;
                    System.Console.WriteLine($"Your current bet is set to {BaseBet}");
                    return;
                }
                System.Console.WriteLine("Invalid amount.  Try again.");
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

            bool firstMove = true;
            while (true)
            {
                CurrentBet = BaseBet;
                Console.Write("Hit or Stand or Double Down (DD on first move only)? (h/s/d)");
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
                        BankRoll -= CurrentBet;
                        Console.WriteLine($"Your Bankroll is {BankRoll}");
                        UpdateCount(dealer.Cards[1]);
                        Console.WriteLine($"\n*** Running Count is: {RunningCount} (Dealer shows bottom card) ***\n");
                        return;
                    }
                }
                else if (choice == "s")
                {
                    break;
                }
                else if (choice == "d" && firstMove)
                {
                    if(BankRoll < CurrentBet * 2)
                    {
                        System.Console.WriteLine("You don't have enough to double down");
                        System.Console.WriteLine("Would you like to add to your bankroll? y/n");
                        if(Console.ReadLine().ToLower() == "y")
                        {
                            AddToBankroll();
                        }
                    }
                    //BankRoll -= BaseBet;
                    CurrentBet = BaseBet*2;

                    Console.WriteLine($"You doubled down! New bet: ${CurrentBet}");
                    System.Console.WriteLine("You get one card");

                    var cntdCard = deck.Deal();
                    UpdateCount(cntdCard);
                    player.AddCard(cntdCard);
                    Console.WriteLine($"\nYour hand: {player}");
                    if(player.GetValue() > 21)
                    {
                        Console.WriteLine("Buuuusted!  Dealer Wins");
                        BankRoll -= CurrentBet;
                        Console.WriteLine($"Your Bankroll is {BankRoll}");
                        UpdateCount(dealer.Cards[1]);
                        Console.WriteLine($"\n*** Running Count is: {RunningCount} (Dealer shows bottom card) ***\n");
                        return;
                    }

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
                BankRoll+= CurrentBet;                        
                Console.WriteLine($"Your Bankroll is {BankRoll}");
                return;
            }

            int p = player.GetValue();
            int d = dealer.GetValue();

            Console.WriteLine($"\nFinal Hands:");
            Console.WriteLine($"Player: {player}");
            Console.WriteLine($"Dealer: {dealer}");

            if (p > d) 
            {
                Console.WriteLine("You win!");
                BankRoll += CurrentBet;
                Console.WriteLine($"Bankroll = {BankRoll}");
            }
            else if (p < d) 
            {
                Console.WriteLine("Dealer wins.");
                BankRoll -= CurrentBet;
                Console.WriteLine($"Bankroll = {BankRoll}");
            }
            else {
                Console.WriteLine("Push (tie).");    
                Console.WriteLine($"Bankroll = {BankRoll}");     
            }   
            CurrentBet = BaseBet;
        }
    }
}
