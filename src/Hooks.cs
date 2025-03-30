using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using System;
using UnityEngine;
using static JollyCoop.JollyEnums;
using static MoreSlugcats.MoreSlugcatsEnums;
using static Player;

namespace MaulAliveCreatures;

public static class Hooks
{
    private static bool mauling;

    public static void Init()
    {
        IL.Player.GrabUpdate += Player_GrabUpdate;
        On.Lizard.Violence += Lizard_Violence;
        On.Creature.Violence += Creature_Violence;
        On.Player.ReleaseGrasp += Player_ReleaseGrasp;
        On.Player.TossObject += Player_TossObject;
        On.Player.GrabUpdate += Player_GrabUpdate1;
        On.Player.Grabability += Player_Grabability;
        On.Player.CanEatMeat += Player_CanEatMeat;
        On.Player.CanMaulCreature += Player_CanMaulCreature;
        On.SlugcatStats.SlugcatCanMaul += SlugcatStats_SlugcatCanMaul;
    }

    public static void Unhook()
    {
        IL.Player.GrabUpdate -= Player_GrabUpdate;
        On.Lizard.Violence -= Lizard_Violence;
        On.Creature.Violence -= Creature_Violence;
        On.Player.ReleaseGrasp -= Player_ReleaseGrasp;
        On.Player.TossObject -= Player_TossObject;
        On.Player.GrabUpdate -= Player_GrabUpdate1;
        On.Player.Grabability -= Player_Grabability;
        On.Player.CanEatMeat -= Player_CanEatMeat;
        On.Player.CanMaulCreature -= Player_CanMaulCreature;
        On.SlugcatStats.SlugcatCanMaul -= SlugcatStats_SlugcatCanMaul;
    }

    private static bool SlugcatStats_SlugcatCanMaul(On.SlugcatStats.orig_SlugcatCanMaul orig, SlugcatStats.Name slugcatNum)
    {
        orig.Invoke(slugcatNum);
        bool maulPups = OptionsMenu.CanMaulPups.Value;
        bool canMaul = OptionsMenu.CanMaul.Value;
        bool canMaulMSC = OptionsMenu.NoMaulMSC.Value;
        if (ModManager.MSC && canMaulMSC && ((ExtEnum<Name>)(object)slugcatNum == (ExtEnum<Name>)(object)SlugcatStatsName.Saint || (ExtEnum<Name>)(object)slugcatNum == (ExtEnum<Name>)(object)SlugcatStatsName.Spear))
        {
            return orig.Invoke(slugcatNum);
        }
        if (maulPups)
        {
            return true;
        }
        if (canMaul && !maulPups)
        {
            return canMaul;
        }
        return orig.Invoke(slugcatNum);
    }

    private static bool Player_CanMaulCreature(On.Player.orig_CanMaulCreature orig, Player self, Creature crit)
    {
        orig.Invoke(self, crit);
        bool canMaul = true;
        bool critChecks = crit is not Cicada || crit is not Yeek || crit is not JetFish;
        bool critStun = !crit.Stunned || crit.Stunned;
        bool sessionStory = ((UpdatableAndDeletable)self).room.game.IsStorySession;
        bool sessionArena = ((UpdatableAndDeletable)self).room.game.IsArenaSession;
        bool maulPups = OptionsMenu.CanMaulPups.Value;
        bool maulAllies = OptionsMenu.EnableMaulAllies.Value;
        bool canMaulMSC = OptionsMenu.NoMaulMSC.Value;
        if (!maulPups)
        {
            Player player = (Player)(object)((crit is Player) ? crit : null);
            if (crit is Player && player != null && (player.isNPC || player.isSlugpup || !Custom.rainWorld.options.friendlyFire))
            {
                canMaul = false;
            }
        }
        if (maulPups && !maulAllies && crit is Player && sessionStory)
        {
            canMaul = false;
        }
        if (ModManager.MSC && canMaulMSC && ((ExtEnum<Name>)(object)self.SlugCatClass == (ExtEnum<Name>)(object)SlugcatStatsName.Spear || (ExtEnum<Name>)(object)self.SlugCatClass == (ExtEnum<Name>)(object)SlugcatStatsName.Saint))
        {
            canMaul = false;
        }
        if (canMaul && critChecks && !Custom.rainWorld.options.friendlyFire && crit != null && !crit.dead && crit is not IPlayerEdible && critStun)
        {
            if (OptionsMenu.EnableExtraStun.Value)
            {
                crit.Stun(20);
            }
            return true;
        }
        if (crit is Cicada && crit.Stunned)
        {
            return true;
        }
        if (crit is Player && crit.Stunned && sessionArena)
        {
            return true;
        }
        return false;
    }

    private static bool Player_CanEatMeat(On.Player.orig_CanEatMeat orig, Player self, Creature crit)
    {
        orig.Invoke(self, crit);
        bool maulPups = OptionsMenu.CanMaulPups.Value;
        bool playerCheck = crit is not Player && (!self.isNPC || !self.isSlugpup);
        bool canMaulMSC = OptionsMenu.NoMaulMSC.Value;
        if (ModManager.MSC && canMaulMSC && ((ExtEnum<Name>)(object)self.SlugCatClass == (ExtEnum<Name>)(object)SlugcatStatsName.Spear || (ExtEnum<Name>)(object)self.SlugCatClass == (ExtEnum<Name>)(object)SlugcatStatsName.Saint))
        {
            return orig.Invoke(self, crit);
        }
        if (maulPups && crit.dead)
        {
            return true;
        }
        if (!maulPups && !OptionsMenu.CantMaulCreatures.Value && crit.dead && playerCheck)
        {
            return true;
        }
        return orig.Invoke(self, crit);
    }

