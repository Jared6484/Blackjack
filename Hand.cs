using System;
using System.Collections.Generic;

namespace Blackjack
{
    public class Hand()
    {
        public List<Card> Cards = new List<Card>();

        public void AddCard(Card c) => Cards.Add(c);

        public int GetValue()
        {
            int total = 0;
            int aceCount = 0;

            foreach (var card in Cards)
            {
                total += (int)card.Rank;
                if(card.Rank == Rank.Ace)
                {
                    aceCount++;
                }
            }
            while (total > 21 && aceCount > 0)
            {
                total -=10;
                aceCount--;
            }
            return total;
            
        }
        public override string ToString()
        {
            string result = "";
            foreach (var c in Cards)
            {
                result += c + ", ";
            }
            return result + $"(Value: {GetValue()})";
        }
    }
}
 

/*
    public class Hand
    {
        public List<Card> Cards {get;} = new List<Card>();

        public void AddCard(Card card){
            Cards.Add(card);
        }

    }


    public class Player{
        public Hand PlayerHand {get;} = new Hand(); // auto-property- make private field
        public string Name {get;}

        public Player(string name){
            Name = name;
        }
    }

    public class Dealer{
        public Hand DealerHand {get;} = new Hand();
    }
    */