using System;
using System.Collections.Generic;
using System.Linq;

namespace TipaDiplom.Models
{
    // Модель вопроса
    public class TestQuestion
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
        public int Difficulty { get; set; } // 1 - легко, 2 - средне, 3 - сложно

        public TestQuestion()
        {
            Options = new List<string>();
        }
    }

    // Результат теста
    public class TestResult
    {
        public string Category { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public DateTime CompletedAt { get; set; }
        public double Percentage => TotalQuestions > 0
            ? Math.Round((double)CorrectAnswers / TotalQuestions * 100, 1) : 0;
        public string Grade
        {
            get
            {
                if (Percentage >= 90) return "Отлично (5)";
                if (Percentage >= 75) return "Хорошо (4)";
                if (Percentage >= 60) return "Удовлетворительно (3)";
                return "Неудовлетворительно (2)";
            }
        }
    }

    // Банк вопросов
    public static class QuestionBank
    {
        public static List<TestQuestion> GetAllQuestions()
        {
            var questions = new List<TestQuestion>();
            questions.AddRange(GetSetTheoryQuestions());
            questions.AddRange(GetBooleanAlgebraQuestions());
            questions.AddRange(GetGraphTheoryQuestions());
            questions.AddRange(GetCombinatoricsQuestions());
            questions.AddRange(GetNumberSystemQuestions());
            questions.AddRange(GetMatrixQuestions());
            return questions;
        }

        public static List<TestQuestion> GetQuestionsByCategory(string category)
        {
            return GetAllQuestions().Where(q => q.Category == category).ToList();
        }

        public static List<string> GetCategories()
        {
            return new List<string>
            {
                "Теория множеств",
                "Булева алгебра",
                "Теория графов",
                "Комбинаторика",
                "Системы счисления",
                "Матрицы и отношения",
                "Все темы"
            };
        }

        // ═══════════════════════════════════════════
        //  ВОПРОСЫ ПО ТЕОРИИ МНОЖЕСТВ
        // ═══════════════════════════════════════════
        public static List<TestQuestion> GetSetTheoryQuestions()
        {
            return new List<TestQuestion>
            {
                new TestQuestion
                {
                    Id = 1,
                    Category = "Теория множеств",
                    QuestionText = "Даны множества A = {1, 2, 3} и B = {2, 3, 4}.\nЧему равно A ∪ B?",
                    Options = new List<string> { "{2, 3}", "{1, 2, 3, 4}", "{1, 4}", "{1, 2, 3}" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Объединение A ∪ B содержит все элементы, принадлежащие хотя бы одному из множеств: {1, 2, 3, 4}",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 2,
                    Category = "Теория множеств",
                    QuestionText = "Даны множества A = {1, 2, 3, 4} и B = {3, 4, 5}.\nЧему равно A ∩ B?",
                    Options = new List<string> { "{1, 2}", "{3, 4}", "{1, 2, 5}", "{3, 4, 5}" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Пересечение A ∩ B содержит элементы, принадлежащие обоим множествам: {3, 4}",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 3,
                    Category = "Теория множеств",
                    QuestionText = "Даны множества A = {1, 2, 3, 4} и B = {2, 4}.\nЧему равно A \\ B?",
                    Options = new List<string> { "{2, 4}", "{1, 3}", "{1, 2, 3, 4}", "∅" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Разность A \\ B содержит элементы A, не принадлежащие B: {1, 3}",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 4,
                    Category = "Теория множеств",
                    QuestionText = "Сколько элементов содержит булеан (множество всех подмножеств) множества A = {a, b, c}?",
                    Options = new List<string> { "3", "6", "8", "9" },
                    CorrectAnswerIndex = 2,
                    Explanation = "|P(A)| = 2^|A| = 2³ = 8. Булеан содержит все подмножества, включая пустое и само множество.",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 5,
                    Category = "Теория множеств",
                    QuestionText = "Что такое симметрическая разность A △ B?",
                    Options = new List<string>
                    {
                        "(A \\ B) ∪ (B \\ A)",
                        "(A ∪ B) \\ (A ∩ B)",
                        "Оба ответа верны",
                        "Ни один ответ не верен"
                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "Симметрическая разность определяется двумя эквивалентными способами:\nA △ B = (A \\ B) ∪ (B \\ A) = (A ∪ B) \\ (A ∩ B)",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 6,
                    Category = "Теория множеств",
                    QuestionText = "По закону де Моргана, чему равно дополнение (A ∪ B)'?",
                    Options = new List<string> { "A' ∪ B'", "A' ∩ B'", "A ∩ B", "(A ∩ B)'" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Закон де Моргана: (A ∪ B)' = A' ∩ B'\nДополнение объединения равно пересечению дополнений.",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 7,
                    Category = "Теория множеств",
                    QuestionText = "Сколько элементов в декартовом произведении A × B,\nесли |A| = 3, |B| = 4?",
                    Options = new List<string> { "7", "12", "64", "24" },
                    CorrectAnswerIndex = 1,
                    Explanation = "|A × B| = |A| · |B| = 3 · 4 = 12",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 8,
                    Category = "Теория множеств",
                    QuestionText = "Если A ⊂ B, то что верно?",
                    Options = new List<string>
                    {
                        "A = B",
                        "Все элементы A принадлежат B, но A ≠ B",
                        "B содержится в A",
                        "A и B не пересекаются"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "A ⊂ B означает строгое включение: A является подмножеством B, но A ≠ B",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 9,
                    Category = "Теория множеств",
                    QuestionText = "По формуле включений-исключений: |A ∪ B| = ?",
                    Options = new List<string>
                    {
                        "|A| + |B|",
                        "|A| + |B| - |A ∩ B|",
                        "|A| · |B|",
                        "|A| + |B| + |A ∩ B|"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Формула включений-исключений для двух множеств:\n|A ∪ B| = |A| + |B| - |A ∩ B|",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 10,
                    Category = "Теория множеств",
                    QuestionText = "Какое из свойств НЕ является свойством операций над множествами?",
                    Options = new List<string>
                    {
                        "A ∪ B = B ∪ A (коммутативность)",
                        "A ∪ (B ∪ C) = (A ∪ B) ∪ C (ассоциативность)",
                        "A ∪ (B ∩ C) = (A ∪ B) ∩ C (дистрибутивность)",
                        "A ∪ ∅ = A (свойство пустого множества)"
                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "Правильная дистрибутивность:\nA ∪ (B ∩ C) = (A ∪ B) ∩ (A ∪ C)\nВ варианте пропущена часть формулы.",
                    Difficulty = 3
                }
            };
        }

  
        public static List<TestQuestion> GetBooleanAlgebraQuestions()
        {
            return new List<TestQuestion>
            {
                new TestQuestion
                {
                    Id = 101,
                    Category = "Булева алгебра",
                    QuestionText = "Чему равно значение выражения 1 ∧ 0 ∨ 1?",
                    Options = new List<string> { "0", "1", "Не определено", "2" },
                    CorrectAnswerIndex = 1,
                    Explanation = "1 ∧ 0 = 0, затем 0 ∨ 1 = 1.\nКонъюнкция имеет больший приоритет, чем дизъюнкция.",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 102,
                    Category = "Булева алгебра",
                    QuestionText = "Какая операция соответствует импликации A → B?",
                    Options = new List<string> { "A ∧ B", "¬A ∨ B", "A ∨ ¬B", "¬A ∧ ¬B" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Импликация A → B эквивалентна ¬A ∨ B.\nОна ложна только когда A = 1, B = 0.",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 103,
                    Category = "Булева алгебра",
                    QuestionText = "Что такое СДНФ?",
                    Options = new List<string>
                    {
                        "Совершенная дизъюнктивная нормальная форма",
                        "Совершенная дискретная нормальная форма",
                        "Стандартная дизъюнктивная начальная форма",
                        "Система дизъюнктивных нормальных функций"
                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "СДНФ — Совершенная Дизъюнктивная Нормальная Форма.\nЭто дизъюнкция конъюнкций, где каждая конъюнкция содержит все переменные.",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 104,
                    Category = "Булева алгебра",
                    QuestionText = "Когда импликация A → B принимает значение ЛОЖЬ?",
                    Options = new List<string>
                    {
                        "A = 0, B = 0",
                        "A = 1, B = 0",
                        "A = 0, B = 1",
                        "A = 1, B = 1"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Импликация ложна только в одном случае:\nкогда посылка истинна (A=1), а заключение ложно (B=0).",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 105,
                    Category = "Булева алгебра",
                    QuestionText = "Система булевых функций является полной (по критерию Поста), если...",
                    Options = new List<string>
                    {
                        "Содержит все 16 функций двух переменных",
                        "Не содержится целиком ни в одном из 5 замкнутых классов",
                        "Содержит функции И, ИЛИ, НЕ",
                        "Все функции линейны"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "По критерию Поста система полна ⟺ она не содержится целиком ни в одном из классов: T₀, T₁, S, M, L",
                    Difficulty = 3
                },
                new TestQuestion
                {
                    Id = 106,
                    Category = "Булева алгебра",
                    QuestionText = "Чему равно ¬(A ∧ B) по закону де Моргана?",
                    Options = new List<string> { "¬A ∧ ¬B", "¬A ∨ ¬B", "A ∨ B", "¬A ∧ B" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Закон де Моргана: ¬(A ∧ B) = ¬A ∨ ¬B\nОтрицание конъюнкции = дизъюнкция отрицаний.",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 107,
                    Category = "Булева алгебра",
                    QuestionText = "Штрих Шеффера (A | B) — это:",
                    Options = new List<string> { "A ∧ B", "¬(A ∧ B)", "A ∨ B", "¬(A ∨ B)" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Штрих Шеффера (NAND): A | B = ¬(A ∧ B).\nЭта единственная функция образует полную систему.",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 108,
                    Category = "Булева алгебра",
                    QuestionText = "Функция f(x) = x принадлежит классу T₀ (сохраняющих 0)?",
                    Options = new List<string> { "Да", "Нет" , "Зависит от контекста", "Недостаточно данных"},
                    CorrectAnswerIndex = 0,
                    Explanation = "f(0) = 0, значит функция сохраняет 0 и принадлежит классу T₀.",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 109,
                    Category = "Булева алгебра",
                    QuestionText = "Что такое полином Жегалкина?",
                    Options = new List<string>
                    {
                        "Представление функции через XOR и AND",
                        "Представление функции через OR и NOT",
                        "Минимальная ДНФ",
                        "Каноническая КНФ"
                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "Полином Жегалкина — единственное представление булевой функции в виде XOR (⊕) произведений (∧) переменных.\nНапример: f = 1 ⊕ x ⊕ xy",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 110,
                    Category = "Булева алгебра",
                    QuestionText = "Сколько различных булевых функций от 2 переменных существует?",
                    Options = new List<string> { "4", "8", "16", "32" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Для n переменных существует 2^(2^n) функций.\nДля n=2: 2^(2²) = 2⁴ = 16 функций.",
                    Difficulty = 2
                }
            };
        }


        public static List<TestQuestion> GetGraphTheoryQuestions()
        {
            return new List<TestQuestion>
            {
                new TestQuestion
                {
                    Id = 201,
                    Category = "Теория графов",
                    QuestionText = "Сколько рёбер в полном графе K₅ (5 вершин)?",
                    Options = new List<string> { "5", "10", "15", "20" },
                    CorrectAnswerIndex = 1,
                    Explanation = "В полном графе Kₙ число рёбер = n(n-1)/2 = 5·4/2 = 10",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 202,
                    Category = "Теория графов",
                    QuestionText = "Граф содержит Эйлеров цикл тогда и только тогда, когда:",
                    Options = new List<string>
                    {
                        "Все вершины имеют чётную степень",
                        "Все вершины имеют нечётную степень",
                        "Ровно 2 вершины имеют нечётную степень",
                        "Граф является деревом"
                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "Эйлеров цикл существует ⟺ граф связный и все вершины имеют чётную степень.",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 203,
                    Category = "Теория графов",
                    QuestionText = "Что такое дерево в теории графов?",
                    Options = new List<string>
                    {
                        "Граф без циклов",
                        "Связный граф без циклов",
                        "Граф, в котором все вершины имеют степень 2",
                        "Полный граф"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Дерево — это связный ациклический граф.\nОба условия обязательны: связность и отсутствие циклов.",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 204,
                    Category = "Теория графов",
                    QuestionText = "Сколько рёбер в дереве с n вершинами?",
                    Options = new List<string> { "n", "n - 1", "n + 1", "n(n-1)/2" },
                    CorrectAnswerIndex = 1,
                    Explanation = "В дереве с n вершинами всегда ровно n-1 рёбер.",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 205,
                    Category = "Теория графов",
                    QuestionText = "Алгоритм Дейкстры находит:",
                    Options = new List<string>
                    {
                        "Минимальное остовное дерево",
                        "Кратчайший путь от одной вершины до всех остальных",
                        "Эйлеров цикл",
                        "Хроматическое число"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Алгоритм Дейкстры находит кратчайшие пути от заданной вершины до всех остальных в графе с неотрицательными весами рёбер.",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 206,
                    Category = "Теория графов",
                    QuestionText = "Чему равна сумма степеней всех вершин графа с m рёбрами?",
                    Options = new List<string> { "m", "2m", "m²", "m - 1" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Лемма о рукопожатиях: сумма степеней всех вершин равна 2m (удвоенному числу рёбер), так как каждое ребро вносит вклад 1 в степень каждого из двух концов.",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 207,
                    Category = "Теория графов",
                    QuestionText = "Какой алгоритм используется для поиска минимального остовного дерева?",
                    Options = new List<string> { "Дейкстра", "Флойд-Уоршелл", "Краскал", "BFS" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Алгоритм Краскала (а также алгоритм Прима) находит минимальное остовное дерево графа.",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 208,
                    Category = "Теория графов",
                    QuestionText = "Хроматическое число графа — это:",
                    Options = new List<string>
                    {
                        "Число вершин",
                        "Минимальное число цветов для правильной раскраски",
                        "Число рёбер",
                        "Максимальная степень вершины"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Хроматическое число χ(G) — минимальное число цветов, необходимое для правильной раскраски вершин (соседние вершины имеют разные цвета).",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 209,
                    Category = "Теория графов",
                    QuestionText = "Граф является двудольным тогда и только тогда, когда:",
                    Options = new List<string>
                    {
                        "Он не содержит циклов",
                        "Он не содержит циклов нечётной длины",
                        "Все вершины имеют одинаковую степень",
                        "Он является деревом"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Граф двудольный ⟺ он не содержит циклов нечётной длины (теорема Кёнига).",
                    Difficulty = 3
                },
                new TestQuestion
                {
                    Id = 210,
                    Category = "Теория графов",
                    QuestionText = "Алгоритм BFS использует структуру данных:",
                    Options = new List<string> { "Стек", "Очередь", "Дерево", "Хеш-таблицу" },
                    CorrectAnswerIndex = 1,
                    Explanation = "BFS (поиск в ширину) использует очередь (FIFO).\nDFS (поиск в глубину) использует стек (LIFO).",
                    Difficulty = 1
                }
            };
        }

        public static List<TestQuestion> GetCombinatoricsQuestions()
        {
            return new List<TestQuestion>
            {
                new TestQuestion
                {
                    Id = 301,
                    Category = "Комбинаторика",
                    QuestionText = "Чему равно 5! (5 факториал)?",
                    Options = new List<string> { "25", "60", "120", "720" },
                    CorrectAnswerIndex = 2,
                    Explanation = "5! = 5 × 4 × 3 × 2 × 1 = 120",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 302,
                    Category = "Комбинаторика",
                    QuestionText = "Чему равно C(6,2)?",
                    Options = new List<string> { "12", "15", "30", "720" },
                    CorrectAnswerIndex = 1,
                    Explanation = "C(6,2) = 6!/(2!·4!) = (6·5)/(2·1) = 15",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 303,
                    Category = "Комбинаторика",
                    QuestionText = "Сколько способов выбрать 3 человека из 10 для команды (порядок не важен)?",
                    Options = new List<string> { "120", "720", "30", "1000" },
                    CorrectAnswerIndex = 0,
                    Explanation = "Это сочетания: C(10,3) = 10!/(3!·7!) = 120",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 304,
                    Category = "Комбинаторика",
                    QuestionText = "Чем отличаются размещения от сочетаний?",
                    Options = new List<string>
                    {
                        "Ничем не отличаются",
                        "В размещениях важен порядок, в сочетаниях — нет",
                        "В сочетаниях важен порядок, в размещениях — нет",
                        "Размещения используют повторения"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Размещения (A) учитывают порядок элементов.\nСочетания (C) не учитывают порядок.\nA(n,k) = C(n,k) · k!",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 305,
                    Category = "Комбинаторика",
                    QuestionText = "Сколько существует 4-значных PIN-кодов (цифры 0-9, могут повторяться)?",
                    Options = new List<string> { "5040", "10000", "40", "256" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Это размещения с повторениями: 10⁴ = 10000",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 306,
                    Category = "Комбинаторика",
                    QuestionText = "Чему равно число Каталана C₃?",
                    Options = new List<string> { "3", "5", "14", "42" },
                    CorrectAnswerIndex = 1,
                    Explanation = "C₃ = C(6,3)/(3+1) = 20/4 = 5.\nЧисла Каталана: 1, 1, 2, 5, 14, 42, ...",
                    Difficulty = 3
                },
                new TestQuestion
                {
                    Id = 307,
                    Category = "Комбинаторика",
                    QuestionText = "В треугольнике Паскаля элемент C(n,k) равен:",
                    Options = new List<string>
                    {
                        "C(n-1,k-1) + C(n-1,k)",
                        "C(n-1,k-1) · C(n-1,k)",
                        "C(n,k-1) + C(n,k+1)",
                        "n · C(n-1,k)"
                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "Основное свойство биномиальных коэффициентов:\nC(n,k) = C(n-1,k-1) + C(n-1,k)",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 308,
                    Category = "Комбинаторика",
                    QuestionText = "Чему равно 0! ?",
                    Options = new List<string> { "0", "1", "Не определено", "-1" },
                    CorrectAnswerIndex = 1,
                    Explanation = "По определению 0! = 1. Это соглашение необходимо для корректности комбинаторных формул.",
                    Difficulty = 1
                }
            };
        }

  
        public static List<TestQuestion> GetNumberSystemQuestions()
        {
            return new List<TestQuestion>
            {
                new TestQuestion
                {
                    Id = 401,
                    Category = "Системы счисления",
                    QuestionText = "Чему равно двоичное число 1011 в десятичной системе?",
                    Options = new List<string> { "9", "11", "13", "15" },
                    CorrectAnswerIndex = 1,
                    Explanation = "1011₂ = 1·2³ + 0·2² + 1·2¹ + 1·2⁰ = 8 + 0 + 2 + 1 = 11",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 402,
                    Category = "Системы счисления",
                    QuestionText = "Чему равно число 255 в шестнадцатеричной системе?",
                    Options = new List<string> { "FF", "FE", "100", "F0" },
                    CorrectAnswerIndex = 0,
                    Explanation = "255 = 15·16 + 15 = F·16 + F = FF₁₆",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 403,
                    Category = "Системы счисления",
                    QuestionText = "Чему равно 17 в двоичной системе?",
                    Options = new List<string> { "10000", "10001", "10010", "10011" },
                    CorrectAnswerIndex = 1,
                    Explanation = "17 = 16 + 1 = 2⁴ + 2⁰ = 10001₂",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 404,
                    Category = "Системы счисления",
                    QuestionText = "В восьмеричной системе используются цифры:",
                    Options = new List<string> { "0-7", "0-8", "1-8", "0-9" },
                    CorrectAnswerIndex = 0,
                    Explanation = "В системе с основанием n используются цифры от 0 до n-1.\nВ восьмеричной: 0, 1, 2, 3, 4, 5, 6, 7",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 405,
                    Category = "Системы счисления",
                    QuestionText = "Чему равна сумма 1101₂ + 1011₂?",
                    Options = new List<string> { "10100₂", "11000₂", "11001₂", "11000₂" },
                    CorrectAnswerIndex = 1,
                    Explanation = "1101₂ = 13, 1011₂ = 11.\n13 + 11 = 24 = 11000₂",
                    Difficulty = 2
                }
            };
        }


        public static List<TestQuestion> GetMatrixQuestions()
        {
            return new List<TestQuestion>
            {
                new TestQuestion
                {
                    Id = 501,
                    Category = "Матрицы и отношения",
                    QuestionText = "Отношение является отношением эквивалентности, если оно:",
                    Options = new List<string>
                    {
                        "Рефлексивно и симметрично",
                        "Рефлексивно, симметрично и транзитивно",
                        "Симметрично и транзитивно",
                        "Только рефлексивно"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Отношение эквивалентности обладает тремя свойствами:\n1) Рефлексивность: aRa\n2) Симметричность: aRb → bRa\n3) Транзитивность: aRb ∧ bRc → aRc",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 502,
                    Category = "Матрицы и отношения",
                    QuestionText = "Чему равен определитель матрицы [[1,2],[3,4]]?",
                    Options = new List<string> { "-2", "2", "10", "-10" },
                    CorrectAnswerIndex = 0,
                    Explanation = "det = 1·4 - 2·3 = 4 - 6 = -2",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 503,
                    Category = "Матрицы и отношения",
                    QuestionText = "Транспонирование матрицы — это:",
                    Options = new List<string>
                    {
                        "Умножение всех элементов на -1",
                        "Замена строк столбцами",
                        "Поворот матрицы на 90°",
                        "Обращение матрицы"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "При транспонировании строки становятся столбцами и наоборот: (Aᵀ)ᵢⱼ = Aⱼᵢ",
                    Difficulty = 1
                },
                new TestQuestion
                {
                    Id = 504,
                    Category = "Матрицы и отношения",
                    QuestionText = "Отношение частичного порядка должно быть:",
                    Options = new List<string>
                    {
                        "Рефлексивным, антисимметричным, транзитивным",
                        "Рефлексивным, симметричным, транзитивным",
                        "Антирефлексивным, симметричным",
                        "Только транзитивным"
                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "Частичный порядок — рефлексивное, антисимметричное и транзитивное отношение.\nПример: ≤ на числах.",
                    Difficulty = 2
                },
                new TestQuestion
                {
                    Id = 505,
                    Category = "Матрицы и отношения",
                    QuestionText = "Для каких матриц определено умножение A·B?",
                    Options = new List<string>
                    {
                        "Только квадратных",
                        "Число столбцов A = числу строк B",
                        "Одинакового размера",
                        "Любых матриц"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Умножение A(m×n) · B(n×p) возможно, когда число столбцов A равно числу строк B. Результат — матрица (m×p).",
                    Difficulty = 1
                }
            };
        }
    }
}