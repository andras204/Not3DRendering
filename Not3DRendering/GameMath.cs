using System;

namespace Not3DRendering
{
    internal class GameMath
    {
        public class vec2
        {
            public float x;
            public float y;

            public vec2(float x, float y)
            {
                this.x = x;
                this.y = y;
            }


            // factory functions --------------------------------
            public static vec2 from_ivec2(ivec2 v) { return new vec2(v.x, v.y); }
            public static vec2 from_angle(float angle)
            {
                return new vec2((float)Math.Cos(angle), (float)Math.Sin(angle));
            }


            // methods ------------------------------------
            public ivec2 into_ivec2() { return new ivec2((int)x, (int)y); }
            public float length() {  return (float)Math.Sqrt((x * x) + (y * y)); }
            public float length_squared() { return (x * x) + (y * y); }
            public override string ToString() { return $"({x}, {y})";  }


            // operators --------------------------------
            public static vec2 operator +(vec2 a, vec2 b)
                => new vec2(a.x + b.x, a.y + b.y);

            public static vec2 operator -(vec2 a, vec2 b)
                => new vec2(a.x - b.x, a.y - b.y);

            public static vec2 operator *(int a, vec2 b)
                => new vec2(a * b.x, a * b.y);

            public static vec2 operator *(float a, vec2 b)
                => new vec2(a * b.x, a * b.y);
        }

        public class ivec2
        {
            public int x;
            public int y;

            public ivec2(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static ivec2 from_vec2(vec2 v) { return new ivec2((int)v.x, (int)v.y); }

            public vec2 into_vec2() { return new vec2(x, y); }

            public override string ToString() { return $"({x}, {y})"; }
        }

        public static int clamp(int i, int min, int max)
        {
            if (i < min) { return min; }
            if (i > max) { return max; }
            return i;
        }

        public static ivec2 clamp(ivec2 i, ivec2 min, ivec2 max)
        {
            return new ivec2(clamp(i.x, min.x, max.x), clamp(i.y, min.y, max.y));
        }

        public static float fract(float f) { return Math.Abs(f) % 1f; }
    }
}
