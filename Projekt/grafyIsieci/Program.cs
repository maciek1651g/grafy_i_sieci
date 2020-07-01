using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace grafyIsieci
{
    class Program
    {
        static void Main(string[] args)
        {
            int w = 0;

            int m = 0, p = 0, n = 0;
            int[,] C = null;

            do
            {
                Console.WriteLine("1 - Wpisz graf ręcznie w konsoli");
                Console.WriteLine("2 - Wczytaj plik z grafem");
                Console.WriteLine("3 - Zakończ działanie programu");
                Console.Write("Wybór opcji: ");
                int.TryParse(Console.ReadLine(), out w);

                bool f;
                Console.WriteLine();
                switch (w)
                {
                    case 1:
                        f = WczytajDane(ref m, ref p, ref n, ref C);
                        if (f)
                        {
                            AlgorytmEdmondsaKarpa(m, p, n, C);
                        }
                        else
                        {
                            Console.WriteLine("Nie udało się poprawnie wpisać danych.");
                        }
                        break;
                    case 2:
                        f = WczytajPlik(ref m, ref p, ref n, ref C);
                        if (f)
                        {
                            AlgorytmEdmondsaKarpa(m, p, n, C);
                        }
                        else
                        {
                            Console.WriteLine("Nie udało się odczytać danych.");
                        }
                        break;
                }
                Console.WriteLine();
            } while (w != 3);

            Console.WriteLine("Autorzy: ");
            Console.WriteLine("Dominik Derewońko");
            Console.WriteLine("Maciej Dominiak");
            Console.WriteLine("Radosław Nagiel");
            Console.Write("Kliknij dowolny przycisk aby zakończyć...");
            Console.ReadKey();
        }

        public static void AlgorytmEdmondsaKarpa(int m, int p, int n, int[,] C)
        {
            int[,] F = new int[n + 2, n + 2];
            int fmax = 0;
            Queue<int> Q = new Queue<int>();

            int[] P = new int[n + 2];
            int[] CFP = new int[n + 2];
            while (true)
            {
                for (int i = 0; i <= n + 1; i++) P[i] = -1;

                P[n] = -2;
                CFP[n] = 9999999;

                Q.Clear();
                Q.Enqueue(n);

                bool esc = false;

                while (Q.Count != 0)
                {
                    int v = Q.Dequeue();


                    for (int u = 0; u <= n + 1; u++)
                    {
                        int cp = C[v, u] - F[v, u];
                        if ((cp != 0) && (P[u] == -1))
                        {
                            P[u] = v;
                            if (CFP[v] > cp) CFP[u] = cp; else CFP[u] = CFP[v];
                            if (u == n + 1)
                            {
                                fmax += CFP[n + 1];
                                int i = u;
                                while (i != n)
                                {
                                    v = P[i];
                                    F[v, i] += CFP[n + 1];
                                    F[i, v] -= CFP[n + 1];
                                    i = v;
                                }
                                esc = true; break;
                            }
                            Q.Enqueue(u);
                        }
                    }
                    if (esc) break;
                }
                if (!esc) break;
            }

            Console.WriteLine();
            Console.WriteLine("Wynik:");

            int licznik = 0;
            if (fmax > 0)
            {
                for (int v = m; v < n; v++)
                {
                    bool znaleziono = false;
                    for (int u = 0; u < n; u++)
                        if ((C[v, u] == 1) && (F[v, u] == 1))
                        {
                            int nrM = u + 1;
                            int nrP = v - m + 1;
                            Console.WriteLine("Pracownik nr {0} - maszyna {1}", nrP, nrM);
                            znaleziono = true;
                            licznik++;
                            break;
                        }
                    if (znaleziono == false)
                    {
                        int nrP = v - m + 1;
                        
                        Console.WriteLine("Pracownik nr {0} - brak maszyny", nrP);
                    }
                }
            }
            Console.WriteLine("Liczba wszystkich maszyn: "+m);
            Console.WriteLine("Liczba przypisanych maszyn: "+licznik);
            Console.WriteLine("Liczba nieprzypisanych maszyn: "+(m-licznik));
            Console.WriteLine();
            Console.Write("Kliknij dowolny przycisk aby kontynuować...");
            Console.ReadKey();
            Console.WriteLine();
        }

        public static bool WczytajDane(ref int m, ref int p, ref int n, ref int[,] C)
        {
            Console.Write("Podaj ilość maszyn: ");
            if (!int.TryParse(Console.ReadLine(), out m)) return false;
            Console.Write("Podaj ilość pracowników: ");
            if (!int.TryParse(Console.ReadLine(), out p)) return false;
            n = p + m;
            C = new int[n + 2, n + 2];

            for (int i = 0; i < m; i++)
            {
                C[i, n + 1] = 1;
            }
            for (int i = 0; i < p; i++)
            {
                Console.WriteLine("Podaj numery maszyn, które może obsługiwać pracownik nr {0} (Oddzielone spacją, zatwierdź enterem)", i + 1);
                string[] wejscie = Console.ReadLine().Split(' ');
                int tmp;
                for (int j = 0; j < wejscie.Length; j++)
                {
                    if (!int.TryParse(wejscie[j], out tmp) || tmp > m || tmp <= 0)
                        continue;
                    C[m + i, tmp - 1] = 1;
                }
                C[m + i, n] = 1;
            }
            for (int i = 0; i < p; i++)
            {
                C[n, i + m] = 1;
            }

            return true;
        }

        public static bool WczytajPlik(ref int m, ref int p, ref int n, ref int[,] C)
        {
            string nazwa = "";
            Console.Write("Wpisz nazwę pliku: ");
            nazwa = Console.ReadLine();
            StreamReader sr = null;

            try
            {
                sr = new StreamReader(nazwa + ".txt");
            }
            catch (Exception)
            {
                Console.WriteLine("Nie znaleziono pliku.");
                return false;
            }


            if (sr != null)
            {
                if (!int.TryParse(sr.ReadLine(), out m)) return false;
                if (!int.TryParse(sr.ReadLine(), out p)) return false;
                n = p + m;
                C = new int[n + 2, n + 2];

                for (int i = 0; i < m; i++)
                {
                    C[i, n + 1] = 1;
                }
                for (int i = 0; i < p && !sr.EndOfStream; i++)
                {
                    string[] wejscie = sr.ReadLine().Split(' ');
                    int tmp;
                    for (int j = 0; j < wejscie.Length; j++)
                    {
                        if (!int.TryParse(wejscie[j], out tmp) || tmp > m || tmp <= 0)
                            continue;
                        C[m + i, tmp - 1] = 1;
                    }
                    C[m + i, n] = 1;
                }
                for (int i = 0; i < p; i++)
                {
                    C[n, i + m] = 1;
                }
            }
            sr.Close();
            return true;
        }

        public static void WyswietlTablice(int[,] G)
        {
            for (int i = 0; i < G.GetLength(0); i++)
            {
                for (int j = 0; j < G.GetLength(1); j++)
                {
                    Console.Write(G[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public static void WyswietlTablice(int[] G)
        {
            for (int i = 0; i < G.GetLength(0); i++)
            {
                Console.Write(G[i] + " ");
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
