using System;
using System.Collections.Generic;


namespace Blackjack
{
    public class Deck
    {
        private List<Card> cards = new List<Card>();
        private Random rng = new Random();

        public int Count => cards.Count;

        public Deck()
        {
            foreach (Suit s in Enum.GetValues(typeof(Suit)))
                foreach(Rank r in Enum.GetValues(typeof(Rank)))
                    cards.Add(new Card(s,r));
        }

        public void Shuffle()
        {
            for(int i = cards.Count -1; i>0; i--)
            {
                int j = rng.Next(i+1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }

        public Card Deal()
        {
            if(cards.Count == 0)
            {
                throw new Exception("Deck is Empty");
            }
            Card card = cards[0];
            cards.RemoveAt(0);

            return card;
        }
    }
}
