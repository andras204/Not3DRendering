using System;
using static Not3DRendering.GameMath;

namespace Not3DRendering
{
    internal class Renderer
    {
        const char FULL_BLOCK = '█';
        const char THREETONE_BLOCK = '▓';
        const char HALFTONE_BLOCK = '▒';
        const char QUARTERTONE_BLOCK = '░';
        const char BLANK = ' ';
        const char FLOOR = '.';

        const string EXIT_TEXTURE = " EXIT ";

        class RaycastHit
        {
            public float distance;
            public char surface;
            public bool corner;

            public RaycastHit(float distance, char surface, bool corner)
            {
                this.distance = distance;
                this.surface = surface;
                this.corner = corner;
            }
        }

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

        public void draw_frame(ref Player player, ref string[] map)
        {
            RaycastHit[] ray_hits = raycast(ref player, ref map);
            draw_hits(ray_hits);
        }

        RaycastHit[] raycast(ref Player player, ref string[] map)
        {
            float fov_step = field_of_view / (float)resolution.x;
            float fov_start = (player.direction - (field_of_view / 2f)) + (fov_step / 2f);

            RaycastHit[] hits = new RaycastHit[resolution.x];

            for (int x = 0; x < resolution.x; x++)
            {
                vec2 ray_dir = vec2.from_angle(fov_start + ((float)x * fov_step));

                for (float d = 0f; d < max_view_distance; d += ray_step_size)
                {
                    vec2 ray_pos = player.position + (d * ray_dir);
                    ivec2 test_index = ray_pos.into_ivec2();
                    if (map[test_index.y][test_index.x] != '.')
                    {
                        // corner detection
                        float fx = fract(ray_pos.x);
                        float fy = fract(ray_pos.y);
                        hits[x] = new RaycastHit(d / max_view_distance, map[test_index.y][test_index.x], (fx > 0.95 || fx < 0.05) && (fy > 0.95 || fy < 0.05));
                        break;
                    }
                }
            }

            return hits;
        }

        void draw_hits(RaycastHit[] ray_hits)
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

            for (int x = 0; x < resolution.x; x++)
            {
                if (ray_hits[x] is null) { continue; }
                if (ray_hits[x].distance >= 1f) { continue; }

                char strip_char = get_wall_texture(ray_hits[x]);
                
                int texture_scroll = x % EXIT_TEXTURE.Length;

                int strip_length = (int)((float)resolution.y * (1f - ray_hits[x].distance));
                int strip_start = (resolution.y - strip_length) / 2;
                int strip_end = resolution.y - strip_start;
                for (int y = 0; y < resolution.y; y++)
                {
                    if (y >= strip_start && y <= strip_end)
                    {
                        if (ray_hits[x].surface == 'E')
                        {
                            out_buffer[x, y] = EXIT_TEXTURE[texture_scroll];
                            texture_scroll = (texture_scroll + 1) % EXIT_TEXTURE.Length;
                            continue;
                        }
                        out_buffer[x, y] = strip_char;
                    }
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

        char get_wall_texture(RaycastHit hit)
        {
            // color corners differently to add some texture to walls
            if (hit.corner) { return BLANK; }
            // fade walls based on distance
            if (hit.distance > 0.66f) { return QUARTERTONE_BLOCK; }
            if (hit.distance > 0.35f) { return HALFTONE_BLOCK; }
            if (hit.distance > 0.15f) { return THREETONE_BLOCK; }
            return FULL_BLOCK;
        }
    }
}
