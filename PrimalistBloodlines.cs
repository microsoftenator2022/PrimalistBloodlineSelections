using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;

using MoreLinq;

using Microsoftenator.Wotr.Common;
using Microsoftenator.Wotr.Common.Blueprints;
using Microsoftenator.Wotr.Common.Blueprints.Extensions;
using Microsoftenator.Wotr.Common.Util;
using PrimalistBloodlineSelections;

namespace PrimalistBloodlineSelections
{
    public static partial class OwlcatBlueprints
    {
        public static partial class Guids
        {
            public const string BloodragerBloodlineSelection = "62b33ac8ceb18dd47ad4c8f06849bc01";
            public const string PrimalistProgression = "d8b8d1dd83393484cbacf6c8830080ae";
            public const string PrimalistArchetype = "4d588290ed0736e4f8cd3f4c3beacd69";

            public const string PrimalistLevel4Selection = "e6228dd80521e1149abc6257a8279b75";
            public const string PrimalistLevel8Selection = "1c7cc7d948c64e549a45d2524c61de35";
            public const string PrimalistLevel12Selection = "43b684433ff1e7d439b87099f1717154";
            public const string PrimalistLevel16Selection = "63c42e0d1a84f004c8e4290bb26f359a";
            public const string PrimalistLevel20Selection = "8cfab12649049e44aaa49691a8a16d88";

            public const string PrimalistTakeRagePowers4 = "8eb5c34bb8471a0438e7eb3994de3b92";
            public const string PrimalistTakeRagePowers8 = "db2710cd915bbcf4193fa54083e56b27";
            public const string PrimalistTakeRagePowers12 = "e43a7bfd5c90a514cab1c11b41c550b1";
            public const string PrimalistTakeRagePowers16 = "b6412ff44f3a82f499d0dd6748a123bc";
            public const string PrimalistTakeRagePowers20 = "5905a80d5934248439e83612d9101b4b";

            //public const string PrimalistTakeBloodlinePower4 = "2140040bf367e8b4a9c6a632820becbe";
            //public const string PrimalistTakeBloodlinePower8 = "c5aaccc685a37ed4b97869398cdd3ebb";
            //public const string PrimalistTakeBloodlinePower12 = "57bb4dc36611c7444817c13135ec58b4";
            //public const string PrimalistTakeBloodlinePower16 = "a56a288b9b6097f4eb67be43404321f2";
            //public const string PrimalistTakeBloodlinePower20 = "8cfab12649049e44aaa49691a8a16d88";
        }

        public static readonly OwlcatBlueprint<BlueprintFeatureSelection> BloodragerBloodlineSelection =
            new(Guids.BloodragerBloodlineSelection);
        public static readonly OwlcatBlueprint<BlueprintProgression> PrimalistProgression =
            new(Guids.PrimalistProgression);
        public static readonly OwlcatBlueprint<BlueprintArchetype> PrimalistArchetype =
            new(Guids.PrimalistArchetype);

        public static readonly OwlcatBlueprint<BlueprintFeatureSelection> PrimalistLevel4Selection =
            new(Guids.PrimalistLevel4Selection);
        public static readonly OwlcatBlueprint<BlueprintFeatureSelection> PrimalistLevel8Selection =
            new(Guids.PrimalistLevel8Selection);
        public static readonly OwlcatBlueprint<BlueprintFeatureSelection> PrimalistLevel12Selection =
            new(Guids.PrimalistLevel12Selection);
        public static readonly OwlcatBlueprint<BlueprintFeatureSelection> PrimalistLevel16Selection =
            new(Guids.PrimalistLevel16Selection);
        public static readonly OwlcatBlueprint<BlueprintFeatureSelection> PrimalistLevel20Selection =
            new(Guids.PrimalistLevel20Selection);

        public static readonly OwlcatBlueprint<BlueprintProgression> PrimalistTakeRagePowers4 =
            new(Guids.PrimalistTakeRagePowers4);
        public static readonly OwlcatBlueprint<BlueprintProgression> PrimalistTakeRagePowers8 =
            new(Guids.PrimalistTakeRagePowers8);
        public static readonly OwlcatBlueprint<BlueprintProgression> PrimalistTakeRagePowers12 =
            new(Guids.PrimalistTakeRagePowers12);
        public static readonly OwlcatBlueprint<BlueprintProgression> PrimalistTakeRagePowers16 =
            new(Guids.PrimalistTakeRagePowers16);
        public static readonly OwlcatBlueprint<BlueprintProgression> PrimalistTakeRagePowers20 =
            new(Guids.PrimalistTakeRagePowers20);
    }

