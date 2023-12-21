using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        struct ABC
        {
            
            public int n;
        }
        class EFG
        {
            public int n;
        }
        static void Main(string[] args)
        {
            ABC a = new ABC();
            EFG b = new EFG();
            //List<ABC> ls1 = new List<ABC>();
            //ls1.Add(a);
            //a.n = 456;

            //List<EFG> ls2 = new List<EFG>();
            //ls2.Add(b);
            //b.n = 123;

            ABC[] ar1 = new ABC[1];
            ar1[0] = a;
            ar1[0].n = 999;
            a.n = 789;


            EFG[] ar2 = new EFG[1];
            ar2[0] = b;
            b.n = 789;
            Console.WriteLine("Hello World!");
        }
    }
}
