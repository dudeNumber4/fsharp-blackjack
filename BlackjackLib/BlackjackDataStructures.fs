namespace Blackjack

// 1)  Note how clear, concise & easy to understand this structure representing the deck is.
//     Non-programmer would easily see what's going on.  Would think programming is really easy.
// Especially, see the creation of the deck & the enumeration of the files in a folder.
//type Suit = Heart | Spade | Diamond | Club // <-- could be like this except shuffling relies on values.
type Suit = 
| Heart = 0
| Spade = 1
| Diamond = 2
| Club = 3

type Card =
| ValueCard of int * Suit
| Jack of Suit
| Queen of Suit
| King of Suit
| Ace of Suit

type Hand = Card List
type Hands = { DealerHand: Hand; PlayerHand: Hand; Deck: Card List }

// This module must appear before it's consumer in the list of projects or you will be very confused.
//http://www.devx.com/dotnet/Article/40537/0/page/3  -- cards
module BlackjackDataStructures =

  open System

  let deck =
    [ for suit in [ Suit.Heart; Suit.Diamond; Suit.Spade; Suit.Club ] do
        yield Ace(suit)
        yield King(suit)
        yield Queen(suit)
        yield Jack(suit)
        for v in 2 .. 10 do
          yield ValueCard(v, suit)
    ]

  let shuffledDeck = []

  let randy = new Random()

  let cardFiles = System.IO.Directory.EnumerateFiles( "C:\Data\SideProjects\fsharp-blackjack\Cards" ) |> Seq.toList

  let BLACKJACK = 21
  let DEALER_HIGHEST_SOFT_HIT = 16
