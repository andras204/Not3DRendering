using System;
using static Not3DRendering.GameMath;

namespace Not3DRendering
{
    internal class Game
    {
        Renderer renderer;
        Player player;
        Level level;

        float player_turn_rate;
        float player_move_speed;

        bool running;

        public Game(Renderer renderer, Player player, Level level, float move_speed, float turn_rate)
        {
            this.renderer = renderer;
            this.player = player;
            this.player.position = level.spawn_point;
            this.level = level;
            this.player_move_speed = move_speed;
            this.player_turn_rate = turn_rate;
            this.running = false;
        }

        public static Game Default()
        {
            return new Game(
                Renderer.Default(),
                new Player(),
                Level.level_1(),
                0.1f,
                0.075f
            );
        }

        public static Game default_with_renderer(Renderer renderer)
        {
            return new Game(
                renderer,
                new Player(),
                Level.level_1(),
                0.1f,
                0.075f
            );
        }

        public void run_loop()
        {
            Console.SetCursorPosition(0, renderer.get_resolution().y + 1);
            Console.WriteLine("[W] move forward | [S] move backwards | [A] turn left | [D] turn right | [ESC] exit");
            Console.CursorVisible = false;
            running = true;

            // draw 1st frame before blocking on input
            renderer.draw_frame(ref player, ref level);

            while (true)
            {
                resolve_input();

                // don't finish processing the frame on exit
                if (!running)
                {
                    Console.CursorVisible = true;
                    return;
                }

                renderer.draw_frame(ref player, ref level);
            }
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
            if (level.map[mt.y][mt.x] != '#')
            {
                player.position += move;
                return;
            }

            // handle x and y separately to allow sliding along walls
            vec2 mx = new vec2(move.x, 0f);
            vec2 my = new vec2(0f, move.y);

            ivec2 tx = (pp + mx).into_ivec2();
            if (level.map[tx.y][tx.x] != '#') { player.position += mx; }
            ivec2 ty = (pp + my).into_ivec2();
            if (level.map[ty.y][ty.x] != '#') { player.position += my; }
        }
    }
}
