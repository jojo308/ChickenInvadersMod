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
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        /// <summary>
        /// Interval before shooting next projectile
        /// </summary>
        public float Interval
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        /// <summary>
        /// Whether or not the chicken is protected by a barrier
        /// </summary>
        public bool IsProtected
        {
            get => NPC.ai[2] == 1;
            set => NPC.ai[2] = value ? 1 : 0;
        }
    }
}