    private static Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
    {
        if (obj is Player || obj is JetFish || obj is Fly || obj is Hazer || obj is PoleMimic || obj is Snail || obj is Cicada || obj is LanternMouse || obj is EggBug || obj is Centipede || obj is FlyLure || obj is SmallNeedleWorm || obj is TentaclePlant || obj is VultureGrub || obj is Spider)
        {
            return orig.Invoke(self, obj);
        }
        if (obj is not Scavenger && obj is not BigEel)
        {
            Player player = (Player)(object)((obj is Player) ? obj : null);
            if ((player == null || (!player.isNPC && !player.isSlugpup)) && obj is not Hazer && obj is not VultureGrub)
            {
                if (obj is BigJellyFish || obj is Yeek)
                {
                    return (ObjectGrabability)3;
                }
                if (obj is Creature)
                {
                    return (ObjectGrabability)4;
                }
                return orig.Invoke(self, obj);
            }
        }
        return (ObjectGrabability)1;
    }

    private static void Player_GrabUpdate1(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        mauling = self.maulTimer > 20;
        try
        {
            orig.Invoke(self, eu);
        }
        finally
        {
            mauling = false;
        }
    }

    private static void Player_TossObject(On.Player.orig_TossObject orig, Player self, int grasp, bool eu)
    {
        if (!mauling)
        {
            orig.Invoke(self, grasp, eu);
        }
    }

    private static void Player_ReleaseGrasp(On.Player.orig_ReleaseGrasp orig, Player self, int grasp)
    {
        if (!mauling)
        {
            orig.Invoke(self, grasp);
        }
    }

    private static void Creature_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
    {
        orig.Invoke(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
        Room room = ((UpdatableAndDeletable)self).room;
        bool bite = (ExtEnum<Creature.DamageType>)(object)type == (ExtEnum<Creature.DamageType>)(object)Creature.DamageType.Bite;
        PhysicalObject obj = (source?.owner);
        Player player1 = (Player)(object)((obj is Player) ? obj : null);
        if (player1 != null && bite && (double)UnityEngine.Random.value <= 0.05)
        {
            if (!OptionsMenu.DisableSounds.Value && room != null)
            {
                room.PlaySound(Enums.yummers, ((PhysicalObject)self).firstChunk);
            }
            if (!OptionsMenu.NoMeatExtra.Value)
            {
                player1.AddQuarterFood();
            }
        }
        if ((source?.owner) is Player && bite && !OptionsMenu.DisableSounds.Value && room != null)
        {
            room.PlaySound(Enums.munch, ((PhysicalObject)self).firstChunk);
        }
    }

    private static void Lizard_Violence(On.Lizard.orig_Violence orig, Lizard self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos onAppendagePos, Creature.DamageType type, float damage, float stunBonus)
    {
        orig.Invoke(self, source, directionAndMomentum, hitChunk, onAppendagePos, type, damage, stunBonus);
        Room room = ((UpdatableAndDeletable)self).room;
        bool bite = (ExtEnum<Creature.DamageType>)(object)type == (ExtEnum<Creature.DamageType>)(object)Creature.DamageType.Bite;
        PhysicalObject obj = (source?.owner);
        Player player1 = (Player)(object)((obj is Player) ? obj : null);
        if (player1 != null && bite && (double)UnityEngine.Random.value <= 0.05)
        {
            if (!OptionsMenu.DisableSounds.Value && room != null)
            {
                room.PlaySound(Enums.yummers, ((PhysicalObject)self).firstChunk);
            }
            if (!OptionsMenu.NoMeatExtra.Value)
            {
                player1.AddQuarterFood();
            }
        }
        if ((source?.owner) is Player && bite && !OptionsMenu.DisableSounds.Value && room != null)
        {
            room.PlaySound(Enums.munch, ((PhysicalObject)self).firstChunk);
        }
    }

    private static void Player_GrabUpdate(MonoMod.Cil.ILContext il)
    {
        var cursor = new ILCursor(il);
        try
        {
            if (cursor.TryGotoNext(
            [
                (Instruction i) => ILPatternMatchingExt.MatchLdsfld<Creature.DamageType>(i, "Bite")
            ]) && cursor.TryGotoNext(
            [
                (Instruction i) => ILPatternMatchingExt.MatchCallOrCallvirt<Creature>(i, "Violence")
            ]) && cursor.TryGotoPrev((MoveType)2,
            [
                (Instruction i) => ILPatternMatchingExt.MatchLdcR4(i, 1f)
            ]))
            {
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.EmitDelegate<Func<float, Player, float>>((Func<float, Player, float>)((float baseDamage, Player player) => 1f + OptionsMenu.MaulDamageExtra.Value));
            }
            Plugin.DebugWarning("CUSTOM MAUL DAMAGE SET FROM MAUL ALIVE CREATURES");
        }
        catch (Exception ex)
        {
            Debug.LogError((object)"THE IL HOOK IN PLAYER GRAB UPDATE, DAMAGE, FAILED SO MUCH, SKILL ISSUE!");
            Debug.LogException(ex);
            Debug.LogError((object)il);
            throw;
        }
    }
}