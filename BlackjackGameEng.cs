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

        private List<Hand> SplitHands = new List<Hand>();
        private List<int> SplitBets = new List<int>();
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
        private Card DealCard()
        {
            Card c = deck.Deal();
            UpdateCount(c);
            return c;
        }

    //  Deal a card to a specific hand
        private void DealToHand(Hand h)
        {
            h.AddCard(DealCard());
        }
        private void HandleSplit(ref Hand originalHand, ref int originalBet)
        {
            if(originalHand.Cards.Count ==2 && 
            originalHand.Cards[0].Rank == originalHand.Cards[1].Rank)
            {
                while (true)
                {              
                    System.Console.WriteLine(" Split ?  (y/n)");
                    string? ans = Console.ReadLine().ToLower();

                    if(ans == "y" && BankRoll < originalBet * 2){
                        System.Console.WriteLine(" You don't have enough to split.");
                        System.Console.WriteLine(" Add more to Bankroll?  (y/n)");
                        string? ans2 = Console.ReadLine().ToLower();

                        if (ans2 == "y")
                        {
                            AddToBankroll();
                        }
                        else if (ans2 == "n") {break;}
                    }
                    else if(ans =="n"){break;}

                    else if(ans == "y")
                    {
                        Hand hand1 = new Hand();
                        Hand hand2 = new Hand();

                        hand1.AddCard(originalHand.Cards[0]);
                        hand2.AddCard(originalHand.Cards[1]);

                        DealToHand(hand1); // added DTH after 1st Major refactor
                        DealToHand(hand2);

                        SplitHands.Clear();
                        SplitHands.Add(hand1);
                        SplitHands.Add(hand2);

                        SplitBets.Clear();
                        SplitBets.Add(originalBet);
                        SplitBets.Add(originalBet);
                        break;
                    }

                    else
                    {
                    System.Console.WriteLine("Invalid Response - Choose y or n");
                    }
                }
            }
            
        }        
        private void PlayerTurn(Hand hand, ref int bet)
        {
            bool firstMove = true;
            while (true)
            {

                Console.Write("Hit or Stand or Double Down (DD on first move only)? (h/s/d)");
                string? choice = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(choice))
                    continue;   

                choice = choice.ToLower();

                if(choice == "h")
                {
                    //firstMove = false;
                    //var cntdCard = deck.Deal();
                    //UpdateCount(cntdCard);
                    //player.AddCard(cntdCard);
                    DealToHand(hand);


                    if(hand.GetValue() > 21)
                    {
                        Console.WriteLine("Buuuusted!  Dealer Wins");
                        //BankRoll -= CurrentBet;
                        Console.WriteLine($"Your Bankroll is {BankRoll}");
                        //UpdateCount(dealer.Cards[1]);
                        //Console.WriteLine($"\n*** Running Count is: {RunningCount} (Dealer shows bottom card) ***\n");
                        return;
                    }
                    Console.WriteLine($"\nYour hand: {hand}");
                    
                    Console.WriteLine($"\n*** Running Count is: {RunningCount} ***\n");
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
                    bet = bet*2; // since bet is a ref, must use 'bet' to update in the List

                    Console.WriteLine($"You doubled down! New bet: ${bet}");
                    System.Console.WriteLine("You get one card");

                    DealToHand(hand);
                    Console.WriteLine($"\nYour hand: {hand}");
                    if(hand.GetValue() > 21)
                    {
                        Console.WriteLine("Buuuusted!  Dealer Wins");
                        //BankRoll -= CurrentBet;
                        //UpdateCount(dealer.Cards[1]);
                        return;
                    }

                    break; //one card for Double Downs
                }
                firstMove = false;
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

            Console.WriteLine($"\n###############################################");
            Console.WriteLine($"\nDealer shows: {dealer.Cards[0]}");
            Console.WriteLine($"Player shows: {player}");
            System.Console.WriteLine($"*** Running Count is: {RunningCount} ***");
            Console.WriteLine($"\n###############################################");

            CurrentBet = BaseBet;
            HandleSplit(ref player, ref CurrentBet);

            if(SplitHands.Count > 0)
            {
                for(int i =0; i < SplitHands.Count; i++)
                {
                    Hand splitHand = SplitHands[i];
                    int splitBet = SplitBets[i];

                    System.Console.WriteLine($"\nPlaying Hand {i+1}: {splitHand}");
                    PlayerTurn(splitHand, ref splitBet);

                    SplitBets[i] = splitBet;

                }
            }
            else
            {
                PlayerTurn(player, ref CurrentBet);
            }

            Console.WriteLine($"Dealers' hand: {dealer}");
            UpdateCount(dealer.Cards[1]);
            Console.WriteLine($"\n*** Running Count is: {RunningCount} (Dealer shows bottom card) ***\n");
            //Console.WriteLine($"\n*** Running Count is: {RunningCount} ***");

            while(dealer.GetValue() < 17)
            {
                //var cntdCard = deck.Deal();
                //UpdateCount(cntdCard);
                //dealer.AddCard(cntdCard);
                DealToHand(dealer);
                Console.WriteLine($"Dealer hits: {dealer}");
                Console.WriteLine($"\n*** Running Count is: {RunningCount} ***\n");
            }
            if(dealer.GetValue()> 21)
            {
                System.Console.WriteLine("Dealer busts.  You WIN!");
                //BankRoll+= CurrentBet;                        
                //Console.WriteLine($"Your Bankroll is {BankRoll}");
                //return;
            }
            if(SplitHands.Count > 0)
            {
                for(int i =0; i< SplitHands.Count; i++)
                {
                    Hand splitHand = SplitHands[i];
                    int bet = SplitBets[i];

                    System.Console.WriteLine($"\n Hand {i+1}: {splitHand}");
                    EvaluateHand(splitHand, bet);

                }

            }
            else
            {
                EvaluateHand(player, CurrentBet);
            }
            Console.WriteLine($"\n$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            Console.WriteLine($"\nFinal Hands:");

            if(SplitHands.Count > 0)
            {
                foreach(var hand in SplitHands)
                {
                    Console.WriteLine($"Player: {hand}");
                }
                SplitHands.Clear();
                SplitBets.Clear();
            }
            else
            {
                Console.WriteLine($"Player: {player}");
            }
            //Console.WriteLine($"Player: {player}");
            Console.WriteLine($"Dealer: {dealer}");

            System.Console.WriteLine($"Current Bankroll = {BankRoll}");
            Console.WriteLine($"\n$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            //EvaluateHand(player, CurrentBet);


        }

        public void EvaluateHand(Hand playerHand, int bet)
        {             
            int p = playerHand.GetValue();
            int d = dealer.GetValue();
            //UpdateCount(dealer.Cards[1]);
            //Console.WriteLine($"\n*** Running Count is: {RunningCount} (Dealer shows bottom card) ***\n");

            if (p > 21)
            {
                System.Console.WriteLine($":(  Busted. You lose: {bet}");
                BankRoll -= bet;
            }

            else if (d >21 || p > d) 
            {
                Console.WriteLine("You win!");
                BankRoll += bet;
                Console.WriteLine($"Bankroll = {BankRoll}");
            }
            else if (p < d) 
            {
                Console.WriteLine("Dealer wins.");
                BankRoll -= bet;
                Console.WriteLine($"Bankroll = {BankRoll}");
            }
            else {
                Console.WriteLine("Push (tie).");    
                Console.WriteLine($"Bankroll = {BankRoll}");     
            }   
        }
    }
        
}


