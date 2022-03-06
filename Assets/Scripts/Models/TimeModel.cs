using System;
using System.Collections.Generic;

namespace Solu.Model
{
    [System.Serializable]
    public class TimerModel // use gamecontroller
    {
        public Status status;
        public Timer data;
    }

    [System.Serializable]
    public class Timer
    {
        public int countdown;
    }
}
