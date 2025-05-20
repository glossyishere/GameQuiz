using System;
using System.Collections.Generic;
using System.Xml;

namespace QuizGame
{
    /// <summary>
    /// Класс для работы с хранилищем вопросов викторины
    /// </summary>
    class Memory
    {
        private readonly List<Question> _questions = new List<Question>();
        private const string QuestionsFilePath = "../../QuestionsForGame.xml";

        /// <summary>
        /// Доступ к списку вопросов (только для чтения)
        /// </summary>
        public IReadOnlyList<Question> Questions => _questions.AsReadOnly();

        /// <summary>
        /// Загружает вопросы из XML-файла
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Load()
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(QuestionsFilePath);

                var rootElement = xmlDoc.DocumentElement;
                if (rootElement == null)
                    return;

                _questions.Clear();

                foreach (XmlElement questionNode in rootElement)
                {
                    var question = ParseQuestionNode(questionNode);
                    if (question != null)
                    {
                        _questions.Add(question);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка загрузки вопросов", ex);
            }
        }

        private Question ParseQuestionNode(XmlElement questionNode)
        {
            var question = new Question();

            var numberAttribute = questionNode.Attributes?["number"];
            question.number = numberAttribute?.Value ?? string.Empty;

            foreach (XmlNode childNode in questionNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "question":
                        question.question = childNode.InnerText;
                        break;
                    case "answer1":
                        question.answer1 = childNode.InnerText;
                        break;
                    case "answer2":
                        question.answer2 = childNode.InnerText;
                        break;
                    case "answer3":
                        question.answer3 = childNode.InnerText;
                        break;
                    case "answer4":
                        question.answer4 = childNode.InnerText;
                        break;
                }
            }

            return question;
        }
    }
}