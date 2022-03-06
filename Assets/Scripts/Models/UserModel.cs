using System;
using System.Collections.Generic;

namespace Solu.Model
{

    [System.Serializable]
    public class UserModel // use lobby
    {
        public Status status;
        public UserData data;

    }

    [System.Serializable]
    public class UserData
    {
        public string name;
        public int numberPlayer;
        public string id;
        public string type;
    }

}
