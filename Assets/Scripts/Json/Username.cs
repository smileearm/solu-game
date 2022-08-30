using System;
using System.Collections.Generic;
namespace UsernameJSON

{
    [System.Serializable]
    public class Username  // user startgame
    {
        public string username;

        public Username(string _username)
        {
            username = _username;
        }
    }
}