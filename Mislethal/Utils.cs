using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using Hearthstone_Deck_Tracker;
using System.Collections.Generic;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;

namespace Chireiden.Mislethal
{
    public static class Utils
    {
        public static List<Entity> FriendlyMinions => Core.Game.Player.Board.Where(e => !e.IsHero).ToList();
        public static List<Entity> EnemyMinions => Core.Game.Opponent.Board.Where(e => !e.IsHero).ToList();

        public static int ToMill(this int draws, int remain)
        {
            return draws > remain ? (draws - remain) * (draws - remain + 1) / 2 : 0;
        }

        public static int[] ToMill(this int[] draws, int remain)
        {
            return draws.Select(d => d.ToMill(remain)).ToArray();
        }

        public static int Count(this string[] source, string target)
        {
            return source.Count(c => c == target);
        }

        [Conditional("DEBUG")]
        public static void DebugRun(Action action)
        {
            action.Invoke();
        }

        [Conditional("DEBUG")]
        public static void Write(string line)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Log(1, "", line + "\r\n");
            }
        }

        private static int _consoleState = -1;

        public static void ToggleConsole()
        {
            if (_consoleState == -1)
            {
                ConsoleMgr.InitConsoleHandles();
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    _consoleState = ConsoleMgr.Toggle(_consoleState);
                };
                Console.WriteLine("Console Init.");
                EscHaltCheck();
                _consoleState = 5;
            }
            else
            {
                _consoleState = ConsoleMgr.Toggle(_consoleState);
            }
        }

        [Conditional("DEBUG")]
        private static void EscHaltCheck()
        {
            Console.WriteLine("[DEBUG] Enter q to quit (the HDT).");
            new System.Threading.Thread(() =>
            {
                while (true)
                {
                    var item = Console.ReadLine();
                    if (item.StartsWith("q", StringComparison.OrdinalIgnoreCase))
                    {
                        Environment.Exit(0);
                    }
                }
            }).Start();
        }

        private static class ConsoleMgr
        {
            [DllImport("user32.dll")]
            public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

            [DllImport("user32.dll")]
            private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

            [DllImport("kernel32.dll", ExactSpelling = true)]
            private static extern IntPtr GetConsoleWindow();

            [DllImport("kernel32")]
            private static extern bool AllocConsole();

            [DllImport("user32.dll")]
            private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            public static void InitConsoleHandles()
            {
                AllocConsole();

                // Delete X from menu.
                DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), 0xF060, 0x00000000);
            }

            public static int Toggle(int current)
            {
                // Show 5 / Hide 0
                if (current == 0)
                {
                    ShowWindow(GetConsoleWindow(), 5);
                    return 5;
                }
                else
                {
                    ShowWindow(GetConsoleWindow(), 0);
                    return 0;
                }
            }
        }
    }
}