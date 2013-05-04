using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MakeTonsOfMoneyInc;

namespace TestBowlingScorer
{
    [TestFixture]
    public class TestBowlingGame
    {
        BowlingGame _game;
        [SetUp]
        public void BeforeEachTest()
        {
            _game = new BowlingGame();
        }

        [Test]
        public void TestCreation()
        {
            Assert.IsNotNull(_game);
        }
        
        [Test]
        public void TestScore_ZeroForAllGutterBalls()
        {
            RepeatRollsForFrames(0, 0, 10);
            Assert.AreEqual(0, _game.Score);
        }

        [Test]
        public void TestScore_GameWithNoStrikesOrSpares()
        {
            RepeatRollsForFrames(3, 4, 10);

            Assert.AreEqual(70, _game.Score);
        }

        [Test]
        public void TestScore_GameWithSpares()
        {
            _game.Roll(4); _game.Roll(6);
            RepeatRollsForFrames(3, 4, 9);

            Assert.AreEqual(76, _game.Score);
        }

        [Test]
        public void TestScore_AllSpares()
        {
            RepeatRollsForFrames(6, 4, 10);
            _game.Roll(7);

            Assert.AreEqual(161, _game.Score);
        }

        [Test]
        public void TestScore_OneStrike()
        {
            _game.Roll(10);
            RepeatRollsForFrames(3, 2, 9);

            Assert.AreEqual(60, _game.Score);
        }

        [Test]
        public void TestScore_TwoStrike()
        {
            _game.Roll(10);
            _game.Roll(10);
            RepeatRollsForFrames(3, 2, 8);

            Assert.AreEqual(78, _game.Score);
        }

        [Test]
        public void TestScore_LastFrame_Spare()
        {
            RepeatRollsForFrames(3, 4, 9);
            _game.Roll(6); _game.Roll(4); 
            _game.Roll(10);

            Assert.AreEqual(83, _game.Score);
        }

        [Test]
        public void TestScore_LastFrame_OneStrike()
        {
            RepeatRollsForFrames(3, 4, 9);
            _game.Roll(10);
            _game.Roll(0); _game.Roll(7);

            Assert.AreEqual(80, _game.Score);
        }

        [Test]
        public void TestScore_LastFrame_TwoStrikes()
        {
            RepeatRollsForFrames(3, 4, 9);
            _game.Roll(10);
            _game.Roll(0); _game.Roll(10);

            Assert.AreEqual(83, _game.Score);
        }

        [Test]
        public void TestScore_LastFrame_ThreeStrikes()
        {
            RepeatRollsForFrames(3, 4, 9);
            _game.Roll(10);
            _game.Roll(10); _game.Roll(10);

            Assert.AreEqual(93, _game.Score);
        }

        [Test]
        public void Test_SampleGame()
        {
            foreach( var roll in new int[] {1,4, 4,5, 6,4, 5,5, 10, 
                                         0,1, 7,3, 6,4, 10, 2, 8, 6})
            {
                _game.Roll(roll);
            }

            Assert.AreEqual(133, _game.Score);
        }

        [Test]
        public void Test_PerfectGame()
        {
            for (int i = 0; i < 12; i++)
                _game.Roll(10);

            Assert.AreEqual(300, _game.Score);
        }

        private void RepeatRollsForFrames(int roll1, int roll2, int number_of_frames)
        {
            for (int i = 0; i < number_of_frames; i++)
            {
                _game.Roll(roll1);
                _game.Roll(roll2);
            }
        }



    }
}
