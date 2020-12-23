using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Hazel;
#if ITCH
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
#elif STEAM
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
    public class RpcFunctions
    {
        static public void RpcExample(bool data)
        {
            var writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, 140, SendOption.Reliable);
            writer.WritePacked(PlayerControl.LocalPlayer.NetId);
            writer.Write(data);
            writer.EndMessage();
        }
    }
}
