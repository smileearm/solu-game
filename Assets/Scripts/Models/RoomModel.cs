using System;
using System.Collections.Generic;

namespace Solu.Model
{
    [System.Serializable]
    public class RoomModel //use lobby
    {
        public Status status;
        public RoomData data;
    }
   
    [System.Serializable]
    public class RoomData
    {
        public CreatorModel creator;
        public List<PlayersModel> players;
        public int password;
        public int maxPlayers;
        public int minPlayers;
        public int numberPlayer;
        public int timer;
        public string[] playersAnswerQuestionFirst;
    }

    [System.Serializable]
    public class CreatorModel
    {
        public string id;
        public string name;
        public string status;
    }

    [System.Serializable]
    public class PlayersModel
    {
        public string id;
        public string name;
        public string status;
        public int score;
        public int timeAnwser;
        public string type;
    }

    [System.Serializable]
    public class PlayerSortScore // use result
    {
        public Status staus;
        public List<PlayersModel> data;
    }
}