using System;
using System.Collections.Generic;
namespace AnswerJSON

{
    [System.Serializable]
    public class Answer // use gamecontroller
    {
        public string id;
        public int numberQuestion;
        public int numberRoom;
        public string name;
    
        public Answer(string _id, int _numberQuestion, int _numberRoom, string _name)
        {
            id = _id;
            numberQuestion = _numberQuestion;
            numberRoom = _numberRoom;
            name = _name;
        }
    }
}