    public static class PrimalistRageSelections
    {

        internal static IEnumerable<BlueprintFeatureBase> BloodlinePowerForLevel(
            BlueprintProgression bloodline, int level) =>
            bloodline.GetLevelEntry(level).Features
                .Where(f =>
                    f.Components
                        .OfType<PrerequisiteNoFeature>()
                        .Where(p => p.Feature.AssetGuid == BlueprintGuid.Parse(OwlcatBlueprints.Guids.PrimalistProgression))
                        .Any());

        internal class BloodlinePowers
        {
            public Dictionary<int, IEnumerable<BlueprintFeatureBase>> AllPowers { get; } = new();

            public IEnumerable<BlueprintFeatureBase> Level4
            {
                get => AllPowers[4];
                set => AllPowers[4] = value;
            }

            public IEnumerable<BlueprintFeatureBase> Level8
            {
                get => AllPowers[8];
                set => AllPowers[8] = value;
            }

            public IEnumerable<BlueprintFeatureBase> Level12
            {
                get => AllPowers[12];
                set => AllPowers[12] = value;
            }

            public IEnumerable<BlueprintFeatureBase> Level16
            {
                get => AllPowers[16];
                set => AllPowers[16] = value;
            }

            public IEnumerable<BlueprintFeatureBase> Level20
            {
                get => AllPowers[20];
                set => AllPowers[20] = value;
            }
        }

        internal static Dictionary<BlueprintProgression, BloodlinePowers> GetBloodragerBloodlinePowers()
        {
            var bloodlineSelection = OwlcatBlueprints.BloodragerBloodlineSelection.GetBlueprint();
            var bloodlines = bloodlineSelection.m_AllFeatures.Select(f => f.Get()).OfType<BlueprintProgression>();

            Main.Log?.Debug($"{bloodlines.Count()} bloodlines");

            var powers = bloodlines.Select(bloodline =>
                (bloodline, new BloodlinePowers()
                {
                    Level4 = BloodlinePowerForLevel(bloodline, 4),
                    Level8 = BloodlinePowerForLevel(bloodline, 8),
                    Level12 = BloodlinePowerForLevel(bloodline, 12),
                    Level16 = BloodlinePowerForLevel(bloodline, 16),
                    Level20 = BloodlinePowerForLevel(bloodline, 20)
                }));

            return powers.ToDictionary();
        }

        internal class PrerequisiteInProgression : PrerequisiteFeature
        {
            public PrerequisiteInProgression() : base() { }

            public PrerequisiteInProgression(BlueprintProgression progression) : this() =>
                m_Feature = progression.ToReference<BlueprintFeatureReference>();

            public BlueprintProgression? Progression => Feature as BlueprintProgression;

            public override bool CheckInternal(FeatureSelectionState selectionState, UnitDescriptor unit, LevelUpState state)
            {
                if (Progression is null || !base.CheckInternal(selectionState, unit, state)) return false;

                if (selectionState?.SourceFeature is var source)
                    return source == Progression;

                return false;
            }
        }

        internal class PrerequisiteCustom : Prerequisite
        {
            public PrerequisiteCustom() : base() { }

            public Func<FeatureSelectionState, UnitDescriptor, LevelUpState, bool> Predicate = (_, _, _) => false;

            public Func<UnitDescriptor, string> GenerateUIText = _ => "";

            public PrerequisiteCustom(Func<FeatureSelectionState, UnitDescriptor, LevelUpState, bool> predicate) : this() =>
                Predicate = predicate;

            public override bool CheckInternal(FeatureSelectionState selectionState, UnitDescriptor unit, LevelUpState state) =>
                Predicate(selectionState, unit, state);
            public override string GetUITextInternal(UnitDescriptor unit) => GenerateUIText(unit);
        }

        internal static List<(int level, BlueprintFeatureSelection selection)> GetBloodlineSelections() => new()
        {
            (4, OwlcatBlueprints.PrimalistLevel4Selection.GetBlueprint()),
            (8, OwlcatBlueprints.PrimalistLevel8Selection.GetBlueprint()),
            (12, OwlcatBlueprints.PrimalistLevel12Selection.GetBlueprint()),
            (16, OwlcatBlueprints.PrimalistLevel16Selection.GetBlueprint()),
            (20, OwlcatBlueprints.PrimalistLevel20Selection.GetBlueprint())
        };

