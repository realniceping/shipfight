using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;


namespace ClassLibrary1
{
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("F5EEB632-AD77-419B-938D-963A6039C2B7")]
    public interface IShipFightServer {

        int[,] map1 { get; set;}
        int[,] map2 { get; set;}

        int turn { get; set; }

        int killedBy1 { get; set; }
        int killedBy2{ get; set; }

        bool ready1 { get; set; }
        bool ready2 { get; set; }

        int lastA { get; set; }
        int lastB { get; set; }

        bool setField(int[,] field, int client); // ture - если расстановка принята false - если нет

        int PointShootEventHandler(int a, int b, int client); // -1 если не попал 1 если попал 2 если выйграл

        string TurnRequest(int client);  // true если твой ход false если не твой ход

        bool Ready(int client); // true если противник готов . false если противник не готов
        void GameReset();

        string sayHello();

        string GetServerInfo();


    }
    //[Guid("36F5011E-390F-4521-9E1B-F1BBEFCE664C")]

    [ClassInterface(ClassInterfaceType.None)]
    [Guid("65D44511-BFD4-45EF-B5A5-A044B358C364")]
    [ProgId("ShipFightServer")]
    [ComVisible(true)]
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

        private static string mem = "memoryMapFile";

        private static MemoryMappedFile buffer = MemoryMappedFile.CreateOrOpen(mem, 830, MemoryMappedFileAccess.ReadWrite);
        private MemoryMappedViewAccessor accessor = buffer.CreateViewAccessor(0, 0);

        private enum table : int
        { 
            map1 = 0,
            map2 = 400,
            turn = 804,
            killedBy1 = 808,
            killedBy2 = 812,
            lastA = 816, 
            lastB = 820,
            ready1 = 824,
            ready2 = 825
        }

        public int[,] map1 { get; set; } // = 400 '' 400
        public int[,] map2 { get; set; } // = 400 '' 800
        public int turn { get; set; } = 1; // = 4 '' 804

        public int killedBy1 { get; set; } = 0; // = 4 '' 808
        public int killedBy2 { get; set; } = 0; // = 4 '' 812

        public int lastA { get; set; } = -1; // = 4 '' 816
        public int lastB { get; set; } = -1; // = 4 '' 820


        public bool ready1 { get; set; } = false; // = 1 '' 821
        public bool ready2 { get; set; } = false; // = 1 '' 822 

        
        public string sayHello() {
            return "hello world";
        }

        public string GetServerInfo() {
            int[,] voidmap = new int[10, 10];
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                    voidmap[i, j] = 0;
                }
            }
            
            
            return $"killedBy1 \t {accessor.ReadInt32((int)table.killedBy1)}\n" +
                $"killedBy2 \t {accessor.ReadInt32((int)table.killedBy2)}\n" +
                $"ready1 \t {accessor.ReadBoolean((int)table.ready1)}\n" +
                $"ready2 \t {accessor.ReadBoolean((int)table.ready2)}\n" +
                $"lastA \t {accessor.ReadInt32((int)table.lastA)}\n" +
                $"lastB \t {accessor.ReadInt32((int)table.lastB)}\n";

        }

        

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
                    

                    for (int i = 0; i < 10; i++) {
                        for (int j = 0; j < 10; j++) {
                            
                            accessor.Write((((int)table.map1) + (((i * 10) + j) * 4)), field[i,j]);
                        }
                    }
                   
                    accessor.Write((int)table.ready1, true);

                    return true;
                }
                if (client == 2)
                {
                    
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            
                            accessor.Write((((int)table.map2) + (((i * 10) + j) * 4)), field[i, j]);
                        }
                    }
                    accessor.Write((int)table.ready2, true);
                    return true;
                }
            } //просто чекает то, если ли на поле 20 клеток
            return false;
        }

        public int PointShootEventHandler(int a, int b, int client) {
            if (client == 1) {
                lastA = a;
                accessor.Write((int)table.lastA, a);
                lastB = b;
                accessor.Write((int)table.lastB, b);
                turn = 2;
                accessor.Write((int)table.turn, 2);
                if (accessor.ReadInt32((a * 10 + b) * 4 + 400) == 0) {

                    accessor.Write(((a * 10 + b) * 4 + 400), -2);
 
                    //return -1 if miss
                    return -1;
                }
                
                if (accessor.ReadInt32((a * 10 + b) * 4 + 400) == 1) {
                   
                    
                    accessor.Write(((a * 10 + b) * 4 + 400), -1);
                 
                    
                 
                    accessor.Write((int)table.killedBy1, accessor.ReadInt32((int)table.killedBy1) + 1);
                   
               
                    if (accessor.ReadInt32((int)table.killedBy1) == 20) {
                        return 2; //return 2 if win
                    }
                    return 1; // return 1 if hit
                }
            }

            if (client == 2)
            {

                accessor.Write((int)table.lastA, a);
                accessor.Write((int)table.lastB, b);
                accessor.Write((int)table.turn, 1);

                if (accessor.ReadInt32((a * 10 + b) * 4) == 0)
                {
                    
                  
                    accessor.Write((a * 10 + b) * 4, -2);
                   
                    
                    return -1;
                }
                
                if (accessor.ReadInt32((a * 10 + b) * 4) == 1)
                {
                                                      
                    accessor.Write((a * 10 + b) * 4, -1);                                                      
                    accessor.Write((int)table.killedBy2, accessor.ReadInt32((int)table.killedBy2) + 1);
                  
                    
                    if (accessor.ReadInt32((int)table.killedBy2) == 20)
                    {
                        return 2;
                    }
                    return 1;
                }
            }
            return -3;
        }
        public string TurnRequest(int client) {
            int t = accessor.ReadInt32((int)table.turn);
            int la = accessor.ReadInt32((int)table.lastA);
            int lb = accessor.ReadInt32((int)table.lastB);

            if (client == 1) {
                if (t == 1) {
                    return $"{la} {lb} 1";
                }
                return "-1 -1 0";
            }
            if (client == 2) {
                if (t == 2)
                {
                    return $"{la} {lb} 1";
                }
                return "-1 -1 0";
            }

            return "-1 -1 0";
        }
        public bool Ready(int client) {
            bool rd1 = accessor.ReadBoolean((int)table.ready1);
            bool rd2 = accessor.ReadBoolean((int)table.ready2);
            if (client == 1) {
                if (rd2 == true) {
                    return true;
                }
            }
            if (client == 2) {
                if (rd1 == true) {
                    return true;
                }
            }
            return false;
        }

        public void GameReset() {

            accessor.Write((int)table.turn, 1);
            accessor.Write((int)table.lastA, -1);
            accessor.Write((int)table.lastB, -1);
            accessor.Write((int)table.killedBy1, 0);
            accessor.Write((int)table.killedBy2, 0);
            accessor.Write((int)table.ready1, false);
            accessor.Write((int)table.ready2, false);
            
            this.turn = 1;

            this.killedBy1 = 0;
            this.killedBy2 = 0;

            this.ready1 = false;
            this.ready2 = false;

        }

}
}
