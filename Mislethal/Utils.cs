using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Chireiden.Mislethal
{
    public static class Utils
    {
        public static Card First(this List<Card> cards, string target)
        {
            return cards.First(c => c.Id == target);
        }

        public static List<Card> Of(this List<Card> cards, string target)
        {
            return cards.Where(c => c.Id == target).ToList();
        }

        public static bool Contains(this List<Card> cards, string target)
        {
            return cards.Any(c => c.Id == target);
        }

        public static int Count(this List<Card> source, string target)
        {
            return source.Count(c => c.Id == target);
        }

        public static int ToMill(this int draws, int remain)
        {
            return draws > remain ? (draws - remain) * (draws - remain + 1) / 2 : 0;
        }

        public static int[] ToMill(this int[] draws, int remain)
        {
            return draws.Select(d => d.ToMill(remain)).ToArray();
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
                EscHaltCheck();
                _consoleState = 5;
            }
            else
            {
                _consoleState = ConsoleMgr.Toggle(_consoleState);
            }
        }

        private static void EscHaltCheck()
        {
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