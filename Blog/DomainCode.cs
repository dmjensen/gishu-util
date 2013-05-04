namespace  My.Domain
{
    public interface BoneyMImpersonator
    {
        string HowDidRasputinDie();
        bool WasYesterdaySunny();
        string WhoRunsTheBakerFamily();
    }

    public class BoneyM2000 : BoneyMImpersonator
    {
        public string HowDidRasputinDie()
        {   return "His enemies shot him till he was dead.";  }

        public bool WasYesterdaySunny()
        {   return false;   }

        public string WhoRunsTheBakerFamily()
        {   return "Ma Baker";  }
    }

    public class Wannabe : BoneyMImpersonator
    {
        public string HowDidRasputinDie()
        {   return "Old age?";  }

        public bool WasYesterdaySunny()
        {   return false;   }

        public string WhoRunsTheBakerFamily()
        {   return "The Father?";   }
    }
  }