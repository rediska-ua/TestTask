using System.Collections.Generic;
using System.Linq;

namespace TestTask;

class Program
{

    // Метод для того, что бы найти последовательности в рамках массива (строки или столбца матрицы)

    public static List<Tuple<int, int>> CheckMatchingInArray(int[] array)
    {
        List<Tuple<int, int>> matchValues = new List<Tuple<int, int>>();
        int counter = 1;

        for (int i = 1; i < array.Length; i++)
        {
            int elem = array[i];

            for (int j = i - 1; j < array.Length;)
            {
                if (array[j] != elem)
                {
                    if (counter >= 3)
                    {
                        matchValues.Add(new Tuple<int, int>(i - counter, j));
                    }
                    counter = 1;
                } else
                {
                    counter++;
                    if (i == array.Length - 1 && counter >= 3)
                    {
                        matchValues.Add(new Tuple<int, int>(i - counter + 1, i));
                    }
                }
                break;
            }
        }

        return matchValues;
    }

    // Метод для того, чтобы узнать есть ли вообще на доске какие-то доступные последовательности

    public static Tuple<string, int, List<Tuple<int, int>>> CheckIfThereIsMatching(int[,] board)
    {

        for (int i = 0; i < board.GetUpperBound(0) + 1; i++)
        {
            int[] row = MatrixHelper<int>.GetRow(board, i);
            List<Tuple<int, int>> indecies = CheckMatchingInArray(row);
            if (indecies.Count > 0)
            {
                return new Tuple<string, int, List<Tuple<int, int>>>("row", i, indecies);
            }
        }

        for (int j = 0; j < board.GetUpperBound(1) + 1; j++)
        {
            int[] column = MatrixHelper<int>.GetColumn(board, j);
            List<Tuple<int, int>> indecies = CheckMatchingInArray(column);
            if (indecies.Count > 0)
            {
                return new Tuple<string, int, List<Tuple<int, int>>>("col", j, indecies);
            }
        }

        int noMatchIndex = -1;
        string noMatchString = "noMatch";
        Tuple<int, int> noMatchIndicies = new Tuple<int, int>(-1, -1);
        List<Tuple<int, int>> noMatchIndiciesList = new List<Tuple<int, int>>();
        noMatchIndiciesList.Add(noMatchIndicies);

        return new Tuple<string, int, List<Tuple<int, int>>>(noMatchString, noMatchIndex, noMatchIndiciesList);
    }


    public static void SwapRow(Random rnd, int[,] board, int maxRow)
    {
        for (int i = 0; i <= maxRow; i++)
        {
            for (int j = 0; j < board.GetUpperBound(1) + 1; j++)
            {
                if (i == 0 && board[i, j] == -1)
                {
                    board[i, j] = rnd.Next(0, 4);
                }
                else if (board[i, j] == -1)
                {
                    board[i, j] = board[i - 1, j];
                    board[i - 1, j] = -1;
                    SwapRow(rnd, board, maxRow - 1);
                }
            }
        }
    }

    public static void SwapColumn(Random rnd, int[,] board, int columnIndex)
    {
        int[] col = MatrixHelper<int>.GetColumn(board, columnIndex);

        for (int j = 0; j < col.Length; j ++)
        {
            for (int i = 0; i < col.Length - 1; i++)
            {
                if (col[i] >= 0 && col[i + 1] == -1)
                {
                    int temp = col[i];
                    col[i] = col[i + 1];
                    col[i + 1] = temp;
                }
            }
        }

        for (int k = 0; k < col.Length; k++)
        {
            if (col[k] == -1)
            {
                board[k, columnIndex] = rnd.Next(0, 4);
            } else
            {
                board[k, columnIndex] = col[k];
            }
        }

    }

    public static void Move(int[,] board, Tuple<string, int, List<Tuple<int, int>>> boardInfo)
    {
        Random rnd = new Random();

        if (boardInfo.Item1 == "row")
        {
            Console.WriteLine(boardInfo.Item1);
            Console.WriteLine(boardInfo.Item2);
            foreach (Tuple<int, int> indeciesRow in boardInfo.Item3)
            {
                Console.WriteLine("Start index " + indeciesRow.Item1 + " end index " + indeciesRow.Item2 + '\n');
                for (int j = indeciesRow.Item1; j <= indeciesRow.Item2; j++)
                {
                    board[boardInfo.Item2, j] = -1;
                }
            }

            SwapRow(rnd, board, boardInfo.Item2);
        }
        else
        {
            Console.WriteLine(boardInfo.Item1);
            Console.WriteLine(boardInfo.Item2);
            foreach (Tuple<int, int> indeciesCol in boardInfo.Item3)
            {
                Console.WriteLine("Start index " + indeciesCol.Item1 + " end index " + indeciesCol.Item2 + '\n');
                for (int j = indeciesCol.Item1; j <= indeciesCol.Item2; j++)
                {
                    board[j, boardInfo.Item2] = -1;
                }
            }

            SwapColumn(rnd, board, boardInfo.Item2);
        }
    }

    public static void Play(int m, int n)
    {
        int[,] board = new int[m, n];
        Random rnd = new Random();

        MatrixHelper<int>.FillMatrix(rnd, board);
        MatrixHelper<int>.PrintMatrix(board);

        Tuple<string, int, List<Tuple<int, int>>> boardInfo = CheckIfThereIsMatching(board);
        int counter = 0;

        while (boardInfo.Item1 != "noMatch")
        {
            counter++;
            Move(board, boardInfo);
            Console.WriteLine("Step " + counter + '\n');
            MatrixHelper<int>.PrintMatrix(board);
            boardInfo = CheckIfThereIsMatching(board);
        }
        Console.WriteLine("Game over, no match has been found");
    }

    public static void Main(string[] args)
    {
        int m = 9;
        int n = 9;

        // -1 - пустое место;

        Play(m, n);

    }
}
