using AlisaieMod.Survivors.Alisaie.SkillStates;

namespace AlisaieMod.Survivors.Alisaie
{
    public static class AlisaieStates
    {
        public static void Init()
        {
            //primary
            Modules.Content.AddEntityState(typeof(Jolt));

            //secondary
            Modules.Content.AddEntityState(typeof(Acceleration));

            //dualcast   
            Modules.Content.AddEntityState(typeof(Verthunder));
            Modules.Content.AddEntityState(typeof(Veraero));

            //utility
            Modules.Content.AddEntityState(typeof(CorpsACorps));

            //Special
            Modules.Content.AddEntityState(typeof(RapierComboIdle));
            Modules.Content.AddEntityState(typeof(SwordComboStep));
            Modules.Content.AddEntityState(typeof(Riposte));
            Modules.Content.AddEntityState(typeof(Zwerchhau));
            Modules.Content.AddEntityState(typeof(Redoublement));
            Modules.Content.AddEntityState(typeof(EnchantedRiposte));
            Modules.Content.AddEntityState(typeof(EnchantedZwerchhau));
            Modules.Content.AddEntityState(typeof(EnchantedRedoublement));

            //finishers
            Modules.Content.AddEntityState(typeof(Verholy));
            Modules.Content.AddEntityState(typeof(Verflare));
        }
    }
}
