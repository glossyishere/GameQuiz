using System;

namespace QuizGame
{
    internal class Presenter
    {
        private readonly IGeneralView view;
        private readonly Model model;
        private int currentQuestionNumber;
        private const int MaxQuestions = 15;

        public Presenter(IGeneralView view)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.model = new Model();

            view.GameStarted += StartGame;
            view.AnswerSelected += CheckAnswer;
            view.TipRequested += ProcessTip;
        }

        private void StartGame(bool isStarted)
        {
            try
            {
                if (!isStarted)
                {
                    view.StartGame(false);
                    return;
                }

                if (model.CanStartGame())
                {
                    currentQuestionNumber = MaxQuestions;
                    view.ShowQuestionNumber(currentQuestionNumber);
                    DisplayCurrentQuestion();
                }
                else
                {
                    view.ShowException("Для игры необходимо минимум 15 вопросов!");
                }
            }
            catch (Exception ex)
            {
                view.ShowException($"Ошибка при запуске игры: {ex.Message}");
            }
        }

        private void DisplayCurrentQuestion()
        {
            try
            {
                var question = model.GetCurrentQuestion();
                if (question == null)
                {
                    view.ShowException("Не удалось загрузить вопрос");
                    return;
                }

                view.DisplayQuestion(
                    question.question,
                    question.answer1,
                    question.answer2,
                    question.answer3,
                    question.answer4
                );
            }
            catch (Exception ex)
            {
                view.ShowException($"Ошибка при отображении вопроса: {ex.Message}");
            }
        }

        private void CheckAnswer(int selectedAnswer)
        {
            try
            {
                if (model.IsAnswerCorrect(selectedAnswer))
                {
                    currentQuestionNumber--;
                    view.ShowQuestionNumber(currentQuestionNumber);

                    if (currentQuestionNumber == 0)
                    {
                        view.ShowHostMessage("Вы выиграли 1 000 000 радиокоинов!", 5000);
                        ResetGame();
                        return;
                    }

                    DisplayCurrentQuestion();

                    if (currentQuestionNumber == 10)
                        view.ShowHostMessage("Вы можете взять 1 000 радиокоинов и уйти!", 3000);
                    else if (currentQuestionNumber == 5)
                        view.ShowHostMessage("Вы можете взять 32 000 радиокоинов и уйти!", 3000);
                    else
                        view.ShowHostMessage("Правильно!", 1000);
                }
                else
                {
                    view.ShowHostMessage("Вы проиграли", 1000);
                    ResetGame();
                }
            }
            catch (Exception ex)
            {
                view.ShowException($"Ошибка при проверке ответа: {ex.Message}");
                ResetGame();
            }
        }

        private void ProcessTip(int tipType)
        {
            try
            {
                switch (tipType)
                {
                    case 1: // 50/50
                        var reducedQuestion = model.ApplyFiftyFifty();
                        view.DisplayQuestion(
                            reducedQuestion.question,
                            reducedQuestion.answer1,
                            reducedQuestion.answer2,
                            reducedQuestion.answer3,
                            reducedQuestion.answer4
                        );
                        break;

                    case 2: // Звонок другу
                        view.ShowStudentSuggestion(model.GetFriendSuggestion(), 4000);
                        break;

                    case 3: // Помощь зала
                        var pollResults = model.GetAudiencePoll();
                        view.ShowAudiencePoll(
                            pollResults[0],
                            pollResults[1],
                            pollResults[2],
                            pollResults[3],
                            6000
                        );
                        break;

                    default:
                        view.ShowException("Неизвестный тип подсказки");
                        break;
                }
            }
            catch (Exception ex)
            {
                view.ShowException($"Ошибка при обработке подсказки: {ex.Message}");
            }
        }

        private void ResetGame()
        {
            try
            {
                view.StartGame(false);
                currentQuestionNumber = MaxQuestions;
                model.ResetGame();
            }
            catch (Exception ex)
            {
                view.ShowException($"Ошибка при сбросе игры: {ex.Message}");
            }
        }
    }
}