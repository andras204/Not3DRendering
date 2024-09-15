using System;

namespace Not3DRendering
{
    internal class Program
    {
        /* My main lagnuage is Rust, that's the reason behind
         * the snake_case names, factory functions, and the
         * occasional pass-by-reference funcion arguments
         */

        static void Main(string[] args)
        {
            Renderer custom_renderer = new Renderer(
                // resolution
                new GameMath.ivec2(Console.WindowWidth - 1, Console.WindowHeight - 3),

                // field of view (in radians)
                (float)Math.PI / 2f,

                // max view distance
                10f,

                // depth resolution
                0.01f
            );

            Game game = Game.default_with_renderer(custom_renderer);

            game.run_loop();
        }
    }
}
