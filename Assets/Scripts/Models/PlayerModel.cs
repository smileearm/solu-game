using System;
using System.Collections.Generic;

namespace Solu.Model
{
    [System.Serializable]
    public class PlayerModel // use gamecontroller
    {
        public Status status;
        public PlayerData data;
    }

    [System.Serializable]
    public class PlayerData // use gamecontroller
    {
        public string id;
        public string name;
        public string status;
        public int score;
        public int timeAnswer;
    }
}
