using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using UnityEngine;
using System.Reflection;
using System.IO;
using UnhollowerBaseLib;

namespace VentingCrew
{
    public class CooldownButton
    {
        public static List<CooldownButton> buttons = new List<CooldownButton>();
        public KillButtonManager killButtonManager;
        private Color startColorButton = new Color(255, 255, 255);
        private Color startColorText = new Color(255, 255, 255);
        public Vector2 PositionOffset = Vector2.zero;
        public float MaxTimer = 0f;
        public float Timer = 0f;
        public float EffectDuration = 0f;
        public bool isEffectActive;
        public bool hasEffectDuration;
        public bool enabled = true;
        public Category category;
        private string ResourceName;
        private Action OnClick;
        private Action OnEffectEnd;
        private HudManager hudManager;
        private float pixelsPerUnit;
        private bool canUse;

        public CooldownButton(Action OnClick, float Cooldown, string ImageEmbededResourcePath, float PixelsPerUnit, Vector2 PositionOffset, Category category, HudManager hudManager, float EffectDuration, Action OnEffectEnd)
        {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.OnEffectEnd = OnEffectEnd;
            this.PositionOffset = PositionOffset;
            this.EffectDuration = EffectDuration;
            this.category = category;
            pixelsPerUnit = PixelsPerUnit;
            MaxTimer = Cooldown;
            Timer = MaxTimer;
            ResourceName = ImageEmbededResourcePath;
            hasEffectDuration = true;
            isEffectActive = false;
            buttons.Add(this);
            Start();
        }

        public CooldownButton(Action OnClick, float Cooldown, string ImageEmbededResourcePath, float pixelsPerUnit, Vector2 PositionOffset, Category category, HudManager hudManager)
        {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.pixelsPerUnit = pixelsPerUnit;
            this.PositionOffset = PositionOffset;
            this.category = category;
            MaxTimer = Cooldown;
            Timer = MaxTimer;
            ResourceName = ImageEmbededResourcePath;
            hasEffectDuration = false;
            buttons.Add(this);
            Start();
        }

        private void Start()
        {
            killButtonManager = UnityEngine.Object.Instantiate(hudManager.KillButton, hudManager.transform);
            startColorButton = killButtonManager.renderer.color;
            startColorText = killButtonManager.TimerText.Color;
            killButtonManager.gameObject.SetActive(true);
            killButtonManager.renderer.enabled = true;
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream myStream = assembly.GetManifestResourceStream(ResourceName);
            byte[] buttonTexture = new byte[myStream.Length];
            myStream.Read(buttonTexture, 0, (int)myStream.Length);
            LoadImage(tex, buttonTexture, false);
            killButtonManager.renderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            PassiveButton button = killButtonManager.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener((UnityEngine.Events.UnityAction)listener);
            void listener()
            {
                if (Timer < 0f && canUse)
                {
                    killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                    if (hasEffectDuration)
                    {
                        isEffectActive = true;
                        Timer = EffectDuration;
                        killButtonManager.TimerText.Color = new Color(0, 255, 0);
                    }
                    OnClick();
                }
            }
        }
        public void CanUse()
        {
#if STEAM
            if (PlayerControl.LocalPlayer.NDGFFHMFGIG == null) return;
            bool isImpostor = PlayerControl.LocalPlayer.NDGFFHMFGIG.DAPKNDBLKIA;
#elif ITCH
            if (PlayerControl.LocalPlayer.IOLEBICMFDO == null) return;
            bool isImpostor = PlayerControl.LocalPlayer.IOLEBICMFDO.NMPIGEJPNAM;
#endif
            switch (category)
            {
                case Category.Everyone:
                    {
                        canUse = true;
                        break;
                    }
                case Category.OnlyCrewmate:
                    {
                        canUse = !isImpostor;
                        break;
                    }
                case Category.OnlyImpostor:
                    {
                        canUse = isImpostor;
                        break;
                    }
            }
        }
        public static void HudUpdate()
        {
            buttons.RemoveAll(item => item.killButtonManager == null);
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].CanUse();
                buttons[i].Update();
            }
        }
        private void Update()
        {
            if (killButtonManager.transform.localPosition.x > 0f)
                killButtonManager.transform.localPosition = new Vector3((killButtonManager.transform.localPosition.x + 1.3f) * -1, killButtonManager.transform.localPosition.y, killButtonManager.transform.localPosition.z) + new Vector3(PositionOffset.x, PositionOffset.y);
            if (Timer < 0f)
            {
                killButtonManager.renderer.color = new Color(1f, 1f, 1f, 1f);
                if (isEffectActive)
                {
                    killButtonManager.TimerText.Color = startColorText;
                    Timer = MaxTimer;
                    isEffectActive = false;
                    OnEffectEnd();
                }
            }
            else
            {
                if (canUse)
                    Timer -= Time.deltaTime;
                killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
            }
            killButtonManager.gameObject.SetActive(canUse);
            killButtonManager.renderer.enabled = canUse;
            if (canUse)
            {
                killButtonManager.renderer.material.SetFloat("_Desat", 0f);
                killButtonManager.SetCoolDown(Timer, MaxTimer);
            }
        }
        internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
        internal static d_LoadImage iCall_LoadImage;
        public static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            if (iCall_LoadImage == null)
                iCall_LoadImage = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");

            var il2cppArray = (Il2CppStructArray<byte>)data;

            return iCall_LoadImage.Invoke(tex.Pointer, il2cppArray.Pointer, markNonReadable);
        }
        public enum Category
        {
            Everyone,
            OnlyCrewmate,
            OnlyImpostor
        }
    }
}
