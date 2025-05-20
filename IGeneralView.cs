using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame
{
    public delegate void AnswerSelectedHandler(int answerNumber);
    public delegate void GameStartHandler(bool isStarted);
    public delegate void TipRequestedHandler(int tipType);
    internal interface IGeneralView
    {
        event AnswerSelectedHandler AnswerSelected;
        event GameStartHandler GameStarted;
        event TipRequestedHandler TipRequested;

        void DisplayQuestion(string question, string answer1, string answer2, string answer3, string answer4);
        void ShowException(string errorMessage);
        void StartGame(bool isStarted);
        void ShowQuestionNumber(int questionNumber);

        void ShowHostMessage(string message, int displayTime);
        void ShowStudentSuggestion(string suggestion, int displayTime);
        void ShowAudiencePoll(int aPercent, int bPercent, int cPercent, int dPercent, int displayTime);

    }
}
