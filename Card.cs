using Blackjack;

public class Card
{
    public Rank Rank {get; set;} 
    public Suit Suit {get;set;}   

    public Card(Suit suit, Rank rank){
        Suit = suit;
        Rank = rank;
    }

    public override string ToString(){
        return $"{Rank} of {Suit}";
    }
}
