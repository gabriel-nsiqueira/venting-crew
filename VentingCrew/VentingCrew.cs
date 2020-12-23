using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Collections.Generic;
using Hazel;
using System;
using System.IO;
using Hazel.Udp;
#if ITCH
//Mappings for itch among us
using InnerNetObject = DPIBCOKLFPN;
using InnerNetClient = EALKALAAHDM;
using ServerManager = CJPEJFOJIOC;
using PlayerControl = ELIOALIOPDI;
using PlayerInfo = LBJNDJLFGDM.IOAPMBCNLBK;
using GameData = LBJNDJLFGDM;
using ShipStatus = EPHLEGMMJCC;
using HudManager = NCPDOOIHJKC;
using KillButtonManager = PIIAIJBIJLL;
using AmongUsClient = BMBJNONAKIO;
using PassiveButton = AJGGJOBLDCP;
using PingTracker = FAINCKIMJII;
using PlayerPhisics = IFHEMDKICHP;
using IUsable = PEIKCOBNCAI;
using ServerRegion = KMFCKPLMGDK;
using ServerInfo = PIOPAJCMNDK;
using VersionShower = MGIHLHPAPLB;
#elif STEAM
//Mappings for steam among us
using InnerNetObject = NJAHILONGKN;
using MeetingHud = OOCJALPKPEP;
using MeetingHud_VoteStates = OOCJALPKPEP.BAMDJGFKOFP;
using ShipStatus = HLBNNHFCNAJ;
using GameData = EGLJNOMOGNP;
using PlayerInfo = EGLJNOMOGNP.DCJMABDDJCF;
using NoShadowBehaviour = NIMAOKMAOEC;
using GameOptionsData = KMOGFLPJLLK;
using PlayerVoteArea = HDJGDMFCHDN;
using PlayerTab = MAOILGPNFND;
using TextRenderer = AELDHKGBIFD;
using PingTracker = ELDIDNABIPI;

using GameStartManager = ANKMIOIMNFE;
using InnerNetClient = KHNHJFFECBP;
using AmongUsClient = FMLLKEACGIO;
using VersionShower = BOCOFLHKCOJ;
using ServerManager = AOBNFCIHAJL;
using ServerRegion = OIBMKGDLGOG;
using ServerInfo = PLFDMKKDEMI;
using HudManager = PIEFJFEOGOL;
using PassiveButton = HHMBANDDIOA;
using KillButtonManager = MLPJGKEACMM;

using PoolableBehavior = CCLDIACANAF;
using PlayerAnimator = GIFADNNODPB;
using PlayerControl = FFGALNAPKCD;
using PlayerParticle = DPGAAKLFEDA;
using PlayerPhysics = LIMNONMAAFA;
#endif

namespace VentingCrew
{
    [BepInPlugin("com.gabrielsiqueira.VentingCrew", "Venting Crew", "0.1.0")]
    [BepInProcess("Among Us.exe")]
    public class VentingCrew : BasePlugin
    {
        public static BepInEx.Logging.ManualLogSource log;
        public static HudManager hudManager = null;


        public override void Load()
        {
            log = Log;
            var name = "localhost";
            var ip = "127.0.0.1";
            ushort port = 22023;

            var defaultRegions = ServerManager.DefaultRegions.ToList();

            defaultRegions.Insert(0, new ServerRegion(
                name, ip, new ServerInfo[]
                {
                    new ServerInfo($"{name}-Master-1", ip, port)
                })
            );

            ServerManager.DefaultRegions = defaultRegions.ToArray();
            //UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<ButtonManager>();


            Harmony _harmony;
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }
    // Impostors walk through walls
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class PlayerControlFixedUpdatePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
#if STEAM
            if (__instance.NDGFFHMFGIG == null) return;
            bool isImpostor = __instance.NDGFFHMFGIG.DAPKNDBLKIA;
#elif ITCH
            if (__instance.IOLEBICMFDO == null) return;
            bool isImpostor = __instance.IOLEBICMFDO.NMPIGEJPNAM;
#endif
            if (__instance != PlayerControl.LocalPlayer && isImpostor)
            {
                __instance.gameObject.layer = LayerMask.NameToLayer("Ghost");
                return;
            }
            if (Input.GetKey(KeyCode.X))
            {
                __instance.gameObject.layer = LayerMask.NameToLayer("Ghost");
            }
            else
            {
                __instance.gameObject.layer = LayerMask.NameToLayer("Players");
            }
        }
        
    }
#if ITCH
    // Choose impostors patch
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CANBADGMALM))]
    public class ImpostorPatch
    {
        public static bool Prefix(ShipStatus __instance)
        {
            Func<PlayerInfo, bool> p = (player1) => { return player1.CLFIOGOAIMO == "koresd"; };
            PlayerControl.LocalPlayer.RpcSetInfected(new PlayerInfo[] { PlayerControl.LocalPlayer.IOLEBICMFDO, GameData.Instance.AllPlayers.Find(p) });
            return false;
        }
    }
#endif
    // Receive rpc calls
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public class RpcPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(1)] MessageReader message, [HarmonyArgument(0)] int callId)
        {
            if(callId == 140)
            {
                message.ReadByte();
                bool data = message.ReadBoolean();
            }
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public class HudStartPatch
    {
        public static void Postfix(HudManager __instance)
        {
            VentingCrew.hudManager = __instance;
            CooldownButton button = new CooldownButton(
                () => { PlayerControl.LocalPlayer.RpcSetScanner(true); },
                20f,
                "VentingCrew.Resources.small.png",
                270f,
                Vector2.zero,
                CooldownButton.Category.Everyone,
                __instance,
                10f,
                () => { PlayerControl.LocalPlayer.RpcSetScanner(false) ; }
             );
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudUpdatePatch
    {
        public static void Postfix(HudManager __instance)
        {
            CooldownButton.HudUpdate();
        }
    }

    // Ping Patch
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public class PingTextPatch
    {
        public static void Postfix(PingTracker __instance)
        {
            __instance.text.Text += "\r\n[00FF00FF]> minethingo <";
        }
    }

    // Version name patch
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public class VersionPatch
    {
        public static void Postfix(VersionShower __instance)
        {
            __instance.text.Text += " [00FF00FF]Modded by minethingo[]";
        }
    }
}