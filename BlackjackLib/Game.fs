namespace Blackjack

module Game =
  open BlackjackDataStructures
  open functions_and_stuff

  // 2) Look at states and DU (Discriminated Union) type below.
  // States (record types - records are preferred over classes)
  type NewGameState = { Deck: Card List }
  type BetPlacedState = { Deck: Card List; Bet: decimal }
  type InitialCardsDealtState = { Deck: Card List; Bet: decimal; Hands: Hands }
  // Not implementing split for now
  // These 2 states have all the same data; just need to distinguish them as different states.
  type DealingAdditionalCardsToPlayerState = { Deck: Card List; Bet: decimal; Hands: Hands; DoubledDown: bool }
  type DealingAdditionalCardsToDealerState = { Deck: Card List; Bet: decimal; Hands: Hands; DoubledDown: bool }
  type GameOverState = { Deck: Card List; Bet: decimal; Hands: Hands; DoubledDown: bool; Winnings: decimal; Push: bool }

  /// Tie the states together as one DU type.  We use this so we can have a single type referenced around in functions.
  /// DU is roughly analagous to an entire OO hierarchy.  Imagine how much code there would be in that case.
  type BlackJackGame =
  | NewGame of NewGameState
  | BetPlaced of BetPlacedState
  | InitialCardsDealt of InitialCardsDealtState
  | DealingAdditionalCardsToPlayer of DealingAdditionalCardsToPlayerState
  | DealingAdditionalCardsToDealer of DealingAdditionalCardsToDealerState
  | GameOver of GameOverState

  // 3)
  ///////////////////// functions (notice that everything just takes a game:BlackJackGame - mouse over functions to see signature)

  // -> New State
  let StartNewGame() =
    BlackJackGame.NewGame { Deck = functions_and_stuff.Shuffle() }

  // NewGame -> BetPlaced
  // 11) Notice the easy way to throw exception.  No flow control if/else code, just simple branching that catches every possibilty.
  // Reminds me of VB "select case true" but better.
  let PlaceBet (game, bet) =
    match game with
    | NewGame(ng) -> BlackJackGame.BetPlaced { Deck = ng.Deck; Bet = bet }
    | _ -> failwith "You can only place a bet when the game is in NewGameState"

  // BetPlaced -> InitialCardsDealt
  let DealInitialCards (game) =
    match game with
    | BetPlaced(bps) -> BlackJackGame.InitialCardsDealt { Deck = bps.Deck; Bet = bps.Bet; Hands = functions_and_stuff.Deal(bps.Deck) }
    | _ -> failwith "You can only deal initial cards when the game is in BetPlacedState"

  // InitialCardsDealt -> DealingAdditionalCardsToPlayer
  let PlayerInitialDecision ( game, doubleDown ) =
    match game with
    | InitialCardsDealt(icd) -> BlackJackGame.DealingAdditionalCardsToPlayer { Deck = icd.Deck; Bet = icd.Bet; Hands = icd.Hands; DoubledDown = doubleDown }
    | _ -> failwith "You can make double-down decision when the game is in InitialCardsDealtState"

  // Decision for DealingAdditionalCardsToPlayer
  let CanHit ( game ) =
    match game with
    | DealingAdditionalCardsToPlayer(dactp) ->
      let playerHand = dactp.Hands.PlayerHand
      if dactp.DoubledDown then (playerHand.Length < 3) else not (functions_and_stuff.Busted playerHand)
    | _ -> false

  let internal GetWinningsForBlackjack( playerHand, bet ) =
    if functions_and_stuff.NumericValueOfHand( playerHand ) = BLACKJACK then
      bet * 1.5m
    else
      0m

  // Lunch & Learn: Functions must be declared above where they are called.
  let internal RegularWinnings( bet, doubledDown ) =
    if doubledDown then
      bet * 2m
    else
      bet

  let internal HandsAreEqual( playerHand, dealerHand ) =
    functions_and_stuff.NumericValueOfHand(playerHand) = functions_and_stuff.NumericValueOfHand(dealerHand)

  // Lunch & Learn:
  /// You can add access modifiers directly to functions.
  /// We don't want clients calling this because it assumes player has won.  If you call it without a winning player; it'll give you incorrect results.
  let internal Winnings( game ) =
    match game with
    | InitialCardsDealt(icd) ->
      GetWinningsForBlackjack( icd.Hands.PlayerHand, icd.Bet )
    | DealingAdditionalCardsToPlayer(dactp) ->
      RegularWinnings( dactp.Bet, dactp.DoubledDown )
    | DealingAdditionalCardsToDealer(dactd) ->
      RegularWinnings( dactd.Bet, dactd.DoubledDown )
    | GameOver(go) ->
      RegularWinnings( go.Bet, go.DoubledDown )
    | _ -> 0m

  let internal CheckForWinningDealer( game ) =
    match game with
    | DealingAdditionalCardsToDealer(dactd) ->
      if functions_and_stuff.Busted(dactd.Hands.DealerHand) then  // dealer bust
        BlackJackGame.GameOver { Deck = dactd.Deck; Bet = dactd.Bet; Hands = dactd.Hands; DoubledDown = dactd.DoubledDown; Winnings = Winnings( game ); Push = false }
      elif functions_and_stuff.ShouldDealerHit( dactd.Hands.DealerHand ) then  // continue
        BlackJackGame.DealingAdditionalCardsToDealer { Deck = dactd.Deck; Bet = dactd.Bet; Hands = dactd.Hands; DoubledDown = dactd.DoubledDown }
      else  // push or someone wins
        if HandsAreEqual( dactd.Hands.PlayerHand, dactd.Hands.DealerHand ) then  // push
          BlackJackGame.GameOver { Deck = dactd.Deck; Bet = dactd.Bet; Hands = dactd.Hands; DoubledDown = dactd.DoubledDown; Winnings = 0m; Push = true }
        elif functions_and_stuff.NumericValueOfHand(dactd.Hands.PlayerHand) > functions_and_stuff.NumericValueOfHand(dactd.Hands.DealerHand) then // player wins
          BlackJackGame.GameOver { Deck = dactd.Deck; Bet = dactd.Bet; Hands = dactd.Hands; DoubledDown = dactd.DoubledDown; Winnings = Winnings(game); Push = false }
        else  // house wins
          BlackJackGame.GameOver { Deck = dactd.Deck; Bet = dactd.Bet; Hands = dactd.Hands; DoubledDown = dactd.DoubledDown; Winnings = -dactd.Bet; Push = false }
    | _ -> failwith "Expected DealingAdditionalCardsToDealer State in CheckForWinningDealer"

  let internal CheckForWinningPlayer( game ) =
    match game with
    | DealingAdditionalCardsToPlayer(dactp) ->
      if functions_and_stuff.Busted( dactp.Hands.PlayerHand ) then
        BlackJackGame.GameOver { Deck = dactp.Deck; Bet = dactp.Bet; Hands = dactp.Hands; DoubledDown = dactp.DoubledDown; Winnings = -Winnings( game ); Push = false }
      elif functions_and_stuff.NumericValueOfHand( dactp.Hands.PlayerHand ) = BLACKJACK then
        BlackJackGame.GameOver { Deck = dactp.Deck; Bet = dactp.Bet; Hands = dactp.Hands; DoubledDown = dactp.DoubledDown; Winnings = Winnings( game ); Push = false }
      else 
        game  // not yet a winner
    | _ -> failwith "Expected DealingAdditionalCardsToPlayer State in CheckForWinningPlayer"

  let internal CheckForInitialWinner( game ) =
    match game with
    | InitialCardsDealt(icd) ->
      if functions_and_stuff.NumericValueOfHand( icd.Hands.PlayerHand ) = BLACKJACK then
        BlackJackGame.GameOver { Deck = icd.Deck; Bet = icd.Bet; Hands = icd.Hands; DoubledDown = false; Winnings = Winnings( game ); Push = false }
      else if functions_and_stuff.NumericValueOfHand( icd.Hands.DealerHand ) = BLACKJACK then
        BlackJackGame.GameOver { Deck = icd.Deck; Bet = icd.Bet; Hands = icd.Hands; DoubledDown = false; Winnings = -icd.Bet; Push = false }
      else
        game
    | _ -> failwith "Expected InitialCardsDealt State in CheckForInitialWinner"

  let CheckForWinner( game ) =
    match game with
    | GameOver(go) ->  // just return it
      BlackJackGame.GameOver { Deck = go.Deck; Bet = go.Bet; Hands = go.Hands; DoubledDown = go.DoubledDown; Winnings = go.Winnings; Push = HandsAreEqual(go.Hands.PlayerHand, go.Hands.DealerHand) }
    | InitialCardsDealt(icd) ->
      CheckForInitialWinner( game )
    | DealingAdditionalCardsToPlayer(dactp) ->
      CheckForWinningPlayer( game )
    | DealingAdditionalCardsToDealer(dactd) ->
      CheckForWinningDealer( game )
    | _ -> failwith "Invalid state for call to CheckForWinner"

  // DealingAdditionalCardsToPlayerState -> DealingAdditionalCardsToPlayerState | AllPlayerCardsDealtState or WeHaveAWinnerState
  let HitPlayer ( game ) =
    match game with
    | DealingAdditionalCardsToPlayer(deactp) ->
      if CanHit( game ) then
        let newHands = functions_and_stuff.Hit( deactp.Hands, false )
        // Note: record; not class: no constructor, no "new"
        // In this case, because ifthen is an expression (not flow control) I actually can't consolidate the call to CheckForWinner; I must repeat it.
        if deactp.DoubledDown then
          let newGame = BlackJackGame.DealingAdditionalCardsToDealer { Deck = deactp.Deck; Bet = deactp.Bet; Hands = newHands; DoubledDown = deactp.DoubledDown }
          CheckForWinner( newGame )
        else
          let newGame = BlackJackGame.DealingAdditionalCardsToPlayer { Deck = deactp.Deck; Bet = deactp.Bet; Hands = newHands; DoubledDown = deactp.DoubledDown }
          CheckForWinner( newGame )
      else failwith "Illegal: player cannot be dealt another card.  Check with CanHit"
    | _ -> failwith "Invalid state for hitting.  Call CanHit to check whether allowed."

  // DealingAdditionalCardsToDealerState -> DealingAdditionalCardsToDealerState | WeHaveAWinnerState
  let HitDealer ( game ) =
    match game with
    | DealingAdditionalCardsToDealer(dactd) ->                               // Lunch & Learn:  Seems like "then" isn't necessary
      if functions_and_stuff.ShouldDealerHit( dactd.Hands.DealerHand ) then  // <-- remove this then.  Then try to figure out what the fudge.
        let newHands = functions_and_stuff.Hit( dactd.Hands, true )
        let newGame = BlackJackGame.DealingAdditionalCardsToDealer {Deck = dactd.Deck; Bet = dactd.Bet; Hands = newHands; DoubledDown = dactd.DoubledDown}
        CheckForWinner( newGame )
      else failwith "Illegal: dealer cannot be dealt another card.  Check with ShouldDealerHit"
    | _ -> failwith "You may only hit the dealer if in DealingAdditionalCardsToDealerState."

  // Lunch & Learn:
  // Could **NOT** figure out how to do this in the C# side
  // DealingAdditionalCardsToPlayerState | DealingAdditionalCardsToPlayer | InitialCardsDealt -> DealingAdditionalCardsToDealer
  let TransitionToDealer( game ) = 
    match game with
    | DealingAdditionalCardsToDealer(dactd) ->
      game
    | DealingAdditionalCardsToPlayer(dactp) ->
      BlackJackGame.DealingAdditionalCardsToDealer {Deck = dactp.Deck; Bet = dactp.Bet; Hands = dactp.Hands; DoubledDown = dactp.DoubledDown}
    | InitialCardsDealt(icd) ->
      BlackJackGame.DealingAdditionalCardsToDealer {Deck = icd.Deck; Bet = icd.Bet; Hands = icd.Hands; DoubledDown = false}
    | _ -> failwith "TransitionToDealer expects DealingAdditionalCardsToPlayerState or InitialCardsDealtState"
