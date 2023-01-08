using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Localization;

using Microsoftenator.Wotr.Common;
using Microsoftenator.Wotr.Common.Blueprints;
using Microsoftenator.Wotr.Common.Localization;
using Microsoftenator.Wotr.Common.Util;

using UnityModManagerNet;

using static Kingmaker.Blueprints.JsonSystem.BlueprintsCache;

namespace PrimalistBloodlineSelections
{
    static class Main
    {
        internal class Logger
        {
            private readonly UnityModManager.ModEntry.ModLogger logger;

            internal Logger(UnityModManager.ModEntry.ModLogger logger) => this.logger = logger;

            public Action<string> Debug =>
#if DEBUG
                s => logger.Log($"[DEBUG] {s}");
#else
                Functional.Ignore;
#endif
            public Action<string> Info => logger.Log;
            public Action<string> Warning => logger.Warning;
            public Action<string> Error => logger.Error;
            public Action<string> Critical => logger.Critical;
        }

        internal static UnityModManager.ModEntry? ModEntry { get; private set; }

        internal static readonly int SharedModVersion = 0;
        internal static Func<IEnumerable<BlueprintInfo>, bool> AddSharedBlueprints { get; private set; } = _ => false;

        static internal Logger? Log { get; private set; }
        internal static bool Enabled { get; private set; } = false;

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Log?.Debug($"{nameof(Main)}.{nameof(OnToggle)}({value})");

            Enabled = value;
            return true;
        }

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            Log = new(modEntry.Logger);

            Log.Debug($"{nameof(Main)}.{nameof(Load)}");

            ModEntry = modEntry;

            var harmony = new Harmony(modEntry.Info.Id);

            SharedMods.Register(modEntry.Info.Id, SharedModVersion);
            AddSharedBlueprints = blueprints => SharedMods.AddBlueprints(modEntry.Info.Id, SharedModVersion, blueprints);

            harmony.PatchAll();

            return true;
        }
    }

    internal static class Localization
    {
        private static readonly Lazy<LocalizedStringsPack> defaultStringsLazy = new(() => new(LocalizationManager.CurrentLocale));
        public static LocalizedStringsPack Default => defaultStringsLazy.Value;
    }

    internal static class Patches
    {
        [HarmonyPatch(typeof(BlueprintsCache), nameof(BlueprintsCache.Init))]
        [HarmonyAfter("TabletopTweaks-Core", "TabletopTweaks-Base")]
        internal class BlueprintsCache_Init_Patch
        {
            //static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            //{
            //    var addMethod = ResourcesLibrary.BlueprintsCache.m_LoadedBlueprints.GetType().GetMethod("Add");
            //    var setMethod = ResourcesLibrary.BlueprintsCache.m_LoadedBlueprints.GetType().GetMethod("set_Item");

            //    return instructions.Select(i =>
            //    {
            //        if (i.opcode == OpCodes.Callvirt && i.Calls(addMethod))
            //            i.operand = setMethod;
            //        return i;
            //    });
            //}

            private static bool patched;

            static void Postfix()
            {
                if (patched)
                {
                    Main.Log?.Warning($"Duplicate call to {nameof(BlueprintsCache_Init_Patch)}.{nameof(Postfix)}");
                    return;
                }

                patched = true;

                Main.Log?.Debug($"{nameof(BlueprintsCache_Init_Patch)}.{nameof(Postfix)}");

                PrimalistBloodlineFixes.PatchPrimalistProgression();

                Localization.Default.LoadAll();
            }
        }
    }
}
