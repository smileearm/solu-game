using System;
using System.Collections.Generic;
namespace Userdata

{
    [System.Serializable]
    public class Username  // user startgame
    {
        public string username;
        //public bool check;

        //public Username(string _username, bool _check)



        public Username(string _username)
        {
            username = _username;
            //check = _check;
        }
    }

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

    [System.Serializable]
    public class Answer // use gamecontroller
    {
        public string id;
        //public int timeAnswer;
        public int numberQuestion;
        public int numberRoom;
        public string name;

        public Answer(string _id, int _numberQuestion, int _numberRoom, string _name)
        //public Answer(string _id, int _timeAnswer, int _numberQuestion, int _numberRoom, string _name)
        {
            id = _id;
            //timeAnswer = _timeAnswer;
            numberQuestion = _numberQuestion;
            numberRoom = _numberRoom;
            name = _name;
        }
    }

    [System.Serializable]
    public class Checkstatus // use gamecontroller
    {
        public string password;
        public string status;

        public Checkstatus(string _password, string _status)
        {
            password = _password;
            status = _status;
        }
    }

    [System.Serializable]
    public class UserData1
    {
        public int num;
        public string id;

        public UserData1(int _num, string _id)
        {
            num = _num;
            id = _id;
        }
    }

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

    //[System.Serializable]
    //public class UserData
    //{
    //    public Room room;

    //    public UserData(Room _room)
    //    {
    //        room = _room;
    //    }
    //}

    //[System.Serializable]
    //public class Room
    //{
    //    public int password;
    //    public List<NameUser> nameUser;

    //    public Room(int _passward, List<NameUser> _nameUser)
    //    {
    //        password = _passward;
    //        nameUser = _nameUser;
    //    }
    //}

    //[System.Serializable]
    //public class NameUser
    //{
    //    public string name;
    //    public int score;

    //    public NameUser(string _name, int _score)
    //    {
    //        name = _name;
    //        score = _score;
    //    }
    //}
}
