using System.Diagnostics;
Console.Clear();

Stopwatch InefficientStopwatch = new Stopwatch();
Stopwatch EfficientStopwatch = new Stopwatch();

Inefficient_ZombieInfectionCalculation infectionCalculation = new();
InefficientStopwatch.Start();
infectionCalculation.SimulateInfection();
InefficientStopwatch.Stop();
Console.WriteLine($"This method finished in {InefficientStopwatch.ElapsedMilliseconds}ms");
public class Inefficient_ZombieInfectionCalculation
{
    int[,] ZombieMatrix = new int[Console.WindowWidth - 1, Console.WindowHeight - 1];

    void InitializeRandomZombieMatrix()
    {
        for (int Y = 0; Y < ZombieMatrix.GetLength(1); Y++)
        {
            for (int X = 0; X < ZombieMatrix.GetLength(0); X++)
            {
                Random random = new Random();
                int RandomNumber = random.Next(2);
                ZombieMatrix[X, Y] = RandomNumber;
            }
        }
    }

    void PrintZombieMatrix()
    {
        Console.Clear();
        List<List<int>> Rows = new();
        for (int Y = 0; Y < ZombieMatrix.GetLength(1); Y++)
        {
            List<int> Row = new();
            for (int X = 0; X < ZombieMatrix.GetLength(0); X++)
            {
                Row.Add(ZombieMatrix[X, Y]);
            }
            Rows.Add(Row);
        }
        foreach (var Row in Rows)
        {
            foreach (var index in Row)
            {
                if (index == 0)
                {
                    Console.Write($"{index}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{index}");
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }
            Console.Write("|");
            Console.WriteLine();
        }
    }

    void CalculateNextInfection()
    {
        int[,] Copy = (int[,])ZombieMatrix.Clone();

        for (int Y = 0; Y < ZombieMatrix.GetLength(1); Y++)
        {
            for (int X = 0; X < ZombieMatrix.GetLength(0); X++)
            {
                int HorizontalSum = 0;
                int VerticalSum = 0;
                if (X + 1 < ZombieMatrix.GetLength(0) && X - 1 >= 0)
                {
                    HorizontalSum = Copy[X + 1, Y] + Copy[X - 1, Y];
                }
                if (Y + 1 < ZombieMatrix.GetLength(1) && Y - 1 >= 0)
                {
                    VerticalSum = Copy[X, Y + 1] + Copy[X, Y - 1];
                }
                if (VerticalSum == 2 || HorizontalSum == 2)
                {
                    ZombieMatrix[X, Y] = 1;
                }
            }
        }
    }

    public void SimulateInfection()
    {
        InitializeRandomZombieMatrix();
        int NumberOfHumans = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 0).Count();
        int NumberOfZombies = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 1).Count();
        while (NumberOfHumans > 0)
        {
            int[,] PreviousState = (int[,])ZombieMatrix.Clone();
            NumberOfHumans = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 0).Count();
            NumberOfZombies = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 1).Count();
            PrintZombieMatrix();
            CalculateNextInfection();
            Console.WriteLine($"Humans: {NumberOfHumans} Zombies: {NumberOfZombies}");
            // Thread.Sleep(1000);
            if (CheckStaleCondition(ZombieMatrix, PreviousState)) break;
        }
    }

    bool CheckStaleCondition(int[,] currentState, int[,] previousState)
    {
        for (int Y = 0; Y < currentState.GetLength(1); Y++)
        {
            for (int X = 0; X < currentState.GetLength(0); X++)
            {
                if (currentState[X, Y] != previousState[X, Y])
                {
                    return false;
                }
            }
        }
        return true;
    }
}
