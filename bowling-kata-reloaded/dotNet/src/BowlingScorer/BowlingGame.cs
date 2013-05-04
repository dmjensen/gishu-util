using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MakeTonsOfMoneyInc
{
    public class BowlingGame
    {
        private List<int> _rolls = new List<int>();
        
        private const int TOTAL_FRAMES = 10;
        private const int TEN_PINS = 10;
        private const int DUMMY_ROLL_TO_COMPLETE_FRAME = -2020;

        public void Roll(int pinsScored)
        {
            _rolls.Add(pinsScored);
            if (pinsScored == TEN_PINS)
            {
                _rolls.Add(DUMMY_ROLL_TO_COMPLETE_FRAME);
            }

        }

        public int Score
        {
            get 
            {
                int score = 0;
                for(int frame_number = 1; frame_number <= TOTAL_FRAMES; frame_number++)
                {
                    if ( IsStrikeFrame(frame_number) )
                    {
                        score += (TEN_PINS + GetRoll1ForFrame(frame_number+1) + GetRoll2ForFrame(frame_number+1));
                    }
                    else if (IsSpareFrame(frame_number) )
                    {
                        score += (TEN_PINS + GetRoll1ForFrame(frame_number + 1));
                    }
                    else
                    {
                        score += GetRoll1ForFrame(frame_number) + GetRoll2ForFrame(frame_number);
                    }
                }
                return score;  
            }

        }

        
        
        private int GetRoll1ForFrame(int frame_number)
        {
            return _rolls[(frame_number - 1) * 2];
        }
        private int GetRoll2ForFrame(int frame_number)
        {
            var roll2 = _rolls[(frame_number - 1) * 2 + 1];
            
            return (roll2 == DUMMY_ROLL_TO_COMPLETE_FRAME
                ? GetRoll1ForFrame(frame_number + 1)
                : roll2 );
        }

        private bool IsSpareFrame(int frame_number)
        {
            return (GetRoll1ForFrame(frame_number) + GetRoll2ForFrame(frame_number) == TEN_PINS);
        }
        private bool IsStrikeFrame(int frame_number)
        {
            return (GetRoll1ForFrame(frame_number) == TEN_PINS);
        }

        
        
    }
}
