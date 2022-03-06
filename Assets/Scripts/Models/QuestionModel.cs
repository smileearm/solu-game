using System;
using System.Collections.Generic;


namespace Solu.Model
{
    [System.Serializable]
    public class QuestionModel //use controller
    {
        public Status status;
        public QuestionData data;
    }

    [System.Serializable]
    public class QuestionData
    {
        public Question questionList;
        public int number;
        public int numberProblems;
        public int numberQuestion;
    }

    [System.Serializable]
    public class Question
    {
        public string question;
        public List<ChoiceList> choiceList;
        public string answer;
       
    }

    [System.Serializable]
    public class ChoiceList
    {
        public string id;
        public string text;
    }

}