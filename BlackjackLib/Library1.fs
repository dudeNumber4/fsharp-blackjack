namespace Blackjack

// OK, maybe not the best name...
module functions_and_stuff =

  open System
  open BlackjackDataStructures

  // Lunch & Learn:
  // Without the parens, this is simply a let binding and will only get evaluated one time (memoization).
  // Yet, all function examples lack parens.
  let GetNextRandomCard() = 
    let num = randy.Next( deck.Length )
    BlackjackDataStructures.deck.[num]

  let internal AddNextRandomCard( card, deckReference ) =
    // Only add it if it doesn't yet exist.
    if List.exists( fun item -> item = card ) deckReference then deckReference else card :: deckReference

  let internal fullDeck(deck: Card list) =
    deck.Length = BlackjackDataStructures.deck.Length // deck is full when it matches this deck's size

  // 13) Note that pattern matching may include "when" conditions.  In this case, continuing as long as the new
  //     Deck isn't full.
  //     Note clarity of last line.
  let rec internal FillShuffledDeck( deck: Card List ) =
    match deck with
    | list when fullDeck deck -> list
            // See "Function Composition" note
    | _ -> FillShuffledDeck <| AddNextRandomCard(GetNextRandomCard(), deck)
           
  // Card paths like "3Spade.png"
  // 4) Debug experience just as with C#; place cursor inside (each) lambda, hit F9, debug
  let GetPathFor( card ) =
    match card with
    | Ace a -> List.pick( fun (item:string) -> if (item.Contains("Ace") && item.Contains(a.ToString())) then Some(item) else None ) BlackjackDataStructures.cardFiles  // a.ToString() is "Heart"
    | King k -> List.pick( fun (item:string) -> 
                           if (item.Contains("King") && item.Contains(k.ToString())) 
                           then Some(item) 
                           else 
                             None ) BlackjackDataStructures.cardFiles
    | Queen q -> List.pick( fun (item:string) -> if (item.Contains("Queen") && item.Contains(q.ToString())) then Some(item) else None ) BlackjackDataStructures.cardFiles
    | Jack j -> List.pick( fun (item:string) -> if (item.Contains("Jack") && item.Contains(j.ToString())) then Some(item) else None ) BlackjackDataStructures.cardFiles
    | ValueCard(num, x) -> List.pick( fun (item:string) -> if (item.Contains(x.ToString()) && item.Contains(num.ToString())) then Some(item) else None ) BlackjackDataStructures.cardFiles

  // 6)
  // Remove the parens and you get memoization.  Cool, but can be dangerous (I got the same deck back every time until I added the parens).
  let Shuffle() =
    FillShuffledDeck shuffledDeck

  let internal RemoveNextCard( currentDeck: Card List ) =
    let result = currentDeck.Head
    (result, currentDeck.Tail)  // 5) we're returning a new (automatic like the new C# ones) tuple here.

  let Deal( currentDeck ) = 
    let dealerFirstCard, modifiedDeck = RemoveNextCard currentDeck
    let dealerNextCard, modifiedDeck = RemoveNextCard modifiedDeck
    let playerFirstCard, modifiedDeck = RemoveNextCard modifiedDeck
    let playerNextCard, modifiedDeck = RemoveNextCard modifiedDeck
    // 7): Records are preferred over objects.  Note records are very easy to declare.  Returning record indicated by curly braces.
    ///  * Change PlayerHand to some other identifier and check the error (very helpful)
    /// ----
    ///  * Note that the cons operator doesn't work here because these are just 2 objects; 
    ///    in order for cons to work the second item must be a list.
    ///  * Cons operator sample in next function below.
    ///  * Note the other handy list initialization syntax below.
    { DealerHand = [dealerFirstCard ; dealerNextCard]; PlayerHand = [playerFirstCard ; playerNextCard]; Deck = modifiedDeck }

  let Hit( hands, dealer ) =
    let result, modifiedDeck = RemoveNextCard hands.Deck
    let dealerHand =
      if dealer then result :: hands.DealerHand
      else hands.DealerHand
    let playerHand =
      if dealer then hands.PlayerHand
      else result :: hands.PlayerHand
    // Couldn't return this; F# is just too picky about how if then is formatted (pretty sure it has to be on subsequent lines).
    //    { DealerHand = if dealer then result :: hands.DealerHand else hands.DealerHand; PlayerHand = if dealer then result :: hands.DealerHand else hands.DealerHand(*if dealer then hands.PlayerHand else result :: hands.PlayerHand*); Deck = modifiedDeck }
    { DealerHand = dealerHand; PlayerHand = playerHand; Deck = modifiedDeck }

  // 8)
  // Pattern matching function (parameter implied; no need to declare it)
  let NumericValue = function 
    | Ace _ -> 11  // or 1 in func below
    | King _
    | Queen _
    | Jack _ -> 10
    | ValueCard(num, _) -> num

  let internal OverageNumericValue( card ) =
    match card with
    | Ace _ -> 1
    | _ -> NumericValue(card)

  // 9)
  //   - fold: what's in parens in the function, the next 2 are remaining params: starting value and the list to process.
  ///  - The call to sort below sorts the cards in proper order (according to the Card type).  This puts aces where we want them.
  ///  - The last param to fold "(List.sort hand)" is the result of an expression: wrap in parens.
  let NumericValueOfHand( hand ) =
    let simpleValue = List.fold( fun accumulator elem -> accumulator + NumericValue(elem) ) 0 hand
    if simpleValue <= BlackjackDataStructures.BLACKJACK then simpleValue
    else
      List.fold( fun accumulator elem -> let newValue = NumericValue(elem) + accumulator
                                         if newValue < BlackjackDataStructures.BLACKJACK then newValue
                                         else accumulator + OverageNumericValue(elem) ) 0 (List.sort hand)

  /// Simple: dealer stands on soft 17
  let ShouldDealerHit( dealerHand ) =
    NumericValueOfHand( dealerHand ) <= DEALER_HIGHEST_SOFT_HIT

  let Busted( hand ) =
    NumericValueOfHand( hand ) > BLACKJACK

// 10)  Scan back over this listing and see how readable it all is with no scaffolding/type overhead.
//      All the comments are just for demonstration.
//      Functional lends itself to smaller, more distinct functions.
