using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using System.IO;
using Blackjack;
using FSharpx;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {

		[TestMethod]
		public void ParticularScenario()
		{
			/// Here's what happened (unless I'm crazy):
			/// I was dealt 10, king
			/// dealer had 10, 3
			/// I checked double down
			/// I must have hit (though should've stayed) because I got a 2 (bust)
			/// dealer was then dealt 5 (total 18)
			/// I showed winnings at 20

			// player hand (bust)
			var tenOfClub = Blackjack.Card.NewValueCard( 10, Blackjack.Suit.Club );
			var kingOfDiamond = Blackjack.Card.NewKing( Blackjack.Suit.Diamond );
			var twoOfDiamond = Blackjack.Card.NewValueCard( 2, Blackjack.Suit.Diamond );
			var playerHand = FSharpList.Create( new Card[] { tenOfClub, kingOfDiamond, twoOfDiamond } );

			// dealer hand
			var tenOfHeart = Blackjack.Card.NewValueCard( 10, Blackjack.Suit.Heart );
			var threeOfSpade = Blackjack.Card.NewValueCard( 3, Blackjack.Suit.Spade );
			var dealerHand = FSharpList.Create( new Card[] { tenOfHeart, threeOfSpade } );

			// create the deck without dealt cards.
			var shuffledDeck = Blackjack.functions_and_stuff.Shuffle();
			var listOfUsedCards = new List<Card>{ tenOfClub, kingOfDiamond, twoOfDiamond, tenOfHeart, threeOfSpade/*, fiveOfSpade*/ };
			foreach ( var card in shuffledDeck )
			{
				if ( !listOfUsedCards.Contains(card) )
				{
					listOfUsedCards.Add(card);
				}
			}

			var currentDeck = FSharpList.Create( listOfUsedCards.ToArray() );
			var hands = new Blackjack.Hands( dealerHand, playerHand, currentDeck );
			var game = new Blackjack.Game.NewGameState( currentDeck );
			var dealingToPlayerState = new Blackjack.Game.DealingAdditionalCardsToPlayerState( currentDeck, 10, hands, true );
			var transformAgain = Game.BlackJackGame.DealingAdditionalCardsToDealer.NewDealingAdditionalCardsToPlayer( dealingToPlayerState );  // why is this necessary???
			var newGame = Blackjack.Game.CheckForWinner( transformAgain );
			Assert.IsTrue( newGame.IsGameOver );
			Assert.AreEqual( -20, ((Blackjack.Game.BlackJackGame.GameOver)newGame).Item.Winnings );  // ugly cast
		}

		[TestMethod]
        public void Shuffle()
        {
            var card = Blackjack.functions_and_stuff.GetNextRandomCard();
            var path = Blackjack.functions_and_stuff.GetPathFor(card);
            Assert.IsTrue( File.Exists( path ), "GetXamlPathFor Card returned null or blank string" );

            var shuffledDeck = Blackjack.functions_and_stuff.Shuffle();
            Assert.AreEqual( 52, shuffledDeck.Length );
            var firstCard = shuffledDeck.Head;

            shuffledDeck = Blackjack.functions_and_stuff.Shuffle();
            Assert.AreNotEqual( firstCard, shuffledDeck.Head, "Same first card in 2 shuffles; not likely.  Run again to be sure this test no worky." );
        }

        [TestMethod]
        public void Deal()
        {
            var shuffledDeck = Blackjack.functions_and_stuff.Shuffle();
            var hand = Blackjack.functions_and_stuff.Deal( shuffledDeck );
            Assert.AreEqual( 48, hand.Deck.Length );
        }

        [TestMethod]
        public void Hit()
        {
            var shuffledDeck = Blackjack.functions_and_stuff.Shuffle();
            var hands = Blackjack.functions_and_stuff.Deal( shuffledDeck );
            var currentDeckCount = hands.Deck.Length;
            var handsAfterHit = Blackjack.functions_and_stuff.Hit( hands, true );
            Assert.AreEqual( 3, handsAfterHit.DealerHand.Length );
            Assert.AreEqual( currentDeckCount - 1, handsAfterHit.Deck.Length );
            handsAfterHit = Blackjack.functions_and_stuff.Hit( handsAfterHit, false );
            Assert.AreEqual( 3, handsAfterHit.PlayerHand.Length );
            Assert.AreEqual( 3, handsAfterHit.DealerHand.Length );
        }

        [TestMethod]
        public void ShouldHitMultiple()
        {
            for ( int i = 0; i < 25; i++ )
            {
                ShouldHit();
            }
        }

        private static void ShouldHit()
        {
            var game = Blackjack.Game.PlayerInitialDecision( Blackjack.Game.DealInitialCards( Blackjack.Game.PlaceBet( Blackjack.Game.StartNewGame(), 10.0m ) ), false );
            var numericValueOfPlayerHand = Blackjack.functions_and_stuff.NumericValueOfHand( ( (Blackjack.Game.BlackJackGame.DealingAdditionalCardsToPlayer)game ).Item.Hands.PlayerHand );
            while ( Blackjack.Game.CanHit( game ) && numericValueOfPlayerHand < 16 )
            {
                game = Blackjack.Game.HitPlayer( game );
            }
            while ( !game.IsGameOver )
            {
                game = Blackjack.Game.TransitionToDealer( game );
                var valueOfDealerHand = Blackjack.functions_and_stuff.NumericValueOfHand( ( (Blackjack.Game.BlackJackGame.DealingAdditionalCardsToDealer)game ).Item.Hands.DealerHand );
                if ( Blackjack.functions_and_stuff.ShouldDealerHit( ( (Blackjack.Game.BlackJackGame.DealingAdditionalCardsToDealer)game ).Item.Hands.DealerHand ) )
                {
                    Assert.IsTrue( valueOfDealerHand <= Blackjack.BlackjackDataStructures.DEALER_HIGHEST_SOFT_HIT, "Dealer should hit returned true, but value of hand was greater than constant" );
                    game = Blackjack.Game.HitDealer( game );
                }
                else
                {
                    Assert.IsTrue( valueOfDealerHand > Blackjack.BlackjackDataStructures.DEALER_HIGHEST_SOFT_HIT, "Dealer should hit returned true, but value of hand was greater than constant" );
                    game = Blackjack.Game.CheckForWinner( game );
                }
            }
        }

        [TestMethod]
        public void ValueOfCard()
        {
			// 12)
			/// Check out the "NewAce" you get in C# (though you didn't define such a thing in F#).
			/// These objects are part of the compiled F# code, you only see those objects when importing them into another .Net language.
			/// See also game.IsGameOver above; do intellisense.
			var card = Blackjack.Card.Ace.NewAce( Blackjack.Suit.Club );
            /// You get all sorts of helper functions though you didn't create them
            Assert.IsFalse( card.IsValueCard );

            var numericValue = Blackjack.functions_and_stuff.NumericValue( card );
            Assert.AreEqual( 11, numericValue );

            card = Blackjack.Card.Ace.NewKing( Blackjack.Suit.Club );
            numericValue = Blackjack.functions_and_stuff.NumericValue( card );
            Assert.AreEqual( 10, numericValue );

            card = Blackjack.Card.ValueCard.NewValueCard( 2, Blackjack.Suit.Club );
            numericValue = Blackjack.functions_and_stuff.NumericValue( card );
            Assert.AreEqual( 2, numericValue );
        }

        [TestMethod]
        public void ValueOfHand()
        {
            /// 9, ace, ace == 21
            var aceOfClubs = Blackjack.Card.Ace.NewAce( Blackjack.Suit.Club );
            var aceOfDiamonds = Blackjack.Card.Ace.NewAce( Blackjack.Suit.Diamond );
            var nineOfDiamonds = Blackjack.Card.NewValueCard( 9, Blackjack.Suit.Diamond );
            var hand = new List<Blackjack.Card> { aceOfClubs, aceOfDiamonds, nineOfDiamonds };
            var value = Blackjack.functions_and_stuff.NumericValueOfHand( ListModule.OfSeq(hand) );
            Assert.AreEqual( Blackjack.BlackjackDataStructures.BLACKJACK, value, "2 aces and a 9 expected = blackjack" );

            /// 9, ace == 20
            hand = new List<Blackjack.Card> { aceOfDiamonds, nineOfDiamonds };
            value = Blackjack.functions_and_stuff.NumericValueOfHand( ListModule.OfSeq( hand ) );
            Assert.AreEqual( 20, value, "1 aces and a 9 expected = 20" );

            /// 9, ace, ace, ace == 22
            var aceOfHearts = Blackjack.Card.Ace.NewAce( Blackjack.Suit.Heart );
            hand = new List<Blackjack.Card> { aceOfDiamonds, aceOfClubs, aceOfHearts, nineOfDiamonds };
            value = Blackjack.functions_and_stuff.NumericValueOfHand( ListModule.OfSeq( hand ) );
            Assert.AreEqual( 22, value, "3 aces and a 9 expected = 22" );

            /// 2 face cards == 20
            var queenOfHearts = Blackjack.Card.Ace.NewQueen( Blackjack.Suit.Heart );
            var queenOfDiamonds = Blackjack.Card.Ace.NewQueen( Blackjack.Suit.Diamond );
            hand = new List<Blackjack.Card> { queenOfHearts, queenOfDiamonds };
            value = Blackjack.functions_and_stuff.NumericValueOfHand( ListModule.OfSeq( hand ) );
            Assert.AreEqual( 20, value, "2 queens expected = 20" );

            /// BLACKJACK
            hand = new List<Blackjack.Card> { queenOfDiamonds, aceOfHearts };
            value = Blackjack.functions_and_stuff.NumericValueOfHand( ListModule.OfSeq( hand ) );
            Assert.AreEqual( Blackjack.BlackjackDataStructures.BLACKJACK, value, "queen, ace expected = blackjack" );

            hand = new List<Blackjack.Card> { queenOfHearts, queenOfDiamonds, aceOfHearts };
            value = Blackjack.functions_and_stuff.NumericValueOfHand( ListModule.OfSeq( hand ) );
            Assert.AreEqual( Blackjack.BlackjackDataStructures.BLACKJACK, value, "queen, queen, ace expected = blackjack" );

            /// different order
            hand = new List<Blackjack.Card> { aceOfHearts, queenOfHearts, queenOfDiamonds };
            value = Blackjack.functions_and_stuff.NumericValueOfHand( ListModule.OfSeq( hand ) );
            Assert.AreEqual( Blackjack.BlackjackDataStructures.BLACKJACK, value, "ace, queen, queen expected = blackjack" );

            /// different order
            hand = new List<Blackjack.Card> { queenOfHearts, aceOfHearts, queenOfDiamonds };
            value = Blackjack.functions_and_stuff.NumericValueOfHand( ListModule.OfSeq( hand ) );
            Assert.AreEqual( Blackjack.BlackjackDataStructures.BLACKJACK, value, "queen, ace, queen expected = blackjack" );
        }

        /// <summary>
        /// Note that this is really only a demonstration.  It's virtually impossible to get through a game incorrectly.
        /// I tried to a few times while constructing this test, but failed (good).
        /// Debug through this to see just how visible the game structure is throughout.
        /// </summary>
        [TestMethod]
        public void FullGame()
        {
			var game = GetGameUntilPlayerFinishedHitting();
			if ( game.IsGameOver )
			{
                CheckWinner( game );
            }
            else
            {
                game = Blackjack.Game.TransitionToDealer( game );
            }
            while ( !game.IsGameOver )
            {
                if ( Blackjack.functions_and_stuff.ShouldDealerHit(( (Blackjack.Game.BlackJackGame.DealingAdditionalCardsToDealer)game ).Item.Hands.DealerHand) )
                {
                    game = Blackjack.Game.HitDealer( game );
                }
                if ( game.IsGameOver )
                {
                    CheckWinner( game );
                }
            }
            Assert.IsTrue( game.IsGameOver );
        }



		private static Blackjack.Game.BlackJackGame GetGameUntilPlayerFinishedHitting()
		{
			var game = Blackjack.Game.StartNewGame();
			/// Again; methods to check state for free.
			Assert.IsTrue( game.IsNewGame );
			const decimal BET = 10.0m;
			game = Blackjack.Game.PlaceBet( game, BET );
			game = Blackjack.Game.DealInitialCards( game );
			game = Blackjack.Game.PlayerInitialDecision( game, false );
			var numericValueOfPlayerHand = Blackjack.functions_and_stuff.NumericValueOfHand( ( (Blackjack.Game.BlackJackGame.DealingAdditionalCardsToPlayer)game ).Item.Hands.PlayerHand );
			while ( Blackjack.Game.CanHit( game ) && numericValueOfPlayerHand < 16 )
			{
				game = Blackjack.Game.HitPlayer( game );
			}
			return game;
		}

		private static void CheckWinner( Blackjack.Game.BlackJackGame game )
		{
            var winnerState = ( (Blackjack.Game.BlackJackGame.GameOver)game ).Item;
            var valueOfPlayerHand = Blackjack.functions_and_stuff.NumericValueOfHand( winnerState.Hands.PlayerHand );
            var valueOfDealerHand = Blackjack.functions_and_stuff.NumericValueOfHand( winnerState.Hands.DealerHand );
            if ( winnerState.Winnings == 0 )
            {
                Assert.AreEqual( valueOfDealerHand, valueOfPlayerHand, "Push: hands not equal" );
                Assert.IsTrue( winnerState.Push, "0 winnings; expected push" );
            }
            else if ( winnerState.Winnings > 0 )
            {
                Assert.IsTrue( valueOfPlayerHand <= Blackjack.BlackjackDataStructures.BLACKJACK, "Player won, but value of hand greater than 21" );
                if ( valueOfDealerHand < Blackjack.BlackjackDataStructures.BLACKJACK )
                {
                    Assert.IsTrue( valueOfDealerHand < valueOfPlayerHand, "Player won, but dealer hand not less than player when dealer didn't bust" );
                }
            }
            else
            {
                Assert.IsTrue( valueOfDealerHand <= Blackjack.BlackjackDataStructures.BLACKJACK, "Player lost, but dealer hand over 21" );
                if ( valueOfPlayerHand < Blackjack.BlackjackDataStructures.BLACKJACK )
                {
                    Assert.IsTrue( valueOfPlayerHand < valueOfDealerHand, "Player lost, but player hand not less than dealer when player didn't bust" );
                }
            }
        }

    }
}
