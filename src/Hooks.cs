using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using System;
using UnityEngine;
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

    private static bool SlugcatStats_SlugcatCanMaul(On.SlugcatStats.orig_SlugcatCanMaul orig, SlugcatStats.Name slugcatNum)
    {
        bool originalResult = orig(slugcatNum);
        bool canMaulMSC = OptionsMenu.NoMaulMSC.Value;

        if (ModManager.MSC && canMaulMSC && (slugcatNum == SlugcatStatsName.Saint || slugcatNum == SlugcatStatsName.Spear))
        {
            return originalResult;
        }

        return OptionsMenu.CanMaulPups.Value || (OptionsMenu.CanMaul.Value ? OptionsMenu.CanMaul.Value : originalResult);
    }

    private static bool Player_CanMaulCreature(On.Player.orig_CanMaulCreature orig, Player self, Creature crit)
    {
        bool maulPups = OptionsMenu.CanMaulPups.Value;
        bool maulAllies = OptionsMenu.EnableMaulAllies.Value;
        bool canMaulMSC = OptionsMenu.NoMaulMSC.Value;
        bool sessionStory = self.room.game.IsStorySession;
        bool sessionArena = self.room.game.IsArenaSession;

        bool critStun = true;

        bool canMaul = true;

        if (!maulPups && crit is Player player && (player.isNPC || player.isSlugpup || !Custom.rainWorld.options.friendlyFire))
        {
            canMaul = false;
        }

        if (maulPups && !maulAllies && crit is Player && sessionStory)
        {
            canMaul = false;
        }

        if (ModManager.MSC && canMaulMSC && (self.SlugCatClass == SlugcatStatsName.Spear || self.SlugCatClass == SlugcatStatsName.Saint))
        {
            canMaul = false;
        }

        if (canMaul && (crit is not Cicada || crit is not Yeek || crit is not JetFish) && !Custom.rainWorld.options.friendlyFire && crit != null && !crit.dead && crit is not IPlayerEdible && critStun)
        {
            if (OptionsMenu.EnableExtraStun.Value)
            {
                crit.Stun(20);
            }
            return true;
        }

        if (crit is Cicada && crit.Stunned)
            return true;

        if (crit is Player && crit.Stunned && sessionArena)
            return true;

        return false;
    }

    private static bool Player_CanEatMeat(On.Player.orig_CanEatMeat orig, Player self, Creature crit)
    {
        bool maulPups = OptionsMenu.CanMaulPups.Value;
        bool canMaulMSC = OptionsMenu.NoMaulMSC.Value;
        bool playerCheck = crit is not Player && (!self.isNPC || !self.isSlugpup);

        if (ModManager.MSC && canMaulMSC &&
            (self.SlugCatClass == SlugcatStatsName.Spear || self.SlugCatClass == SlugcatStatsName.Saint))
        {
            return orig(self, crit);
        }

        if (maulPups && crit.dead)
            return true;

        if (!maulPups && !OptionsMenu.CantMaulCreatures.Value && crit.dead && playerCheck)
            return true;

        return orig(self, crit);
    }

    private static ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
    {
        if (obj is Player or JetFish or Fly or Hazer or PoleMimic or Snail or Cicada
            or LanternMouse or EggBug or Centipede or FlyLure or SmallNeedleWorm or TentaclePlant
            or VultureGrub or Spider)
        {
            return orig(self, obj);
        }

        if (obj is not Scavenger and not BigEel)
        {
            if (obj is not Player player || (!player.isNPC && !player.isSlugpup) && obj is not Hazer && obj is not VultureGrub)
            {
                if (obj is BigJellyFish or Yeek)
                    return ObjectGrabability.TwoHands;
                if (obj is Creature)
                    return ObjectGrabability.Drag;
                return orig(self, obj);
            }
        }
        return ObjectGrabability.OneHand;
    }

    private static void Player_GrabUpdate1(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        mauling = self.maulTimer > 20;
        try
        {
            orig(self, eu);
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
            orig(self, grasp, eu);
        }
    }

    private static void Player_ReleaseGrasp(On.Player.orig_ReleaseGrasp orig, Player self, int grasp)
    {
        if (!mauling)
        {
            orig(self, grasp);
        }
    }

    private static void Creature_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
    {
        orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        Room room = self.room;
        bool bite = type == Creature.DamageType.Bite;

        if (source?.owner is Player player && bite && UnityEngine.Random.value <= 0.05f)
        {
            if (!OptionsMenu.DisableSounds.Value && room != null)
                room.PlaySound(Enums.yummers, self.firstChunk);

            if (!OptionsMenu.NoMeatExtra.Value)
                player.AddQuarterFood();
        }

        if (source?.owner is Player && bite && !OptionsMenu.DisableSounds.Value && room != null)
        {
            room.PlaySound(Enums.munch, self.firstChunk);
        }
    }

    private static void Lizard_Violence(On.Lizard.orig_Violence orig, Lizard self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos onAppendagePos, Creature.DamageType type, float damage, float stunBonus)
    {
        orig(self, source, directionAndMomentum, hitChunk, onAppendagePos, type, damage, stunBonus);

        Room room = self.room;
        bool bite = type == Creature.DamageType.Bite;

        if (source?.owner is Player player && bite && UnityEngine.Random.value <= 0.05f)
        {
            if (!OptionsMenu.DisableSounds.Value && room != null)
                room.PlaySound(Enums.yummers, self.firstChunk);

            if (!OptionsMenu.NoMeatExtra.Value)
                player.AddQuarterFood();
        }
        if (source?.owner is Player && bite && !OptionsMenu.DisableSounds.Value && room != null)
        {
            room.PlaySound(Enums.munch, self.firstChunk);
        }
    }

    private static void Player_GrabUpdate(ILContext il)
    {
        var cursor = new ILCursor(il);
        try
        {
            if (cursor.TryGotoNext(i => i.MatchLdsfld<Creature.DamageType>(nameof(Creature.DamageType.Bite))) && 
                cursor.TryGotoNext(i => i.MatchCallOrCallvirt<Creature>(nameof(Creature.Violence))) && 
                cursor.TryGotoPrev(MoveType.After, i => i.MatchLdcR4(1f)))
            {
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.EmitDelegate((float baseDamage, Player player) => 1f + OptionsMenu.MaulDamageExtra.Value);
            }
            else
            {
                Plugin.DebugError("Failed to find injection point for Player_GrabUpdate");
            }
        }
        catch (Exception ex)
        {
            Plugin.DebugError("Got an Exception Player_GrabUpdate ILHook, report if you see this..." + ex);
            throw;
        }
    }
}