using System;
using System.Collections.Generic;
namespace UserIdJSON

{
    [System.Serializable]
    public class UserId  // user startgame
    {
        public string id;
        public int numberRoom;
        public string status;
        public string check;

        public UserId(string _id, int _numberRoom, string _status, string _check)
        {
            id = _id;
            numberRoom = _numberRoom;
            status = _status;
            check = _check;
        }
    }
}