using System;
using System.Collections.Generic;
using System.Linq;

namespace QuizGame
{
    internal class Model
    {
        private readonly Memory _memory;
        private readonly List<string> _usedQuestions;
        private Question _currentQuestion;
        private string _correctAnswer;

        public Model()
        {
            _memory = new Memory();
            _memory.Load();
            _usedQuestions = new List<string>();
            _currentQuestion = new Question();
        }

        public void ResetGame()
        {
            _usedQuestions.Clear();
        }

        public bool CanStartGame()
        {
            return _memory.Questions.Count >= 15;
        }

        public Question GetCurrentQuestion()
        {
            var rnd = new Random();
            Question question;

            // Получаем случайный неиспользованный вопрос
            var availableQuestions = _memory.Questions
                .Where(q => !_usedQuestions.Contains(q.question))
                .ToList();

            if (availableQuestions.Count == 0)
                throw new InvalidOperationException("No available questions");

            question = availableQuestions[rnd.Next(availableQuestions.Count)];
            _usedQuestions.Add(question.question);
            _correctAnswer = question.answer1;
            _currentQuestion = ShuffleAnswers(question);

            return _currentQuestion;
        }

        private Question ShuffleAnswers(Question question)
        {
            var answers = new List<string> { question.answer1, question.answer2, question.answer3, question.answer4 };

            // Алгоритм Фишера-Йетса для перемешивания
            for (int i = answers.Count - 1; i > 0; i--)
            {
                int j = new Random().Next(i + 1);
                (answers[j], answers[i]) = (answers[i], answers[j]);
            }

            return new Question
            {
                question = question.question,
                answer1 = answers[0],
                answer2 = answers[1],
                answer3 = answers[2],
                answer4 = answers[3]
            };
        }

        public bool IsAnswerCorrect(int answerNumber)
        {
            if (answerNumber < 1 || answerNumber > 4)
                return false;

            string selectedAnswer = answerNumber == 1 ? _currentQuestion.answer1 :
                                  answerNumber == 2 ? _currentQuestion.answer2 :
                                  answerNumber == 3 ? _currentQuestion.answer3 :
                                  answerNumber == 4 ? _currentQuestion.answer4 :
                                  string.Empty;

            return selectedAnswer == _correctAnswer;
        }

        public Question ApplyFiftyFifty()
        {
            var answers = new List<string> { _correctAnswer };
            var wrongAnswers = new List<string>
            {
                _currentQuestion.answer1,
                _currentQuestion.answer2,
                _currentQuestion.answer3,
                _currentQuestion.answer4
            }.Where(a => a != _correctAnswer).ToList();

            // Добавляем один неверный ответ
            answers.Add(wrongAnswers[new Random().Next(wrongAnswers.Count)]);

            // Заполняем оставшиеся варианты пустыми строками
            while (answers.Count < 4)
                answers.Add(string.Empty);

            _currentQuestion = new Question
            {
                question = _currentQuestion.question,
                answer1 = answers[0],
                answer2 = answers[1],
                answer3 = answers[2],
                answer4 = answers[3]
            };

            return _currentQuestion;
        }

        public string GetFriendSuggestion()
        {
            return $"Мы думаем, что верный ответ это: {_correctAnswer}";
        }

        public int[] GetAudiencePoll()
        {
            int correctIndex = new List<string>
            {
                _currentQuestion.answer1,
                _currentQuestion.answer2,
                _currentQuestion.answer3,
                _currentQuestion.answer4
            }.IndexOf(_correctAnswer);

            var rnd = new Random();
            var distribution = new int[4];

            // Базовое распределение голосов
            for (int i = 0; i < 4; i++)
                distribution[i] = i == correctIndex ? rnd.Next(40, 70) : rnd.Next(5, 30);

            // Нормализация к 100%
            int total = distribution.Sum();
            return distribution.Select(x => x * 100 / total).ToArray();
        }
    }
}