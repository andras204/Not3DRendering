using static Not3DRendering.GameMath;

namespace Not3DRendering
{
    internal class Player
    {
        public vec2 position;
        public float direction;

        public Player() 
        {
            this.position = new vec2(0, 0);
            this.direction = 0;
        }
    }
}
