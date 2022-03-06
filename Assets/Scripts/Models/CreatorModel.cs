using System;
using System.Collections.Generic;

namespace Solu.Model
{
    [System.Serializable]
    public class bankerModel // use gamecontroller
    {
        public Status status;
        public bankerData data;
    }

  [System.Serializable]
    public class bankerData // use gamecontroller
    {
        public string id;
        public string name;
        public string status;
    }
}
