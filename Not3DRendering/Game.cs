using System;
using static Not3DRendering.GameMath;

namespace Not3DRendering
{
    internal class Game
    {
        Renderer renderer;
        Player player;
        Level[] levels;

        int current_level;
        string[] map;

        float player_turn_rate;
        float player_move_speed;

        bool running;
        bool advance_level;

        public Game(Renderer renderer, Player player, Level[] levels, float move_speed, float turn_rate)
        {
            this.renderer = renderer;
            this.player = player;
            this.levels = levels;
            this.current_level = 0;
            this.map = levels[0].map;
            this.player.position = levels[0].spawn_point;
            this.player_move_speed = move_speed;
            this.player_turn_rate = turn_rate;
            this.running = false;
            this.advance_level = false;
        }

        public static Game Default()
        {
            return new Game(
                Renderer.Default(),
                new Player(),
                new Level[] { Level.level_1(), Level.level_2(), Level.level_3() },
                0.1f,
                0.075f
            );
        }

        public static Game default_with_renderer(Renderer renderer)
        {
            return new Game(
                renderer,
                new Player(),
                new Level[] { Level.level_1(), Level.level_2(), Level.level_3() },
                0.1f,
                0.075f
            );
        }

        public void start()
        {
            Console.CursorVisible = false;
            running = true;

            start_instructions();

            // draw 1st frame before blocking on input
            renderer.draw_frame(ref player, ref map);

            while (true)
            {
                resolve_input();

                if (advance_level)
                {
                    load_next_level();
                }

                // don't finish processing the frame on exit
                if (!running)
                {
                    Console.CursorVisible = true;
                    return;
                }

                renderer.draw_frame(ref player, ref map);
            }
        }

        void load_next_level()
        {
            current_level += 1;

            if (current_level == levels.Length)
            {
                running = false;
                game_over();
                return;
            }

            map = levels[current_level].map;
            player.position = levels[current_level].spawn_point;
            player.direction = 0f;

            level_transition();

            advance_level = false;
        }

        void resolve_input()
        {
            ConsoleKey key = Console.ReadKey(true).Key;

            // exit on ESC
            if (key == ConsoleKey.Escape)
            {
                running = false;
                return;
            }

            switch (key)
            {
                case ConsoleKey.W:
                    move_and_collide(player_move_speed * vec2.from_angle(player.direction));
                    break;
                case ConsoleKey.S:
                    move_and_collide(-player_move_speed * vec2.from_angle(player.direction));
                    break;
                case ConsoleKey.A:
                    player.direction -= player_turn_rate;
                    break;
                case ConsoleKey.D:
                    player.direction += player_turn_rate;
                    break;
            }
        }

        void move_and_collide(vec2 move)
        {
            vec2 pp = player.position;
            ivec2 mt = (pp + move).into_ivec2();
            if (map[mt.y][mt.x] != '#')
            {
                player.position += move;

                // check for exit
                if (map[mt.y][mt.x] == 'E') { advance_level = true; }

                return;
            }

            // handle x and y separately to allow sliding along walls
            vec2 mx = new vec2(move.x, 0f);
            vec2 my = new vec2(0f, move.y);

            ivec2 tx = (pp + mx).into_ivec2();
            if (map[tx.y][tx.x] != '#') { player.position += mx; }
            ivec2 ty = (pp + my).into_ivec2();
            if (map[ty.y][ty.x] != '#') { player.position += my; }

            // check for exit
            ivec2 pt = player.position.into_ivec2();
            if (map[pt.y][pt.x] == 'E') { advance_level = true; }
        }

        void start_instructions()
        {
            draw_boxed("CONTROLS");
            invert_colors();
            draw_controls();
            invert_colors();
            prompt_continue(0);

            draw_boxed("FIND THE EXIT");
            draw_controls();
            prompt_continue(0);
        }

        void level_transition()
        {

            draw_boxed($"LEVEL {current_level} COMPLETE");
            draw_controls();
            prompt_continue(1000);
        }

        void game_over()
        {

            draw_boxed("YOU WIN");
            prompt_continue(1000);
        }

        void prompt_continue(int delay)
        {
            write_centered(".   ", (renderer.get_resolution().y / 2) + 3);
            System.Threading.Thread.Sleep(delay / 4);

            write_centered("..  ", (renderer.get_resolution().y / 2) + 3);
            System.Threading.Thread.Sleep(delay / 4);

            write_centered("... ", (renderer.get_resolution().y / 2) + 3);
            System.Threading.Thread.Sleep(delay / 4);

            write_centered("....", (renderer.get_resolution().y / 2) + 3);
            System.Threading.Thread.Sleep(delay / 4);

            write_centered("Press any key", (renderer.get_resolution().y / 2) + 3);
            Console.ReadKey();
        }

        void draw_boxed(string message)
        {
            Console.Clear();
            ivec2 res = renderer.get_resolution();
            Console.SetCursorPosition((res.x / 2) - ((message.Length / 2) + 2), (res.y / 2) - 1);
            invert_colors();
            Console.Write("".PadLeft(message.Length + 4));
            Console.SetCursorPosition((res.x / 2) - ((message.Length / 2) + 2), res.y / 2);
            Console.Write("  " + message + "  ");
            Console.SetCursorPosition((res.x / 2) - ((message.Length / 2) + 2), (res.y / 2) + 1);
            Console.Write("".PadLeft(message.Length + 4));
            invert_colors();
        }

        void write_centered(string message, int y)
        {
            ivec2 res = renderer.get_resolution();
            Console.SetCursorPosition((res.x / 2) - (message.Length / 2), y);
            Console.Write(message);
        }

        void draw_controls()
        {
            Console.SetCursorPosition(0, renderer.get_resolution().y + 1);
            Console.WriteLine("[W] move forward | [S] move backwards | [A] turn left | [D] turn right | [ESC] exit");
        }

        void invert_colors()
        {
            ConsoleColor fg = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;
            Console.BackgroundColor = fg;
        }
    }
}
