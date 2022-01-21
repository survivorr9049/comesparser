using System;
using System.Collections.Generic;

namespace comesparser {
    class Program {
        public static Dictionary<string, int> konwerter = new Dictionary<string, int>() {
                {"jeden", 1},
                {"dwa", 2},
                {"trzy", 3},
                {"cztery", 4},
                {"pięć", 5},
                {"sześć", 6},
                {"siedem", 7},
                {"osiem", 8},
                {"dziewięć", 9},
                {"dziesięć", 10},
                {"jedenaście", 11},
                {"dwanaście", 12},
                {"trzynaście", 13},
                {"czternaście", 14},
                {"piętnaście", 15},
                {"szesnaście", 16},
                {"siedemnaście", 17},
                {"osiemnaście", 18},
                {"dziewiętnaście", 19},
                {"dwadzieścia", 20},
                {"trzydzieści", 30},
                {"czterdzieści", 40},
                {"pięćdziesiąt", 50},
                {"sześćdziesiąt", 60},
                {"siedemdziesiąt", 70},
                {"osiemdziesiąt", 80},
                {"dziewięćdziesiąt", 90},
                {"sto", 100},
                {"dwieście", 200},
                {"trzysta", 300},
                {"czterysta", 400},
                {"pięćset", 500},
                {"sześćset", 600},
                {"siedemset", 700},
                {"osiemset", 800},
                {"dziewięćset", 900},
                {"tysiąc", 1000}


            };
        static void Main(string[] args) {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.InputEncoding = System.Text.Encoding.Unicode;
            string wejście = Console.ReadLine();
            Console.WriteLine("\n" + KonwertujPolskiNaLiczby(wejście));
        }
        public static int KonwertujMałeLiczby(string[] słowa) {
            int comesOutput = 0;
            foreach (string comesLiczba in słowa) {
                comesOutput += konwerter[comesLiczba];
            }
            return comesOutput;
        }
        public static int KonwertujPolskiNaLiczby(string polski) {
            string[] polskiPodzielony = polski.Split();
            int pozycjaTysiąca = 0;
            for(int zmiennaPomocnicza = 0; zmiennaPomocnicza < polskiPodzielony.Length; zmiennaPomocnicza++) {
                string zmiennaPomocniczaPodczasWyszukiwania = polskiPodzielony[zmiennaPomocnicza];
                string tysiącBezIąc = zmiennaPomocniczaPodczasWyszukiwania.Substring(0, 3);
                if (tysiącBezIąc == "tys") {
                    pozycjaTysiąca = zmiennaPomocnicza;
                }
            }
            List<string> comesPodciągTysięcy = new List<string>();
            List<string> comesPodciągMniejNiżTysięcy = new List<string>();
            for (int zmiennaPomocnicza = 0; zmiennaPomocnicza < pozycjaTysiąca; zmiennaPomocnicza++) {
                comesPodciągTysięcy.Add(polskiPodzielony[zmiennaPomocnicza]);
            }
            for (int zmiennaPomocnicza = pozycjaTysiąca + 1; zmiennaPomocnicza < polskiPodzielony.Length; zmiennaPomocnicza++) {
                comesPodciągMniejNiżTysięcy.Add(polskiPodzielony[zmiennaPomocnicza]);
            }
            int comesWynikKonwersji = 0;
            comesWynikKonwersji += KonwertujMałeLiczby(comesPodciągMniejNiżTysięcy.ToArray()) + KonwertujMałeLiczby(comesPodciągTysięcy.ToArray()) * 1000;
            return comesWynikKonwersji;
        }
    }
}
