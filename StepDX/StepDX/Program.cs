﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace StepDX
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Game game = new Game();
            GameSounds snd = new GameSounds(game);
            game.setSounds(snd);
            game.Show();
            do
            {
                game.Advance();
                game.Render();
                Application.DoEvents();
            } while (game.Created);
        }
    }
}
