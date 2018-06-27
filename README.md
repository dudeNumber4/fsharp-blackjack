# fsharp-blackjack
Demonstrates many F# features in a fun, pure functional blackjack library.

## Overview
1. BlackjackLib:  F# blackjack "engine"
2. UnitTestProject1:  C# unit tests of BlackjackLib
3. WindowsFormsApplication1:  Winforms (olde school windows UI framework, but very easy to work with) application.
Upon running, see the key commands in the window title.

### Highlights
* Search for numbered highlights in the form "N)" where N = 1-13
* BlackjackDataStructures.fs: This (short) listing is a thing of beauty.  You can see at a glance what each structure does.
* Game.fs:  Heart of the game engine.  The functions therein perhaps aren't as readable as the types, but the essence is the BlackJackGame discriminated union is the game.
That type is manifested over the states listed above it.

_Note_: Change the hard coded path listed in BlackjackDataStructures.fs to where your Cards directory (included in project) lives.