using System;
using static Not3DRendering.GameMath;

namespace Not3DRendering
{
    internal class Renderer
    {
        const char FULL_BLOCK = '█';
        const char MAXTONE_BLOCK = '▓';
        const char HALFTONE_BLOCK = '▒';
        const char QUARTERTONE_BLOCK = '░';
        const char BLANK = ' ';
        const char FLOOR = '.';

        ivec2 resolution;

        float field_of_view;
        float max_view_distance;
        float ray_step_size;

        public Renderer(ivec2 resolution, float field_of_view, float max_view_distance, float ray_step_size)
        {
            this.resolution = clamp(
                resolution,
                new ivec2(0, 0),
                new ivec2(Console.WindowWidth - 1, Console.WindowHeight - 3)
            );
            this.field_of_view = field_of_view;
            this.max_view_distance = max_view_distance;
            this.ray_step_size = ray_step_size;
        }

        public static Renderer Default()
        {
            return new Renderer(
                new ivec2(Console.WindowWidth - 1, Console.WindowHeight - 3),
                (float)(Math.PI / 2),
                8f,
                0.01f
            );
        }

        public ivec2 get_resolution() { return resolution; }

        public void draw_frame(ref Player player, ref Level level)
        {
            (float[], bool[]) ray_hits = raycast(ref player, ref level);
            draw_distances(ray_hits);
        }

        (float[], bool[]) raycast(ref Player player, ref Level level)
        {
            float fov_step = field_of_view / (float)resolution.x;
            float fov_start = (player.direction - (field_of_view / 2f)) + (fov_step / 2f);

            float[] dist = new float[resolution.x];
            bool[] corner = new bool[resolution.x];

            for (int x = 0; x < resolution.x; x++)
            {
                vec2 ray_dir = vec2.from_angle(fov_start + ((float)x * fov_step));

                dist[x] = max_view_distance;

                for (float d = 0f; d < max_view_distance; d += ray_step_size)
                {
                    vec2 ray_pos = player.position + (d * ray_dir);
                    ivec2 test_index = ray_pos.into_ivec2();
                    if (level.map[test_index.y][test_index.x] == '#')
                    {
                        dist[x] = d;
                        // corner detection
                        float fx = fract(ray_pos.x);
                        float fy = fract(ray_pos.y);
                        corner[x] = (fx > 0.95 || fx < 0.05) && (fy > 0.95 || fy < 0.05);
                        break;
                    }
                }
            }

            return (dist, corner);
        }

        void draw_distances((float[], bool[]) ray_hits)
        {
            char[,] out_buffer = new char[resolution.x, resolution.y];

            // construct blank output buffer
            int ft = (int)(0.5f * (float)resolution.y);
            for (int x = 0; x < resolution.x; x++)
            {
                for (int y = 0; y < resolution.y; y++)
                {
                    if (y > ft) { out_buffer[x, y] = FLOOR; }
                    else { out_buffer[x, y] = BLANK; }
                }
            }

            float[] distances = ray_hits.Item1;
            bool[] corners = ray_hits.Item2;

            normalize_distances(ref distances);

            for (int x = 0; x < resolution.x; x++)
            {
                if (distances[x] >= 1f) { continue; }

                // fade walls based on distance
                char strip_char = QUARTERTONE_BLOCK;
                if (distances[x] <= 0.66f) { strip_char = HALFTONE_BLOCK; }
                if (distances[x] <= 0.35f) { strip_char = MAXTONE_BLOCK; }
                if (distances[x] <= 0.15f) { strip_char = FULL_BLOCK; }
                // color corners differently to add some texture to walls
                if (corners[x]) { strip_char = BLANK; }

                int strip_length = (int)((float)resolution.y * (1f - distances[x]));
                int strip_start = (resolution.y - strip_length) / 2;
                int strip_end = resolution.y - strip_start;
                for (int y = 0; y < resolution.y; y++)
                {
                    if (y >= strip_start && y <= strip_end) { out_buffer[x, y] = strip_char; }
                }
            }

            // collect the char array into a single string for faster printing
            string final_output = string.Empty;

            for (int y = 0; y < resolution.y; y++)
            {
                for (int x = 0; x < resolution.x; x++)
                {
                    final_output += out_buffer[x, y];
                }
                final_output += '\n';
            }

            Console.SetCursorPosition(0, 0);
            Console.WriteLine(final_output);
        }

        void normalize_distances(ref float[] distances)
        {
            for (int i = 0; i < distances.Length; i++) 
            {
                distances[i] = distances[i] / max_view_distance;
            }
        }
    }
}
