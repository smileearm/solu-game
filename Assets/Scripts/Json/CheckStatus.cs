using System;
using System.Collections.Generic;
namespace CheckStatusJSON

{
    [System.Serializable]
    public class CheckStatus // use gamecontroller
    {
        public string password;
        public string status;

        public CheckStatus(string _password, string _status)
        {
            password = _password;
            status = _status;
        }
    }
}