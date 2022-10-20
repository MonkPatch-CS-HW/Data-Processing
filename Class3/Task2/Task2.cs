using System.Globalization;
using System.Text;
using OneVariableFunction = System.Func<double, double>;
using FunctionName = System.String;

namespace Task2
{
    public class Task2
    {
/*
 * В этом задании необходимо написать программу, способную табулировать сразу несколько
 * функций одной вещественной переменной на одном заданном отрезке.
 */


// Сформируйте набор как минимум из десяти вещественных функций одной переменной
        internal static Dictionary<FunctionName, OneVariableFunction> AvailableFunctions =
            new Dictionary<FunctionName, OneVariableFunction>
            {
                { "square", x => x * x },
                { "cube", x => x * x * x },
                { "sin", Math.Sin },
                { "cos", Math.Cos },
                { "tg", Math.Tan },
                { "lb", Math.Log2 },
                { "lg", Math.Log10 },
                { "ln", Math.Log },
                { "sqrt", Math.Sqrt },
                { "cbrt", Math.Cbrt },
            };

// Тип данных для представления входных данных
        internal record InputData(double FromX, double ToX, int NumberOfPoints, List<string> FunctionNames)
        {
            public List<double> GeneratePoints()
            {
                var points = new List<double>();
                for (var i = 0; i < NumberOfPoints; i++)
                    points.Add(FromX + (ToX - FromX) * i / (NumberOfPoints - 1));
                return points;
            }
        }

// Чтение входных данных из параметров командной строки
        private static InputData? PrepareData(string[] args)
        {
            if (args.Length <= 3)
                return null;
            var fromX = int.Parse(args[0]);
            var toX = int.Parse(args[1]);
            var numberOfPoints = int.Parse(args[2]);
            var functionNames = new List<string>();
            for (var i = 3; i < args.Length; i++)
                functionNames.Add(args[i]);
            return new InputData(fromX, toX, numberOfPoints, functionNames);
        }

// Тип данных для представления таблицы значений функций
// с заголовками столбцов и строками (первый столбец --- значение x,
// остальные столбцы --- значения функций). Одно из полей --- количество знаков
// после десятичной точки.
        internal record FunctionTable(InputData InputData)
        {
            private List<double> Points { get; } = InputData.GeneratePoints();

            // Код, возвращающий строковое представление таблицы (с использованием StringBuilder)
            // Столбец x выравнивается по левому краю, все остальные столбцы по правому.
            // Для форматирования можно использовать функцию String.Format.
            public override string ToString()
            {
                var colSize = new int[Points.Count + 1];
                var table = new string[InputData.FunctionNames.Count + 1, Points.Count + 1];
                table[0, 0] = @"fn\point";
                colSize[0] = table[0, 0].Length;
                for (var row = 0; row < InputData.FunctionNames.Count; row++)
                {
                    table[row + 1, 0] = InputData.FunctionNames[row];
                    colSize[0] = Math.Max(colSize[0], table[row + 1, 0].Length);
                }

                for (var col = 0; col < Points.Count; col++)
                {
                    table[0, col + 1] = $"{Points[col]:0.######}";
                    colSize[col + 1] = Math.Max(colSize[col + 1], table[0, col + 1].Length);
                }
                for (var row = 0; row < InputData.FunctionNames.Count; row++)
                {
                    var name = InputData.FunctionNames[row];
                    if (!AvailableFunctions.ContainsKey(name))
                        continue;
                    var func = AvailableFunctions[name];
                    for (var col = 0; col < Points.Count; col++)
                    {
                        var point = Points[col];
                        table[row + 1, col + 1] = $"{func(point):0.######}";
                        colSize[col] = Math.Max(colSize[col], table[row + 1, col + 1].Length);
                    }
                }

                var sb = new StringBuilder();
                for (var row = 0; row <= InputData.FunctionNames.Count; row++)
                {
                    for (var col = 0; col <= Points.Count; col++)
                    {
                        sb.Append(table[row, col].PadRight(colSize[col], ' '));
                        if (col != Points.Count)
                            sb.Append(' ');
                    }

                    sb.Append(Environment.NewLine);
                }

                return sb.ToString();
            }
        }

/*
 * Возвращает таблицу значений заданных функций на заданном отрезке [fromX, toX]
 * с заданным количеством точек.
 */
        internal static FunctionTable Tabulate(InputData input)
        {
            return new FunctionTable(input);
        }

        public static void Main(string[] args)
        {
            // Входные данные принимаются в аргументах командной строки
            // fromX fromY numberOfPoints function1 function2 function3 ...
            var input = PrepareData(args);
            if (input == null)
                return;
            Console.WriteLine(Tabulate(input));
        }
    }
}