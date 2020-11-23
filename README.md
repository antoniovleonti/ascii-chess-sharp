# ASCII-Chess-II
This is an extremely modular implementation of chess in C#.

Here's what makes this implementation special (even compared to my C implementation):
- Simple data structure
- Totally independent input & output, game management, & board management.
- *100%* modular implementation of pieces

And here's why each of these points matter:

Simple data structure: 
  
  While bitboards are very fast and cool, they are also error
prone and make changing the code extremely difficult. The "board" in this 
implementation of chess consists of just two arrays: one for the pieces, and one to 
keep track of when each square was last interacted with. This is all the information 
needed for even the most complicated movement patterns in chess (such as en passant 
and castling).

Totally independent input & output, game management, & board management:
  
  This implementation uses ASCII diplay as is, but what if you want to use something else?
With the way this is organized, you can simply swap out the IO layer with whatever you want.
For example, it's relatively easy to hook this up to Unity, create a simple UI system, and
use that instead (which I have also done).

*100%* modular implementation of pieces:
  
  Chess is a beautifully simple game... most of the time. Some pieces have some quirks that
seem to make the entire game more difficult to program (namely the pawns and the king).
However, in this implementation, the core game logic and that of the pieces exist and work
completely independently of each other. Anytime the legality of a move is analyzed, potential
checks are being calculated, etc, the game just references each piece's unique move generation
function. Any time a piece is moved, the piece being moved handles any special cases (the king
handles castling, the pawn handles en passant and promotions, etc). This allows the programmer
to very easily add new pieces or modify existing ones.
