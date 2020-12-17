using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleAppFlaskeAutomaten
{
    class Program
    {
        static object baton = new object();
        static Queue<string> products = new Queue<string>();
        static Queue<string> Beer = new Queue<string>();
        static Queue<string> soda = new Queue<string>();
        static Random random = new Random();

        static void Main(string[] args)
        {
            Thread threadSetProdukt = new Thread(SetProduckt);
            Thread threadSortProdukt = new Thread(SortProduckt);
            Thread threadGetProdukt = new Thread(GetProduckt);

            threadSetProdukt.Name = "Producer";
            threadSortProdukt.Name = "Sorting Consumer";
            threadGetProdukt.Name = "Getting Cunsumer";

            threadSetProdukt.Start();
            Thread.Sleep(500);
            threadSortProdukt.Start();
            Thread.Sleep(500);
            threadGetProdukt.Start();

            Console.ReadKey();
        }

        static void SortProduckt()
        {
            while (true)
            {
                Monitor.Enter(baton);
                try
                {
                    while (products.Count == 0)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] is waiting for the baton...");
                        Monitor.Wait(baton);
                    }

                    Console.WriteLine($"[{Thread.CurrentThread.Name}] gat the baton");
                    while (products.Count != 0)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] Soting stuff from queue");
                        if (products.Peek() == "Beer")
                        {
                            Beer.Enqueue(products.Dequeue());
                        }
                        else if (products.Peek() == "Soda")
                        {
                            soda.Enqueue(products.Dequeue());
                        }
                        Thread.Sleep(random.Next(1000));
                    }
                }
                finally
                {
                    Monitor.Exit(baton);
                }
            }
        }

        static void SetProduckt()
        {
            while (true)
            {
                Monitor.Enter(baton);
                try
                {
                    Console.WriteLine($"[{Thread.CurrentThread.Name}] is waiting ...");
                    Thread.Sleep(random.Next(1000));

                    while (products.Count != 0)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] is waiting for the baton...");
                        Monitor.Wait(baton);
                    }

                    while (products.Count == 0)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] putting stuff in queue");
                        for (int i = 0; i < 5; i++)
                        {
                            switch (random.Next(2))
                            {
                                case 0:
                                    products.Enqueue("Soda");
                                    break;
                                    case 1:
                                    products.Enqueue("Beer");
                                    break;
                                default:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Error Messages: Switch output");
                                    Console.ResetColor();
                                    break;

                            }
                            Thread.Sleep(random.Next(1000));
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(baton);
                }
            }
        }

        static void GetProduckt()
        {
            while (true)
            {
                Monitor.Enter(baton);
                try
                {
                    while (Beer.Count == 0 && soda.Count == 0)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] is waiting for the baton...");
                        Monitor.Wait(baton);
                    }

                    Console.WriteLine($"[{Thread.CurrentThread.Name}] gat the baton");
                    while (Beer.Count != 0 || soda.Count != 0)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] Printing stuff in queue");
                        if (Beer.Count != 0)
                        {
                            Console.WriteLine("Beer dequeue : " + Beer.Dequeue());
                        }
                        else if (soda.Count != 0)
                        {
                            Console.WriteLine("Soda dequeue : " + soda.Dequeue());
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error Message : Print from Soda and Beer");
                            Console.ResetColor();
                        }
                        Thread.Sleep(500);

                    }
                }
                finally
                {
                    Monitor.Exit(baton);
                }
            }
        }
    }
}
