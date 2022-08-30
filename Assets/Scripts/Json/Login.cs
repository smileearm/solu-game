using System;
using System.Collections.Generic;
namespace LoginJSON

{
    [System.Serializable]
    public class Login // use Lobby
    {
        public string playerName;
        public int randomPassword;
        public string status;

        public Login(string _playerName, int _randomPassword, string _status)
        {
            playerName = _playerName;
            randomPassword = _randomPassword;
            status = _status;
        }
    }
}