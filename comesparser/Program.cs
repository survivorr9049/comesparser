using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SixLabors.ImageSharp;
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
        public static string staryPixel = "";
        public static byte staraWartosc = 0;
        public struct ComesObrazek{
            public ComesObrazek(int x, int y, int bpp) {
                szerokość = x;
                wysokość = y;
                bitów_na_pixel = bpp;
            }
            public int szerokość;
            public int wysokość;
            public int bitów_na_pixel;

        }
        struct Pixel {
            public byte red;
            public byte green;
            public byte blue;
            public byte alpha;
        }
        static void Main(string[] args) {
            Stopwatch czasownik = new Stopwatch();
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.InputEncoding = System.Text.Encoding.Unicode;
            if (args.Length < 2)
            {
                Console.WriteLine("Zbyt mało argumentów! Proszę użyć formatu: program <wejscie> <wyjscie>");
                return;
            }
            string wejsciePlik = args[0];
            czasownik.Start();
            if (!File.Exists(wejsciePlik)){
                Console.WriteLine($"plik {wejsciePlik} nie został znaleziony");
                return;
            }
            string[] linie = File.ReadAllLines(wejsciePlik);
            long czasownikWczytanie = czasownik.ElapsedMilliseconds;
            if (!linie[0].StartsWith("CIF:")) {
                Console.WriteLine("Plik nie jest w formacie CIF.");
                return;
            }
            try{
                int[] test = WczytajDanePixeli(linie[2]);
                ComesObrazek obrazek = new ComesObrazek(test[0], test[1], test[2]);
                List<string> metadane = new List<string>();
                int pozycjaPixeli = 0;
                for (int zmiennaPomocnicza = 3; zmiennaPomocnicza < linie.Length; zmiennaPomocnicza++) {
                    if (linie[zmiennaPomocnicza].StartsWith("METADANE")) metadane.Add(linie[zmiennaPomocnicza]);
                    if (linie[zmiennaPomocnicza].Contains(";")) {
                        pozycjaPixeli = zmiennaPomocnicza;
                        break;
                    }
                }
                List<string> pixele = new List<string>();
                for (int zmiennaPomocnicza = pozycjaPixeli; zmiennaPomocnicza < linie.Length; zmiennaPomocnicza++) {
                    pixele.Add(linie[zmiennaPomocnicza]);
                }
                Pixel pixel = new Pixel();
                if (obrazek.bitów_na_pixel == 32) {
                    using (var image = new Image<Rgba32>(obrazek.szerokość, obrazek.wysokość)) {
                        for (int zmiennaPomocnicza = 0; zmiennaPomocnicza < pixele.Count; zmiennaPomocnicza++) {
                            string[] pixelePodzielone = pixele[zmiennaPomocnicza].Split(";");
                            pixel.red = KonwertujPixele(pixelePodzielone[0]);
                            pixel.green = KonwertujPixele(pixelePodzielone[1]);
                            pixel.blue = KonwertujPixele(pixelePodzielone[2]);
                            pixel.alpha = KonwertujPixele(pixelePodzielone[3]);
                            image[zmiennaPomocnicza % obrazek.szerokość, zmiennaPomocnicza / obrazek.szerokość] = Color.FromRgba(pixel.red, pixel.green, pixel.blue, pixel.alpha);
                        }
                        if (args[1].Substring(args[1].Length - 4, 4) != ".png") args[1] += ".png";
                        image.SaveAsPng(args[1]);
                    }
                }//tak
                else {
                    using (var image = new Image<Rgb24>(obrazek.szerokość, obrazek.wysokość)) {
                        for (int zmiennaPomocnicza = 0; zmiennaPomocnicza < pixele.Count; zmiennaPomocnicza++) {
                            string[] pixelePodzielone = pixele[zmiennaPomocnicza].Split(";");
                            pixel.red = KonwertujPixele(pixelePodzielone[0]);
                            pixel.green = KonwertujPixele(pixelePodzielone[1]);
                            pixel.blue = KonwertujPixele(pixelePodzielone[2]);
                            image[zmiennaPomocnicza % obrazek.szerokość, zmiennaPomocnicza / obrazek.szerokość] = Color.FromRgb(pixel.red, pixel.green, pixel.blue);
                        }
                        if (args[1].Substring(args[1].Length - 4, 4) != ".png") args[1] += ".png";
                        image.SaveAsPng(args[1]);
                    }
                } 
                czasownik.Stop();
                Console.WriteLine($"Plik wczytano w: {czasownikWczytanie} ms");
                Console.WriteLine($"Skonwertwano w: {czasownik.ElapsedMilliseconds - czasownikWczytanie} ms");
                Console.WriteLine($"Całkowity czas egzekucji: {czasownik.ElapsedMilliseconds} ms");
                Console.ReadLine();
            }
            catch
            {
                Console.WriteLine("Coś się popsuło ups :) możesz spróbować ponownie :D"); //podalbym excpetion ale nie xd 
            }
        }
        public static int[] WczytajDanePixeli(string dane) {
            string[] comesLiczbyPixeli = new string[3];
            byte licznikLiczb = 0;
            bool liczenieZacznij = false;
            foreach (char znak in dane) {
                if (znak == ':') {
                    liczenieZacznij = true;
                    continue;
                }
                else if (znak == ',') {
                    liczenieZacznij = false;
                    licznikLiczb++;
                }
                if (liczenieZacznij) comesLiczbyPixeli[licznikLiczb] += znak;
            }
            int[] daneLiczbowe = new int[3];
            for (int zmiennaPomocnicza = 0; zmiennaPomocnicza < 3; zmiennaPomocnicza++) {
                daneLiczbowe[zmiennaPomocnicza] = KonwertujPolskiNaLiczby(comesLiczbyPixeli[zmiennaPomocnicza]);
            }
            return daneLiczbowe;
        }
        public static int KonwertujMałeLiczby(string[] słowa) {
            int comesOutput = 0;
            foreach (string comesLiczba in słowa) {
                comesOutput += konwerter[comesLiczba];
            }
            return comesOutput;
        }
        public static byte KonwertujPixele(string pixelSłowa) {
            if (pixelSłowa == staryPixel) return staraWartosc;
            staryPixel = pixelSłowa;
            byte comesOutput = 0;
            string[] słowa = pixelSłowa.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string comesLiczba in słowa) {
                comesOutput += (byte)konwerter[comesLiczba];
            }
            staraWartosc = comesOutput;
            return comesOutput;
        }
        public static int KonwertujPolskiNaLiczby(string polski) {
            string[] polskiPodzielony = polski.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
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

            if (pozycjaTysiąca > 0) {
                for (int zmiennaPomocnicza = 0; zmiennaPomocnicza < pozycjaTysiąca; zmiennaPomocnicza++) {
                    comesPodciągTysięcy.Add(polskiPodzielony[zmiennaPomocnicza]);
                }
            }
            else comesPodciągTysięcy.Add("jeden");
            int comesWynikKonwersji = 0;
            comesWynikKonwersji += KonwertujMałeLiczby(comesPodciągMniejNiżTysięcy.ToArray()) + KonwertujMałeLiczby(comesPodciągTysięcy.ToArray()) * 1000;
            return comesWynikKonwersji;
        }
    }
}
