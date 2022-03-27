using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DEngine
{
    public static class Log
    {
        public static void Default(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[LOG] - {msg}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[LOG] - {msg}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Warning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[LOG] - {msg}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[LOG] - {msg}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
