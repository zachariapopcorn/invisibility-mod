using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using UnityEngine;
using ButtonManager;
using Hazel;

namespace InvisibilityMod
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class InvisibilityMod : BasePlugin
    {
        public const string Id = "InvisibilityMod";
        public static BepInEx.Logging.ManualLogSource log;

        public Harmony Harmony { get; } = new Harmony(Id);

        public override void Load()
        {
            log = Log;
            log.LogMessage("Invisibility Mod has loaded");
            Harmony.PatchAll();
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
        public static class HudPatch
        {
            public static void Postfix(HudManager __instance)
            {
                new CooldownButton(
                    () =>
                    {
                        PlayerControl player = PlayerControl.LocalPlayer;
                        Utils.SetColor(player.myRend, 0.2f);
                        player.nameText.enabled = false;
                        player.HatRenderer.FrontLayer.enabled = false;
                        player.HatRenderer.BackLayer.enabled = false;
                        if(player.MyPhysics.Skin)
                        {
                            Utils.SetColor(player.MyPhysics.Skin.layer, 0.2f);
                        }
                        if(player.CurrentPet)
                        {
                            Utils.SetColor(player.CurrentPet.rend, 0.2f);
                        }
                        MessageWriter sender = AmongUsClient.Instance.StartRpc(player.NetId, 40, Hazel.SendOption.Reliable);
                        sender.Write(player.Data.PlayerId);
                        sender.EndMessage();
                    },
                    40f,
                    Properties.Resources.InvisibleButton,
                    new Vector2(0.125f, 0.125f),
                    () =>
                    {
                        if(!AmongUsClient.Instance.IsGameStarted) return false;
                        if(PlayerControl.LocalPlayer.Data.IsDead) return false;
                        if(PlayerControl.LocalPlayer.Data.IsImpostor) return true;
                        return false;
                    },
                    __instance,
                    20f,
                    () =>
                    {
                        PlayerControl player = PlayerControl.LocalPlayer;
                        Utils.SetColor(player.myRend, 1f);
                        player.nameText.enabled = true;
                        player.HatRenderer.FrontLayer.enabled = true;
                        player.HatRenderer.BackLayer.enabled = true;
                        if(player.MyPhysics.Skin)
                        {
                            Utils.SetColor(player.MyPhysics.Skin.layer, 1f);
                        }
                        if(player.CurrentPet)
                        {
                            Utils.SetColor(player.CurrentPet.rend, 1f);
                        }
                        MessageWriter sender = AmongUsClient.Instance.StartRpc(player.NetId, 41, Hazel.SendOption.Reliable);
                        sender.Write(player.Data.PlayerId);
                        sender.EndMessage();
                    }
                    );
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRpcClass
        {
            public static void Prefix(byte callId, MessageReader reader)
            {
                if (callId != 40 && callId != 41) return;
                byte playerId = reader.ReadByte();
                PlayerControl player = Utils.GetPlayerById(playerId);
                if(player == null)
                {
                    log.LogWarning("Player received from client is null");
                    return;
                }
                if(callId == 40)
                {
                    Utils.SetColor(player.myRend, 0f);
                    player.nameText.enabled = false;
                    player.HatRenderer.FrontLayer.enabled = false;
                    player.HatRenderer.BackLayer.enabled = false;
                    if(player.MyPhysics.Skin)
                    {
                        Utils.SetColor(player.MyPhysics.Skin.layer, 0f);
                    }
                    if(player.CurrentPet)
                    {
                        Utils.SetColor(player.CurrentPet.rend, 0f);
                    }
                } else
                {
                    Utils.SetColor(player.myRend, 1f);
                    player.nameText.enabled = true;
                    player.HatRenderer.FrontLayer.enabled = true;
                    player.HatRenderer.BackLayer.enabled = true;
                    if(player.MyPhysics.Skin)
                    {
                        Utils.SetColor(player.MyPhysics.Skin.layer, 1f);
                    }
                    if(player.CurrentPet)
                    {
                        Utils.SetColor(player.CurrentPet.rend, 1f);
                    }
                }
            }
        }

    }
}
