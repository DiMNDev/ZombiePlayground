using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
Console.Clear();

Stopwatch InefficientStopwatch = new Stopwatch();
Stopwatch EfficientStopwatch = new Stopwatch();
ZombieInfectionCalculations infectionCalculation = new();

// Setup Inefficent Method
// InefficientStopwatch.Start();
// infectionCalculation.inefficient.SimulateInfection();
// InefficientStopwatch.Stop();

// Setup Efficent Method
// EfficientStopwatch.Start();
// infectionCalculation.efficient.SimulateInfection();
// EfficientStopwatch.Stop();

// Log output
// Console.WriteLine($"Inefficient method finished in {InefficientStopwatch.ElapsedMilliseconds}ms");
// Console.WriteLine($"Humans: {infectionCalculation.inefficient.HumanCount} Zombies: {infectionCalculation.inefficient.ZombieCount}");
// Console.WriteLine($"Efficient method finished in {EfficientStopwatch.ElapsedMilliseconds}ms");
// Console.WriteLine($"Humans: {infectionCalculation.efficient.HumanCount} Zombies: {infectionCalculation.efficient.ZombieCount}");

Stopwatch InefficientQStopwatch = new Stopwatch();
Stopwatch EfficientQStopwatch = new Stopwatch();
FoodStorageInventory foodStorage = new();

// Setup Inefficent Method
InefficientQStopwatch.Start();
foodStorage.CreateNewQueue();
foodStorage.foodStorage_Queue.PrintNextItem();
InefficientQStopwatch.Stop();

// Setup Efficent Method
EfficientQStopwatch.Start();
foodStorage.CreateNewPriority();
foodStorage.foodStorage_Priority.PrintNextItem();
EfficientQStopwatch.Stop();
// Log output
Console.WriteLine($"Inefficient method finished in {InefficientQStopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"Efficient method finished in {EfficientQStopwatch.ElapsedMilliseconds}ms");

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

public class FoodStorageInventory
{
    private string FoodFile = "food.json";
    private string FoodExpFile = "FoodExp.json";

    public FoodStorage_Queue foodStorage_Queue { get; set; }
    public FoodStorage_Priority foodStorage_Priority { get; set; }

    public Dictionary<string, int> Food { get; set; } = new();
    public class FoodList
    {
        public List<string> FoodItem { get; set; }
    }
    public FoodStorageInventory()
    {
        AddExp();
    }
    public void CreateNewQueue()
    {
        foodStorage_Queue = new(Food);
    }
    public void CreateNewPriority()
    {
        foodStorage_Priority = new(Food);
    }
    public class FoodInventoryWithExp
    {
        public List<Dictionary<string, int>> Food { get; set; }
    }
    public void AddExp()
    {
        var food = LoadFoodData();
        foreach (var item in food.FoodItem)
        {
            Random random = new();
            int exp = random.Next(60);
            Food.Add(item, exp);
        }
    }
    public void ConvertAndSave()
    {
        var food = LoadFoodData();
        foreach (var item in food.FoodItem)
        {
            Random random = new();
            int exp = random.Next(60);
            Food.Add(item, exp);
        }


        string json = JsonSerializer.Serialize(Food);

        File.WriteAllText(FoodExpFile, json);

        foreach (var item in Food)
        {
            Console.WriteLine($"{item.Key} Exp: {item.Value}");
        }
    }
    public FoodList LoadFoodData()
    {
        var food = JsonSerializer.Deserialize<FoodList>(File.ReadAllText(FoodFile));

        return food;
    }

    public class FoodStorage_Queue
    {
        Queue<KeyValuePair<string, int>> Food { get; set; } = new();
        public FoodStorage_Queue(Dictionary<string, int> food)
        {
            CreateQueue(ref food);
        }
        private void CreateQueue(ref Dictionary<string, int> food)
        {
            for (int i = 0; i < food.Count; i++)
            {
                KeyValuePair<string, int> NextToEat = OrderByExp(food);
                Food.Enqueue(NextToEat);
                food.Remove(NextToEat.Key);
            }
        }
        public KeyValuePair<string, int> OrderByExp(Dictionary<string, int> food)
        {
            int exp = 1000;
            foreach (var item in food)
            {
                exp = Math.Min(item.Value, exp);
            }
            KeyValuePair<string, int> NextToEat = food.FirstOrDefault(f => f.Value == exp);
            return NextToEat;
        }
        public void PrintNextItem()
        {
            for (int i = 0; i < Food.Count; i++)
            {
                KeyValuePair<string, int> Next = Food.Dequeue();
                Console.WriteLine($"{Next.Key} Exp: {Next.Value}");
            }
        }
    }
    public class FoodStorage_Priority
    {
        PriorityQueue<KeyValuePair<string, int>, int> Food { get; set; } = new();
        public FoodStorage_Priority(Dictionary<string, int> food)
        {
            CreateQueue(food);
        }
        private void CreateQueue(Dictionary<string, int> food)
        {
            foreach (var item in food)
            {
                Food.Enqueue(item, item.Value);
            }
        }
        public void PrintNextItem()
        {
            for (int i = 0; i < Food.Count; i++)
            {
                KeyValuePair<string, int> Next = Food.Dequeue();
                Console.WriteLine($"{Next.Key} Exp: {Next.Value}");
            }
        }
    }
}