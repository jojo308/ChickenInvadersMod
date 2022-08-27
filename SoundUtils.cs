using Terraria.Audio;

namespace ChickenInvadersMod
{
    public static class SoundUtils
    {
        #region NPCs
        public static SoundStyle ChickenHit = new SoundStyle("ChickenInvadersMod/Assets/Sounds/Chicken_Hit", 3) with
        {
            PitchVariance = .3f,
            MaxInstances = 2,
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
        };

        public static SoundStyle ChickenDeath = new SoundStyle("ChickenInvadersMod/Assets/Sounds/Chicken_Death", 2) with
        {
            PitchVariance = .3f,
            MaxInstances = 2,
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
        };

        public static SoundStyle SuperChickenDeath = new SoundStyle("ChickenInvadersMod/Assets/Sounds/SuperChicken_Death") with
        {
            Volume = 1f,
            MaxInstances = 1,
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
        };

        public static SoundStyle ChickDeath = new SoundStyle("ChickenInvadersMod/Assets/Sounds/Chick_Death") with
        {
            PitchVariance = .3f,
            MaxInstances = 2,
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
        };

        public static SoundStyle EggHit = new SoundStyle("ChickenInvadersMod/Assets/Sounds/EggHit") with
        {
            PitchVariance = .3f,
            MaxInstances = 3,
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
        };

        public static SoundStyle EggDeath = new SoundStyle("ChickenInvadersMod/Assets/Sounds/EggDeath") with
        {
            PitchVariance = .3f,
            MaxInstances = 3,
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
        };
        #endregion

        #region Projectile
        public static SoundStyle Laser = new SoundStyle("ChickenInvadersMod/Assets/Sounds/Laser") with
        {
            PitchVariance = 0.3f,
            MaxInstances = 1,
            SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
        };

        public static SoundStyle EggDrop = new SoundStyle("ChickenInvadersMod/Assets/Sounds/Egg_Drop") with
        {
            PitchVariance = 0.3f,
            MaxInstances = 1,
            SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
        };

        public static SoundStyle EggSplash = new SoundStyle("ChickenInvadersMod/Assets/Sounds/Egg_Splash") with
        {
            PitchVariance = 0.3f,
            MaxInstances = 1,
            SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
        };

        public static SoundStyle Neutron = new SoundStyle("ChickenInvadersMod/Assets/Sounds/Neutron") with
        {
            PitchVariance = 0.3f,
            MaxInstances = 1,
            SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
        };
        #endregion    

        #region Other
        // not used
        public static SoundStyle Bite = new SoundStyle("ChickenInvadersMod/Assets/Sounds/Bite") with
        {
            PitchVariance = 0.3f,
            MaxInstances = 1,
            SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
        };
        #endregion
    }
}
