using Terraria.ModLoader;

namespace ChickenInvadersMod.NPCs
{
    abstract public class BaseChicken : ModNPC
    {
        /// <summary>
        /// Time left before shooting a projectile
        /// </summary>
        public float TimeLeft
        {
            get => npc.ai[0];
            set => npc.ai[0] = value;
        }
        /// <summary>
        /// Interval before shooting next projectile
        /// </summary>
        public float Interval
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }
        /// <summary>
        /// Whether or not the chicken is protected by a barrier
        /// </summary>
        public bool IsProtected
        {
            get => npc.ai[2] == 1;
            set => npc.ai[2] = value ? 1 : 0;
        }
    }
}