        internal static List<(int level, BlueprintProgression powers)> GetRagePowerEntries() => new()
        {
            (4, OwlcatBlueprints.PrimalistTakeRagePowers4.GetBlueprint()),
            (8, OwlcatBlueprints.PrimalistTakeRagePowers8.GetBlueprint()),
            (12, OwlcatBlueprints.PrimalistTakeRagePowers12.GetBlueprint()),
            (16, OwlcatBlueprints.PrimalistTakeRagePowers16.GetBlueprint()),
            (20, OwlcatBlueprints.PrimalistTakeRagePowers20.GetBlueprint())
        };

        internal static void SetLevelEntry(this BlueprintProgression progression, int level, Func<LevelEntry, LevelEntry> mutator)
        {
            var index = progression.LevelEntries.FindIndex(le => le.Level == level);

            progression.LevelEntries[index] = mutator(progression.LevelEntries[index]);
        }

        internal static void PatchPrimalistProgression()
        {
            Main.Log?.Debug($"{nameof(PrimalistRageSelections)}.{nameof(PatchPrimalistProgression)}");

            var primalistProgression = OwlcatBlueprints.PrimalistProgression.GetBlueprint();

            var ragePowerEntries = GetRagePowerEntries();
            var bloodlineSelections = GetBloodlineSelections();

            foreach (var (level, selection) in bloodlineSelections)
            {
                //selection.Components = new BlueprintComponent[0];
                selection.AddPrerequisiteFeature(primalistProgression, init: prereq => prereq.CheckInProgression = true);

                selection.HideNotAvailibleInUI = true;

                selection.HideInCharacterSheetAndLevelUp = true;

                selection.SetFeatures(
                    ragePowerEntries
                        .Where(p => p.level == level)
                        .Select(p => p.powers.ToReference<BlueprintFeatureReference>())
                        .ToArray());
            }

            foreach (var (_, ragePowers) in ragePowerEntries)
            {
                ragePowers.AddPrerequisiteFeature(primalistProgression, init: Functional.Ignore);
                ragePowers.AddPrerequisiteNoFeature(ragePowers, init: prereq => prereq.HideInUI = true);
                ragePowers.HideNotAvailibleInUI = true;
            }

            var bloodlinePowers = GetBloodragerBloodlinePowers();

            foreach (var bloodline in bloodlinePowers.Keys)
            {
                Main.Log?.Debug($"Patching bloodline {bloodline.Name}. {bloodlinePowers[bloodline].AllPowers.Count} powers");

                foreach (var (level, feats) in bloodlinePowers[bloodline].AllPowers)
                {
                    var levelEntry = bloodline.GetLevelEntry(level);
                    var levelEntryFeatures = levelEntry.Features;

                    foreach (var feat in feats)
                    {
                        if (feat is not BlueprintFeature f) continue;

                        Main.Log?.Debug($" - Patching feature {f.Name}");

                        f.RemoveComponents(c =>
                            c is PrerequisiteNoFeature p
                            && p.Feature.AssetGuid == primalistProgression.AssetGuid);

                        f.AddComponent(new PrerequisiteInProgression(bloodline));

                        f.HideNotAvailibleInUI = true;

                        foreach (var bs in bloodlineSelections.Where(s => s.level == level))
                        {
                            bs.selection.AddFeature(f);

                            f.AddComponent(new PrerequisiteCustom()
                            {
                                GenerateUIText = _ => primalistProgression.Name,
                                Predicate = (selectionState, unit, state) =>
                                {
                                    return !unit.Progression.Features.HasFact(primalistProgression)
                                        || selectionState?.Selection == bs.selection;
                                },

                                CheckInProgression = true
                            });
                        }
                    }

                    levelEntryFeatures =
                        new IEnumerable<BlueprintFeatureBase>[]
                        {
                            levelEntryFeatures,
                            bloodlineSelections
                                .Where(s => s.level == level)
                                .Select(s => s.selection)
                                //.Cast<BlueprintFeatureBase>()
                        }.SelectMany(Functional.Id).ToList();

                    levelEntry.SetFeatures(levelEntryFeatures);

                    bloodline.SetLevelEntry(level, _ => levelEntry);
                }
            }

            primalistProgression.LevelEntries = new LevelEntry[0];
        }
    }
}
