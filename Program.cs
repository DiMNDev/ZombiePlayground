using System.Collections.Concurrent;
using System.Diagnostics;
Console.Clear();

Stopwatch InefficientStopwatch = new Stopwatch();
Stopwatch EfficientStopwatch = new Stopwatch();
ZombieInfectionCalculations infectionCalculation = new();

// Setup Inefficent Method
InefficientStopwatch.Start();
infectionCalculation.inefficient.SimulateInfection();
InefficientStopwatch.Stop();

// Setup Efficent Method
EfficientStopwatch.Start();
infectionCalculation.efficient.SimulateInfection();
EfficientStopwatch.Stop();

// Log output
Console.WriteLine($"Inefficient method finished in {InefficientStopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"Humans: {infectionCalculation.inefficient.HumanCount} Zombies: {infectionCalculation.inefficient.ZombieCount}");
Console.WriteLine($"Efficient method finished in {EfficientStopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"Humans: {infectionCalculation.efficient.HumanCount} Zombies: {infectionCalculation.efficient.ZombieCount}");
public class ZombieInfectionCalculations
{
    // Change this to change the size of the dataset
    int[,] ZombieMatrix = new int[Console.WindowWidth - 1, Console.WindowHeight - 1];
    public Inefficient inefficient { get; set; }
    public Efficient efficient { get; set; }
    public ZombieInfectionCalculations()
    {
        InitializeRandomZombieMatrix();
        inefficient = new(HardCopy());
        efficient = new(HardCopy());
    }
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

    int[,] HardCopy()
    {
        int width = ZombieMatrix.GetLength(0);
        int height = ZombieMatrix.GetLength(1);
        int[,] newMatrix = new int[width, height];
        for (int Y = 0; Y < ZombieMatrix.GetLength(1); Y++)
        {
            for (int X = 0; X < ZombieMatrix.GetLength(0); X++)
            {
                newMatrix[X, Y] = ZombieMatrix[X, Y];
            }
        }
        return newMatrix;
    }
    public class Inefficient
    {
        int[,] ZombieMatrix { get; set; }
        public int ZombieCount { get; private set; }
        public int HumanCount { get; private set; }
        public Inefficient(int[,] Matrix)
        {
            ZombieMatrix = Matrix;
            HumanCount = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 0).Count();
            ZombieCount = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 1).Count();
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
            // foreach (var Row in Rows)
            // {
            //     foreach (var index in Row)
            //     {
            //         if (index == 0)
            //         {
            //             Console.Write($"{index}");
            //             Console.ForegroundColor = ConsoleColor.White;
            //         }
            //         else
            //         {
            //             Console.ForegroundColor = ConsoleColor.Red;
            //             Console.Write($"{index}");
            //             Console.ForegroundColor = ConsoleColor.White;
            //         }

            //     }
            //     Console.Write("|");
            //     Console.WriteLine();
            // }
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
            while (HumanCount > 0)
            {
                int[,] PreviousState = (int[,])ZombieMatrix.Clone();
                HumanCount = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 0).Count();
                ZombieCount = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 1).Count();
                PrintZombieMatrix();
                CalculateNextInfection();
                Console.WriteLine($"Humans: {HumanCount} Zombies: {ZombieCount}");
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

    public class Efficient
    {
        int[,] ZombieMatrix { get; set; }
        public int ZombieCount { get; private set; }
        public int HumanCount { get; private set; }
        public Efficient(int[,] Matrix)
        {
            ZombieMatrix = Matrix;
            HumanCount = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 0).Count();
            ZombieCount = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 1).Count();
        }

        void PrintZombieMatrix()
        {
            Console.Clear();
            ConcurrentBag<List<int>> Rows = new();
            int width = ZombieMatrix.GetLength(0);
            int height = ZombieMatrix.GetLength(1);

            Parallel.For(0, height, Y =>
                        {
                            List<int> Row = new();

                            for (int X = 0; X < width; X++)
                            {
                                Row.Add(ZombieMatrix[X, Y]);
                            };
                            Rows.Add(Row);
                        });
            // foreach (var Row in Rows.ToList())
            // {
            //     foreach (var index in Row)
            //     {
            //         if (index == 0)
            //         {
            //             Console.Write($"{index}");
            //             Console.ForegroundColor = ConsoleColor.White;
            //         }
            //         else
            //         {
            //             Console.ForegroundColor = ConsoleColor.Blue;
            //             Console.Write($"{index}");
            //             Console.ForegroundColor = ConsoleColor.White;
            //         }
            //     }
            //     Console.Write("|");
            //     Console.WriteLine();
            // }
        }

        void CalculateNextInfection()
        {
            int[,] Copy = (int[,])ZombieMatrix.Clone();
            int width = ZombieMatrix.GetLength(0);
            int height = ZombieMatrix.GetLength(1);

            for (int X = 0; X < width; X++)
            {
                Parallel.For(0, height, Y =>
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
                    });
            }
        }

        public void SimulateInfection()
        {
            HumanCount = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 0).Count();
            ZombieCount = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 1).Count();
            while (HumanCount > 0)
            {
                int[,] PreviousState = (int[,])ZombieMatrix.Clone();
                HumanCount = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 0).Count();
                ZombieCount = ZombieMatrix.Cast<int>().ToArray().Where(i => i == 1).Count();
                PrintZombieMatrix();
                CalculateNextInfection();
                Console.WriteLine($"Humans: {HumanCount} Zombies: {ZombieCount}");
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
}