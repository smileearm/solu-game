namespace Constants
{
    public static class SceneConstants
    {
        public const string LoadingGame = "LoadingGame";
        public const string StartGame = "StartGame";
        public const string LobbyGame = "LobbyGame";
        public const string LoginGame = "LoginGame";
        public const string PlayGame = "PlayGame";
        public const string ResultGame = "ResultGame";
		public const string SummaryGame = "SummaryGame";
    }

    public static class PersistentKey
    {
        public const string Username = "Username";
        public const string Score = "Score";
        public const string Pass = "Pass";
    }

    public static class SocketEvent
    {
        public const string Connect = "connect";
        public const string AllData = "alldata";
        public const string Broadcast = "broadcast";

        //StartGame
        public const string CheckPlayerName = "checkplayername";

        //LoginGame
        public const string CheckPassword = "checkpassword";

        //LobbyGame
        public const string CreateRoom = "createroom";
        public const string JoinRoom = "joinroom";
        public const string PlayerUpdate = "playerupdate";
        public const string ChangeStartScene = "changestartscene";
        public const string ChangeCancelScene = "changecancelscene";
        public const string CancelGame = "cancelgame";
        public const string GetPlayerInRoom = "playerInRoom";

        //PlayGame
        public const string StartGame = "startgame";

        //ResultGame
        public const string ChangePlaygameScene = "changeplaygamescene";
        public const string BoardcastToScence = "boardcasttoscence";
        public const string SortScore = "sortscore";


        //SummaryGame
        public const string ChangeLobbyScene = "changelobbyscene";
    }
}