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

        bool TurnRequest(int client);  // true если твой ход false если не твой ход

        bool Ready(int client); // true если противник готов . false если противник не готов
    }

    class Program
    {


        private static Guid TypeGuid => Guid.Parse("65D44511-BFD4-45EF-B5A5-A044B358C364");
        private static int client = 2;

        private static int[,] MyField = new int[10, 10];
        private static int[,] EnemyField = new int[10, 10];

        static void Main(string[] args)
        {
            var objectType = Type.GetTypeFromCLSID(TypeGuid)
                ?? throw new ArgumentException($"Сборка с идентификатором {TypeGuid} не обнаружена");
            dynamic @object = Activator.CreateInstance(objectType);

            bool success = false;
            while (success != true) {
                GamePreparation();
                @object.setField(MyField, client);
            }

            Draw(MyField, EnemyField);
            Console.WriteLine("Your turn is second");           
            Console.WriteLine("GAME! GAME! GAME!");
            int serverResponse = -3;

        }

        public static void Draw(int[,] UserField, int[,] EnemyField) {
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                    if (UserField[i, j] == 0) {
                        Console.Write("X");
                    };
                    if (UserField[i, j] == 1)
                    {
                        Console.Write("#");
                    };
                        
                }
                Console.Write("\t");
                for (int j = 0; j < 10; j++) {
                    if (EnemyField[i, j] == 0) {
                        Console.Write("X");
                    }
                    if (EnemyField[i, j] == 1) {
                        Console.Write("#");
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
                Console.Write($"Enter {setted_fields + 1}'s coordinate");
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
                    EnemyField[i, j] = 0;
                    if (i == 1 || i == 2)
                    {
                        EnemyField[i, j] = 0;
                    }
                }
            }
        }

        private static void GamePreparation() {
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

    }


}
