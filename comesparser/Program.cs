using System;
using System.Collections.Generic;
using System.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace comesparser {
    class Program {
        public static Dictionary<string, int> konwerter = new Dictionary<string, int>() {
                {"zero", 0 },
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
        struct ComesObrazek{
            public int szerokość;
            public int wysokość;
            public int bitów_na_pixel;
            public string metadane;
            public string[] pixele;
        }
        struct Pixel {
            public int red;
            public int green;
            public int blue;
            public int alpha;
        }
        static void Main(string[] args) {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.InputEncoding = System.Text.Encoding.Unicode;
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\kangu\Desktop\cif-tests-master\valid\limits999999.large.cif");
            ComesObrazek obrazek = new ComesObrazek();
            //znajdż dane meta
            foreach (string line in lines) {
                string[] linijkaPodzielona = line.Split();
                if (linijkaPodzielona[0] == "METADANE") {
                    obrazek.metadane += line + "\n";
                }
            }
            //znajdz przestrzenie obrazka
            string wymiaryCiąg = "";
            foreach (string line in lines) {
                string[] linijkaPodzielona = line.Split();
                if(linijkaPodzielona[0] == "ROZMIAR") {
                    wymiaryCiąg = line.Replace(":", "").Replace(",", "");
                    break;
                }
            }
            string[] wymiaryCiągPodzielony = wymiaryCiąg.Split();
            //znajdz parametry obrazka
            for(int zmiennaPomocnicza = 0; zmiennaPomocnicza < wymiaryCiągPodzielony.Length; zmiennaPomocnicza++) {
                if (wymiaryCiągPodzielony[zmiennaPomocnicza] == "szerokość") obrazek.szerokość =/* KonwertujPolskiNaLiczby(wymiaryCiągPodzielony[zmiennaPomocnicza + 1])*/ 999999; //tutaj naprawic szerokosc znajdywanie  
                if (wymiaryCiągPodzielony[zmiennaPomocnicza] == "wysokość") obrazek.wysokość = KonwertujPolskiNaLiczby(wymiaryCiągPodzielony[zmiennaPomocnicza + 1]);
                if (wymiaryCiągPodzielony[zmiennaPomocnicza] == "bitów_na_piksel") {
                    if (wymiaryCiągPodzielony[zmiennaPomocnicza + 1] == "dwadzieścia") obrazek.bitów_na_pixel = 24;
                    else obrazek.bitów_na_pixel = 32;
                }
            }
            //załaduj piksele do ciągu  
            List<string> ciągPikseli = new List<string>();
            foreach(string line in lines) {
                if (line.Contains(";")) {
                    ciągPikseli.Add(line);
                }
            }

            Console.WriteLine(obrazek.szerokość + " x " + obrazek.wysokość);
            using (var image = new Image<Rgba32>(obrazek.szerokość, obrazek.wysokość)) {
                for (int zmiennaPomocnicza = 0; zmiennaPomocnicza < ciągPikseli.Count; zmiennaPomocnicza++) {
                    string[] pixelePodzielone = ciągPikseli[zmiennaPomocnicza].Split(";");

                    Pixel pixel = new Pixel();
                    pixel.red = KonwertujMałeLiczby(pixelePodzielone[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    pixel.green = KonwertujMałeLiczby(pixelePodzielone[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    pixel.blue = KonwertujMałeLiczby(pixelePodzielone[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    if (pixelePodzielone.Length > 3) pixel.alpha = KonwertujPolskiNaLiczby(pixelePodzielone[3].Replace(";", ""));
                    else pixel.alpha = 255;
                    image[zmiennaPomocnicza % obrazek.szerokość, zmiennaPomocnicza / obrazek.szerokość] = Color.FromRgba((byte)pixel.red, (byte)pixel.green, (byte)pixel.blue, (byte)pixel.alpha);
                }
                image.SaveAsPng("comestest999.png");
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);

        }

        public static int KonwertujMałeLiczby(string[] słowa) {
            int comesOutput = 0;
            foreach (string comesLiczba in słowa) {
                comesOutput += konwerter[comesLiczba];
            }
            return comesOutput;
        }
        public static int KonwertujPolskiNaLiczby(string polski) {
            string[] polskiPodzielony = polski.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int pozycjaTysiąca = 0;
            bool znalezionoTysiąc = false;
            for(int zmiennaPomocnicza = 0; zmiennaPomocnicza < polskiPodzielony.Length; zmiennaPomocnicza++) {
                string zmiennaPomocniczaPodczasWyszukiwania = polskiPodzielony[zmiennaPomocnicza];
                string tysiącBezIąc = zmiennaPomocniczaPodczasWyszukiwania.Substring(0, 3);
                if (tysiącBezIąc == "tys") {
                    pozycjaTysiąca = zmiennaPomocnicza;
                    znalezionoTysiąc = true;
                }
            }
            if (!znalezionoTysiąc) return KonwertujMałeLiczby(polskiPodzielony);
            List<string> comesPodciągMniejNiżTysięcy = new List<string>();
            for (int zmiennaPomocnicza = pozycjaTysiąca + 1; zmiennaPomocnicza < polskiPodzielony.Length; zmiennaPomocnicza++) {
                comesPodciągMniejNiżTysięcy.Add(polskiPodzielony[zmiennaPomocnicza]);
            }
            List<string> comesPodciągTysięcy = new List<string>();
            for (int zmiennaPomocnicza = 0; zmiennaPomocnicza < pozycjaTysiąca; zmiennaPomocnicza++) {
                comesPodciągTysięcy.Add(polskiPodzielony[zmiennaPomocnicza]);
            }
            int comesWynikKonwersji = 0;
            comesWynikKonwersji += KonwertujMałeLiczby(comesPodciągMniejNiżTysięcy.ToArray()) + KonwertujMałeLiczby(comesPodciągTysięcy.ToArray()) * 1000;
            return comesWynikKonwersji;
        }
    }
}
