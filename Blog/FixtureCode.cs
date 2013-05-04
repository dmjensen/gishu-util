namespace MyTests.SlimFixtures
{
    public class Quiz
    {
        private BoneyMImpersonator _impersonator;

        public string howDidRasputinDie()
        {   return _impersonator.HowDidRasputinDie(); }

        public bool wasYesterdaySunny()
        {   return _impersonator.WasYesterdaySunny(); }

        public string whoRunsTheBakerFamily()
        {   return _impersonator.WhoRunsTheBakerFamily(); }
    }
}