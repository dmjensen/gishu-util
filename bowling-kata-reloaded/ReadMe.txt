# Sep 19, 2009
# Gishu Pillai (gishu dot pillai at gmail dot com)

Purpose: Demonstrate the practice of TDD (and the underlying principles)

Part1:
http://www.xprogramming.com/xpmag/acsBowling
To Summarize:
Score a game of Bowling.
A game is made of 10 frames (turns).
Each frame consists of 2 turns (attempts) to roll the ball and knock down all the 10 standing pins at the end of the lane.
A "strike" is when you knock down all ten pins with your first try in a frame. The frame ends early and the frame score is 10 + the sum of next two attempts.
A "spare" is when you get all ten pins only with your second try in a frame. The frame score is 10 + the score of the next attempt
In case you don't hit all ten pins with two attempts, your score for the frame is the sum of the two attempts. (A "gutter" is when you fail to hit any pin. Score = 0)
The last frame is special, it can have upto three attempts if you have a strike or spare in it so as to have a score. 

The perfect game therefore consists of 12 strikes and a score of 300.

Part2:
- Design a GUI for the bowling game, which is updated with every roll. The display should use the standard notation.
Borrowed the notation for game score from 
http://butunclebob.com/ArticleS.UncleBob.TheBowlingGameKata
See sample_score_sheet.png in this folder.

- The GUI should have a start button for a new game, which should be disabled for the duration of a game and enabled when the game is over.
* Assume that there is a hardwareInterface, which will post notifications of the numbers of pins scored with each roll.

