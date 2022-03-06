using System.Collections.Generic;

namespace SoluDelegate
{
    public static class QuestionDelegate
    {
        public static string checkAnswer { get; set; } // use gamecontroller
        public static int numberQuestion {get; set;} // use lobby
        public static bool firstGame = true; // use lobby
        public static int score { get; set; } // use lobby
        public static int number { get; set; } //user gamecontroller
        public static int skipGame { get; set; } //user gamecontroller
        
        //public static int numberPlayer { get; set; }

    }

    public static class StatusDelegate
    {
        public static int code { get; set; }
    }

    public static class RoomDelegate
    {
        public static int password { get; set; } //use startgame
        public static int numberPlayer { get; set; } //use startgame
    }

    public static class UserDelegate
    {
        public static string username { get; set; } //use startgame
        public static string status { get; set; } //use startgame
        public static bool checkUsername { get; set; }
        public static string _id { get; set; } //use startgame
    }

    public static class UserDataDelgate
    {
        public static string username { get; set; } //use startgame
        public static string _id { get; set; } //use startgame
    }

    //public static class socketIdDelegate 
    //{
    //    public static string socketId { get; set; }

    //}

}
