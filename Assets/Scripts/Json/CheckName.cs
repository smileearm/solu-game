using System;
using System.Collections.Generic;
namespace CheckNameJSON

{
    [System.Serializable]
    public class CheckName // use gamecontroller
    {
        public string name;
        public string password;

        public CheckName(string _name, string _password)
        {
            name = _name;
            password = _password;
        }
    }
}