using static Not3DRendering.GameMath;

namespace Not3DRendering
{
    internal class Level
    {
        public string[] map;
        public vec2 spawn_point;

        public Level(string[] map, vec2 spawn_point)
        {
            this.map = map;
            this.spawn_point = spawn_point;
        }

        public static Level test_box() {
            string[] map = new string[] {
                "########",
                "#......#",
                "#......#",
                "#......#",
                "#......#",
                "#......#",
                "#......#",
                "########",
            };

            vec2 sp = new vec2(1.5f, 1.5f);

            return new Level(map, sp);
        }

        public static Level level_1() {
            string[] map = new string[] {
                "##############",
                "#....#.......#",
                "#...........##",
                "#....#.......#",
                "######......##",
                "#............#",
                "#............#",
                "##.....#######",
                "#......#.....#",
                "##.....#.....#",
                "#.........#..#",
                "##.....#.....#",
                "#......#.....#",
                "##############",
            };

            vec2 sp = new vec2(1.5f, 2.5f);

            return new Level(map, sp);
        }
    }
}
