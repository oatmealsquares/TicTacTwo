# TicTacTwo
C# console game.
Tic-Tac-Two is a variation of tic-tac-toe. The objective of creating a three-in-a-row is the same, but players are also allowed to move the tic-tac-toe grid and the markers.

The Board
Tic-Tac-Two is played on a 5x5 board with a movable tic-tac-toe (three-column, three-row) grid. At the start of the game, the tic-tac-toe grid is centered at the center cell of the board.

The Pieces
The first player has four "X" pieces and the second player has four "O" pieces to place.

Rules
At the start of the game, each player takes turns placing one of their pieces on any empty cell contained within the tic-tac-toe grid.

Once each player has placed at least two of their pieces, they may do one of three things on their turn: (1) place one of their remaining pieces on an empty cell within the tic-tac-toe grid, (2) move the tic-tac-toe grid such that it is centered at a cell one space horizontally, vertically, or diagonally away from the cell it was originally centered at, or (3) move one of their pieces that is already on the board (regardless of whether it is within the tic-tac-toe grid) to any empty cell within the grid.

The first player to create a horizontal, vertical, or diagonal line of their own pieces contained within the tic-tac-toe grid wins. If in a single move the grid has been moved such that it contains both a three-in-a-row of X pieces and a three-in-a-row of O pieces, then the game is a tie.


Explanation of Game Design
Initiation & termination:
 The game is started with a call to TT2()
 TT2() initializes things like the strategy names and players and prints a menu where the player can play, quit, or switch their player modes to human or computer
 TT2() calls the Play function if the user types to play
 The Play function initializes the board, board pieces, grid, a turn counter equal to 0 that will increment each turn to track who the player is, and starts the game
 The game will go until the player chooses to quit or someone wins or there’s a tie
 It calls the Winner function to check if there is a win, no win, or a tie, and prints accordingly
 The Winner function has a playerWin scoreboard which keeps track of each player’s number of wins; at the end it compares the scores and returns an int representing the player who won (1 for O, -1 for X), or a tie or no win 
 Before quitting, the game thanks the player for playing by assigning the Player to a gameOver boolean variable in TT2()
 
Player function deals with moves
 If no win is detected, the Player function gets a move by calling the HumanStrat or AIStrat that is the player which is identified by finding if the turns is even or odd
 The strategy functions return different length arrays, and that is used to distinguish between different moves; non-move options like quit and the rulebook will have different elements which will be used to return true to    get out of the function or print the rules
 Strategy functions returning a 2-element array are specifying a position to place the player’s piece
 Strategy functions returning an empty 3-element array are specifying a position to move the grid
 In the HumanStrat, the player has the option of exiting out of the option to move the grid; this will change one of the elements in the returned array so that nothing is changed 
 Strategy functions returning a 4-element array are specifying an original position and a new position to move a piece that’s already used
 
HumanStrat
 Prints the options that the player has depending on how many pieces they have on the board
 Takes in user’s choice (forces user to only type one of the choices)
 Depending on the choice, it will call the functions to execute each option, or return an array with specific elements to signal a quit or rulebook
 For the moving grid choice, it will check if array with length of 3 is returned since it signals that the user chose to exit, and the HumanStrat will alter the array it returns to reflect that
 The fail array is length 3 and is returned if the user quits or if the move grid option is picked
 The playerMove function gets a position
 Asks user to enter a row and col
 Uses int.TryParse to force user to enter number, and checks that the row and col are on the grid
 Returns position 
 The smallTracker function shifts the grid
 Makes user enter a direction to shift grid
 Uses int.TryParse to force user to enter number, and checks that the grid is in bounds
 Returns new grid center 
 Returns a length 3 exit array to signal that user chose to cancel the move grid option
 The Option3MovePiece function gets an old and new position 
 Makes user enter valid piece to move
 Uses int.TryParse to force user to enter number, and checks that the piece exists and is in bounds
 Makes user enter valid position to move to
 Uses int.TryParse to force user to enter number, and checks that the position is empty and on the grid 
 Returns old and new positions
 
Ai Strat function calls
 WinMoveDefend
  First checks if a win can be created with a piece (given there are still pieces)
  Returns position of piece
  Second, checks if a win can be created by moving grid
  Moves grid
  Returns 3-length array to AIStrat, which will return it to Play
  Third, checks if opponent can win with a piece
  Returns position of piece 
  Fourth, checks if opponent  can win by moving grid
  Moves grid in opposite direction
  Returns 3-length array to AIStrat, which will return it to Play
  Finally, if the player has used all its pieces, checks if the opponent can win with a piece
  Moves the grid anywhere (random)
  Returns 3-length array to AIStrat, which will return it to Play
  Returns a fail array signaling to AI strat it didn’t  find anything useful
 aiMoveRand
  Returns random untaken position on grid
 AISmall Tracker
  Changes the grid center depending on a number parameter
   If the number is 1, grid moves left; if it’s 8 (9-1), grid moves right, in the opposite direction of when the number is 1; If the number is 2, grid moves up; if it’s 7 (9-2), grid moves down, in the opposite direction of    when the number is 2, and so on with the other numbers 
 InitialMove
  Returns the center piece of the grid or the corner pieces (if the center is taken) or a random piece (if everything i taken somehow)
  Will be called when the player hasn’t put down 2 pieces yet and there is nothing to win or defend
 Ai strat vars
  aiChoiceR: random number to choose between placing piece or shifting grid when the player has put down between 2 or 3 pieces
  forGridShift: 3-length array to return if ai decides to do random grid shift 
  origCent: a copy of the original gridCenter for finding a random position or grid center, since the original gridCenter might change in WinMoveDefend"

The AI strat categorizes the problem into one of three situations. Mainly, it calls the WinMoveDefend function that returns a move to win or else block the opponent; otherwise it returns a random piece or random shift of the grid:

1st Scenario: Player has not put down two pieces yet
If opponent has not put down 2 pieces yet, prioritize center and then corner positions
If all positions are somehow taken, a random piece will be placed (however, this should never happen since the center + corners = 5 places)
If opponent has put down second piece, move will consider putting a piece down to block the opponent by calling WinMoveDefend
WinMoveDefend returns the first winning or blocking move that can be made by using a piece placement or grid shift (for now, it ignores wins or losses created by  swapping pieces)

2nd Scenario: Player has put down at least 2 pieces but not 4 yet
WinMoveDefend is called
Strat puts a piece somewhere if it will create a win
Shifts grid if it will create win 
If no win, Strat puts a piece somewhere to prevent the opponent from winning
If no need for a blocking piece, but opponent can win by shifting the grid, Strat shifts the grid away from where the opponent would want to shift it
If WinMoveDefend identifies no urgent wins/losses, it will return empty fail array{ }
A random AI choice decides whether to randomly put a piece on the grid or move the grid 

3rd Scenario: Player has used up all pieces, so they can only swap or move grid
WinMoveDefend is called
Strat shifts grid if it will create win 
If no win, but opponent can win by shifting the grid, Strat shifts the grid away from where the opponent would want to shift it
If WinMoveDefend identifies no urgent wins/losses, it will return empty fail array{ }
A random AI choice moves the grid randomly 
