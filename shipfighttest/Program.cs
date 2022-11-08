using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace call_queue
{
    public interface IShipFightServer
    {

        int[][] map1 { get; set; }
        int[][] map2 { get; set; }

        int turn { get; set; }

        bool setField(int[][] field, int client); // ture - если расстановка принята false - если нет

        int PointShootEventHandler(int a, int b, int client); // -1 если не попал 1 если попал 2 если выйграл

        string TurnRequest(int client);  // true если твой ход false если не твой ход

        bool Ready(int client); // true если противник готов . false если противник не готов
    }

    class Program
    {

        private static Guid TypeGuid => Guid.Parse("65D44511-BFD4-45EF-B5A5-A044B358C364");
        private static int client = 2;

        private static int[,] MyField = new int[10, 10];
        private static int[,] EnemyField = new int[10, 10];

        private static Type objectType = Type.GetTypeFromCLSID(TypeGuid)
                ?? throw new ArgumentException($"Сборка с идентификатором {TypeGuid} не обнаружена");

        private static dynamic @object = Activator.CreateInstance(objectType);

        public static void DrawGameField(int[,] UserField, int[,] EnemyField)
        {
            Console.WriteLine("Your field\t Enemy field");

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (UserField[i, j] == 0)
                    {
                        Console.Write("X");
                    };
                    if (UserField[i, j] == 1)
                    {
                        Console.Write("#");
                    };
                    if (UserField[i, j] == -1) {
                        Console.Write("O");
                    }

                }
                Console.Write("\t");
                for (int j = 0; j < 10; j++)
                {
                    if (EnemyField[i, j] == 0)
                    {
                        Console.Write("X");
                    }
                    if (EnemyField[i, j] == 1)
                    {
                        Console.Write("#");
                    }
                    if (UserField[i, j] == -1)
                    {
                        Console.Write("O");
                    }
                }
                Console.WriteLine();
            }
        }

        private static void ManualSet()
        {

            Console.WriteLine("Enter each coordinate in next format: X Y");
            Console.WriteLine("Where x <= 10 it's column number and y <= 10 it's row numner");
            int setted_fields = 0;
            while (setted_fields < 20)
            {
                string coordinate = String.Empty;
                Console.Write($"Enter {setted_fields + 1}'s coordinate: ");
                coordinate = Console.ReadLine();
                string[] splitted = coordinate.Split(" ");
                int X = Convert.ToInt32(splitted[0]) - 1;
                int Y = Convert.ToInt32(splitted[1]) - 1;
                //here should be error handling

                setted_fields++;
                MyField[X, Y] = 1;
            }
        }

        private static void AutoSet()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    MyField[i, j] = 0;
                    if (i == 1 || i == 2)
                    {
                        MyField[i, j] = 1;
                    }
                }
            }
        }

        private static void GamePreparation()
        {
            Console.WriteLine("Welcome to ship fight game!");
            Console.WriteLine("First thing first you should set your ships in fight field");
            Console.WriteLine("You can do it manualy, or take one of our random pattern");

            Console.WriteLine("1. Manual set");
            Console.WriteLine("2. Auto set");
            Console.Write(">>> ");
            int set = 0;
            set = Convert.ToInt32(Console.ReadLine());
            if (set == 1)
            {
                ManualSet();
            }
            else
            {
                AutoSet();
            }
        }

        private static async Task<string> DolbitsaVServerTurn() {
            string responce = @object.TurnRequest(client);
            
            while (responce == "-1 -1 0") {
                Thread.Sleep(200);
                responce = @object.TurnRequest(client);
                
            }

            return responce;
        }

        private static async Task DolbitsaVServerReady()
        {
            bool status = @object.Ready(client);
            while (status == false) {
                Thread.Sleep(200);
                status = @object.Ready(client);                
            }

        }

        private static void EmenyShootHandler(int a, int b)
        {
            if (a == -1 || b == -1)
            {
                return;
            }
            Console.WriteLine($"Enemy shooted by {a}; {b} ceil");
            if (MyField[a, b] == 1) { 
                MyField[a, b] = -1;
            }
            if (MyField[a, b] == 0) {
                MyField[a, b] = -1;
            }
        }

        private static bool UserShootHandler() { //return true if game end
            int a, b;
            int answer = 0;
            Console.WriteLine("Let's do a turn!");
            Console.Write("Enter X coordinate: ");
            a = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter Y coordinate: ");
            b = Convert.ToInt32(Console.ReadLine());
            
            answer = @object.PointShootEventHandler(a, b, client);

            if (answer == -1) { 
                EnemyField[a, b] = -1;
                return false;
            }
            if (answer == 1) {
                EnemyField[a, b] = 1;
                return false;
            }
            if (answer == 3)
            {
                Console.WriteLine("you shoot to one ceil twice");
            }
            return true;

        }

        static async Task Main(string[] args)
        {
            

            bool success = false;
            while (success != true) {
                GamePreparation();
                success = @object.setField(MyField, client);
            }

            string serverInfo = @object.GetServerInfo();
            Console.WriteLine(serverInfo);
            
            await DolbitsaVServerReady();
           
            Console.WriteLine("you turn first");
            
            Console.WriteLine("GAME! GAME! GAME!");

            bool areGameEnd = false;
            while (!areGameEnd) {
                
                Console.WriteLine("Whit when your emeny will did a turn");
                string responce = await DolbitsaVServerTurn();
                Console.WriteLine("Your turn");
                string[] responceParsed = responce.Split(" ");
                int x = Convert.ToInt32(responceParsed[0]);
                int y = Convert.ToInt32(responceParsed[1]);
                int status = Convert.ToInt32(responceParsed[2]);
                EmenyShootHandler(x, y);
                DrawGameField(MyField, EnemyField);
                areGameEnd = UserShootHandler();

            }

            int serverResponse = -3;

        }

        

    }


}
