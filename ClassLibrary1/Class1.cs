using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("F5EEB632-AD77-419B-938D-963A6039C2B7")]
    public interface IShipFightServer {

        int[,] map1 { get; set;}
        int[,] map2 { get; set;}

        int turn { get; set; }

        bool setField(int[,] field, int client); // ture - если расстановка принята false - если нет

        int PointShootEventHandler(int a, int b, int client); // -1 если не попал 1 если попал 2 если выйграл

        string TurnRequest(int client);  // true если твой ход false если не твой ход

        bool Ready(int client); // true если противник готов . false если противник не готов
        void GameReset();
        
    }

    [ClassInterface(ClassInterfaceType.None)]
    [Guid("65D44511-BFD4-45EF-B5A5-A044B358C364")]
    [ProgId("ShipFightServer")]
    public class ShipFightServer : IShipFightServer {

        class Ship1
        {

        }
        class Ship2
        {

        }
        class Ship3
        {

        }
        class Ship4
        {

        }
        class ShipPool {

        }

        public int[,] map1 { get; set; }
        public int[,] map2 { get; set; }
        public int turn { get; set; } = 1;

        private int killedBy1 = 0;
        private int killedBy2 = 0;

        private bool ready1 = false;
        private bool ready2 = false;

        private int lastA = -1;
        private int lastB = -1;

        public bool setField(int[,] field, int client)
        {
            //int quadro = 0; //must be 1
            //int triple = 0; //must be 2
            //int doble = 0;  //must be 3
            //int single = 0; //must be 4
            //check quadro 

            int counter = 0;
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) { 
                    if (field[i,j] == 1)
                        counter++;
                }
            }

            if (counter == 20)
            {
                if (client == 1)
                {
                    map1 = field;
                    ready1 = true;
                    return true;
                }
                if (client == 2)
                {
                    map2 = field;
                    ready2 = true;  
                    return true;
                }
            } //просто чекает то, если ли на поле 20 клеток
            return false;
        }

        public int PointShootEventHandler(int a, int b, int client) {
            if (client == 1) {
                lastA = a;
                lastB = b;
                turn = 2;
                if (map2[a,b] == 0) {
                    map2[a,b] = -2; //return -1 if miss
                    return -1;
                }
                if (map2[a,b] == 1) {
                    map2[a,b] = -1;
                    killedBy1++;
                    if (killedBy1 == 20) {
                        return 2; //return 2 if win
                    }
                    return 1; // return 1 if hit
                }
            }

            if (client == 2)
            {
                lastA = a;
                lastB = b;
                turn = 1;
                if (map1[a,b] == 0)
                {
                    map1[a,b] = -2;
                    return -1;
                }
                if (map1[a,b] == 1)
                {
                    map1[a,b] = -1;
                    killedBy2++;
                    if (killedBy2 == 20)
                    {
                        return 2;
                    }
                    return 1;
                }
            }
            return -3;
        }
        public string TurnRequest(int client) {
            if (client == 1) {
                if (turn == 1) {
                    return $"{lastA} {lastB} 1";
                }
                return "-1 -1 0";
            }
            if (client == 2) {
                if (turn == 2)
                {
                    return $"{lastA} {lastB} 1";
                }
                return "-1 -1 0";
            }

            return "-1 -1 0";
        }
        public bool Ready(int client) {
            if (client == 1) {
                if (ready2 == true) {
                    return true;
                }
            }
            if (client == 2) {
                if (ready1 == true) {
                    return true;
                }
            }
            return false;
        }

        public void GameReset() {
            this.map1 = new int[10, 10];
            this.map2 = new int[10, 10];
            this.turn = 1;

            this.killedBy1 = 0;
            this.killedBy2 = 0;

            this.ready1 = false;
            this.ready2 = false;

    }

}
}
