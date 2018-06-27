using Blackjack;
using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        private const int CARD_HEIGHT = 96;
        private const int CARD_WIDTH = 71;
        private const int CARD_OVERLAP = 13;
        private const int DELAY_TO_VIEW_CARDS = 800;  // ms

        private Blackjack.Game.BlackJackGame _game;

        /// <summary>
        /// Cards from: http://www.jfitz.com/cards/
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        private void UpdateUI( int dealerDelay, int playerDelay )
        {
            UpdateCards( dealerDelay, playerDelay );
            numBet.Enabled = (_game != null) && _game.IsNewGame;
            chkDoubleDown.Enabled = ( _game != null ) && _game.IsInitialCardsDealt;
            if ( _game.IsNewGame )
            {
                chkDoubleDown.Checked = false;
            }
            UpdateWinnings();
        }

        private void UpdateWinnings()
        {
            if ( _game.IsGameOver )
            {
                txtWinnings.ForeColor = Color.Red;
                txtWinnings.Font = new Font( "Garamond", 12, FontStyle.Bold );
                txtWinnings.Text = ( (Blackjack.Game.BlackJackGame.GameOver)_game ).Item.Winnings.ToString();
            }
            else
            {
                txtWinnings.ForeColor = Color.Black;
                txtWinnings.Font = new Font( "Garamond", 10 );
                txtWinnings.Text = "";
            }
        }

        private void UpdateCards( int dealerDelay, int playerDelay )
        {
            pnlDealer.Controls.Clear();
            pnlPlayer.Controls.Clear();
            var hands = GetCurrentHands();
            if ( hands != null )
            {
                UpdateHand( dealerDelay, hands.DealerHand, true );
                UpdateHand( playerDelay, hands.PlayerHand, false );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="hand">Lunch & Learn: Note type: must be marshalled to list in C#</param>
        /// <param name="dealer"></param>
        private void UpdateHand( int delay, FSharpList<Card> hand, bool dealer )
        {
            var left = 0;
            var handReversed = hand.Reverse<Card>().ToList();  // new cards are added to head of list in F# so we must reverse them here.
            for ( int i = 0; i < handReversed.Count; i++ )
            {
                AddCardToDisplay( handReversed[i], left, dealer );
                left += CARD_OVERLAP;
                // pause before last card (usually the new card to show)
                if ( i == handReversed.Count - 1 )
                {
                    Thread.Sleep( delay );
                }
            }
        }

        /// <summary>
        /// Lunch & Learn: drawback of strict states; must cast them each differently to get the goods.
        /// </summary>
        /// <returns></returns>
        private Blackjack.Hands GetCurrentHands()
        {
            if ( _game == null )
            {
                return null;
            }
            else if ( _game.IsInitialCardsDealt )
            {
                return ( (Blackjack.Game.BlackJackGame.InitialCardsDealt)_game ).Item.Hands;
            }
            else if ( _game.IsDealingAdditionalCardsToPlayer )
            {
                return ( (Blackjack.Game.BlackJackGame.DealingAdditionalCardsToPlayer)_game ).Item.Hands;
            }
            else if ( _game.IsDealingAdditionalCardsToDealer )
            {
                return ( (Blackjack.Game.BlackJackGame.DealingAdditionalCardsToDealer)_game ).Item.Hands;
            }
            else if ( _game.IsGameOver )
            {
                return ( (Blackjack.Game.BlackJackGame.GameOver)_game ).Item.Hands;
            }
            else
            {
                return null;
            }
        }

        private void AddCardToDisplay( Blackjack.Card c, int left, bool dealer )
        {
            var cardPath = HideDealerFirstCard( left, dealer ) ? @"C:\Data\SideProjects\BlackJack\Cards\b1fv.png" : functions_and_stuff.GetPathFor( c );
            var panel = new Panel { Height = CARD_HEIGHT, Width = CARD_WIDTH, BackgroundImage = Image.FromFile( cardPath ), Left = left };
            if ( dealer )
            {
                AddCardToPanel( pnlDealer, panel );
            }
            else
            {
                AddCardToPanel( pnlPlayer, panel );
            }
        }

        private bool HideDealerFirstCard( int left, bool dealer )
        {
            return dealer && left == 0 && !( _game.IsGameOver || _game.IsDealingAdditionalCardsToDealer );
        }

        private static void AddCardToPanel( Panel owner, Panel cardPanel )
        {
            var temp = new List<Control>( owner.Controls.Count );
            for ( int i = owner.Controls.Count - 1; i > -1; i-- )
            {
                temp.Add( owner.Controls[i] );
                owner.Controls.RemoveAt( i );
            }
            /// New card added to list first so it appears on top of the earlier drawn cards.
            owner.Controls.Add( cardPanel );
            temp.Reverse();  // Reverse again to get display layering right.
            foreach ( var control in temp )
            {
                owner.Controls.Add( control );
            }
        }

        private void Form1_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.N )
            {
                StartNewGame();
                numBet.Focus();
            }
            if ( e.KeyCode == Keys.H )
            {
                Hit();
                e.Handled = true;
            }
            if ( e.KeyCode == Keys.S )
            {
                PlayerFinishedHitting();
                e.Handled = true;
            }
        }

        private void Hit()
        {
            const string YOU_CAINT_HIT = "You can't hit now, fool";

            if ( numBet.Value <= 0 )
            {
                MessageBox.Show( "Place a bet, fool" );
            }
            else
            {
                if ( _game.IsNewGame )
                {
                    _game = Blackjack.Game.CheckForWinner( Blackjack.Game.DealInitialCards( Blackjack.Game.PlaceBet( _game, numBet.Value ) ) );
                    UpdateUI( DELAY_TO_VIEW_CARDS, DELAY_TO_VIEW_CARDS );
                }
                else if ( _game.IsInitialCardsDealt )
                {
                    _game = Blackjack.Game.HitPlayer( Blackjack.Game.PlayerInitialDecision( _game, chkDoubleDown.Checked ) );  // make decision and hit.
                    if ( _game.IsDealingAdditionalCardsToDealer )
                    {
                        PlayerFinishedHitting();  // If the user doubled down, start hitting for dealer.
                    }
                    UpdateUI( 0, DELAY_TO_VIEW_CARDS );
                }
                else if ( _game.IsDealingAdditionalCardsToPlayer )
                {
                    if ( Blackjack.Game.CanHit( _game ) )
                    {
                        _game = Blackjack.Game.HitPlayer( _game );
                        UpdateUI( DELAY_TO_VIEW_CARDS, 0 );
                        if ( !Blackjack.Game.CanHit( _game ) )
                        {
                            PlayerFinishedHitting();
                        }
                    }
                    else
                    {
                        MessageBox.Show( YOU_CAINT_HIT );
                    }
                }
                else
                {
                    MessageBox.Show( YOU_CAINT_HIT );
                }
            }
        }

        private void PlayerFinishedHitting()
        {
            _game = Blackjack.Game.CheckForWinner( _game );
            if ( !_game.IsGameOver )
            {
                if ( !_game.IsDealingAdditionalCardsToDealer )
                {
                    _game = Blackjack.Game.TransitionToDealer( _game );
                }
                while ( !_game.IsGameOver && Blackjack.functions_and_stuff.ShouldDealerHit( ( (Blackjack.Game.BlackJackGame.DealingAdditionalCardsToDealer)_game ).Item.Hands.DealerHand ) )
                {
                    _game = Blackjack.Game.HitDealer( _game );
                    UpdateUI( DELAY_TO_VIEW_CARDS, 0 );
                }
                _game = Blackjack.Game.CheckForWinner( _game );
            }
            UpdateUI( 0, 0 );
        }

        private void StartNewGame()
        {
            _game = Blackjack.Game.StartNewGame();
            UpdateUI( DELAY_TO_VIEW_CARDS, DELAY_TO_VIEW_CARDS );
        }

        private void Form1_Load( object sender, EventArgs e )
        {
            StartNewGame();
        }

    }
}
