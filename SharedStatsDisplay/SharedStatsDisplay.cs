namespace SharedStatsDisplay
{
	using BepInEx;
	using BepInEx.Configuration;
	using RoR2;
	using RoR2.Stats;
	using System.Reflection;
	using System.Text;
	using UnityEngine;
	using UnityEngine.SceneManagement;
    using UnityEngine.Networking;
    using System.Collections.Generic;
    using R2API.Utils;
    using RoR2.Networking;
    using System;

    [BepInPlugin("com.kookehs.statsdisplay", "SharedStatsDisplay", "1.3")]
    [R2APISubmoduleDependency(nameof(R2API.PrefabAPI))]
    public class SharedStatsDisplay : BaseUnityPlugin
	{
#pragma warning disable CS0618 // Type or member is obsolete
        public static ConfigWrapper<string> title { get; private set; }
		public static ConfigWrapper<string> characterBodyStatsNames { get; private set; }
		public static ConfigWrapper<string> statSheetStats { get; private set; }
		public static ConfigWrapper<string> statSheetStatsNames { get; private set; }
		public static ConfigWrapper<int> titleFontSize { get; private set; }
		public static ConfigWrapper<int> descriptionFontSize { get; private set; }
		public static ConfigWrapper<int> x_position_notification { get; private set; }
		public static ConfigWrapper<int> Y { get; private set; }
		public static ConfigWrapper<int> Width { get; private set; }
		public static ConfigWrapper<int> Height { get; private set; }
		public static ConfigWrapper<bool> Persistent { get; private set; }
        public static ConfigWrapper<bool> GatherFromAllPlayers { get; private set; }
        public static ConfigWrapper<ulong> FramesPerUpdate { get; private set; }
        public static ConfigWrapper<ulong> FramesPerRecord { get; private set; }
        public static ConfigWrapper<ulong> ServerUpdatesPerClientUpdate { get; private set; }

#pragma warning restore CS0618 // Type or member is oC:\Users\schum\source\repos\RoR2Stats\SharedStatsDisplay\StatsPullerAbstract.csbsolete

        public Notification Notification { get; set; }
		public CharacterBody CachedCharacterBody { get; set; }
		public string[] CachedStatSheetStats { get; set; }
		public string[] CachedStatSheetStatsNames { get; set; }

        private static ulong frames = 0;
        private static ulong updates = 0;

        public static StatsUpdateList gStatsUpdateList = new StatsUpdateList();
        private List<SharedStatsRecorder> recorderList = new List<SharedStatsRecorder> { };
        // TODO use dynamic linking here and have abstract type.
        private StatsPullerServer statsPuller = new StatsPullerServer();
        private StatsPullerClient statsPullerClient = new StatsPullerClient();
        private Networking networking = new Networking();

        public SharedStatsDisplay()
		{
			Config.ConfigReloaded += OnConfigReloaded;
			OnConfigReloaded(null, null);
            Debug.logger.logEnabled = true;
        }

		private void OnConfigReloaded(object sender, System.EventArgs e)
		{
			const string defaultTitle = "Shared Stats";
			title = Config.Wrap("Display", "Title", "Text to display for the title.", defaultTitle);

			const string defaultStatSheetStats = "totalKills,totalDamageDealt";
			statSheetStats = Config.Wrap("Display", "StatSheetStats", "A comma-separated list of stats to display based on StatSheet fields.", defaultStatSheetStats);
			CachedStatSheetStats = statSheetStats.Value.Split(',');

			const string defaultStatSheetStatsNames = "Kills,Damage Dealt";
			statSheetStatsNames = Config.Wrap("Display", "StatSheetStatsNames", "A comma-separated list of names for the StatSheet stats.", defaultStatSheetStatsNames);
			CachedStatSheetStatsNames = statSheetStatsNames.Value.Split(',');

			if (CachedStatSheetStats.Length != CachedStatSheetStatsNames.Length) Debug.Log($"Length of {nameof(statSheetStats)} and {nameof(statSheetStatsNames)} do not match.");

			const int defaultTitleFontSize = 16;
			titleFontSize = Config.Wrap("Display", "TitleFontSize", "The font size of the title.", defaultTitleFontSize);

			const int defaultDescriptionFontSize = 14;
			descriptionFontSize = Config.Wrap("Display", "DescriptionFontSize", "The font size of the description.", defaultDescriptionFontSize);

			const int defaultX = 5;
			x_position_notification = Config.Wrap("Display", "X", "The X position as percent of screen width of the stats display.", defaultX);

			const int defaultY = 35;
			Y = Config.Wrap("Display", "Y", "The Y position as percent of screen height of the stats display.", defaultY);

			const int defaultWidth = 200;
			Width = Config.Wrap("Display", "Width", "The width of the stats display.", defaultWidth);

			const int defaultHeight = 450;
			Height = Config.Wrap("Display", "Height", "The height of the stats display.", defaultHeight);

			const bool defaultPersistent = false;
			Persistent = Config.Wrap("Display", "Persistent", "Whether the stats display always shows or only on Info Screen.", defaultPersistent);

            const bool defaultGatherFromAllPlayers = true;
            GatherFromAllPlayers = Config.Wrap("Display", "AllPlayers", "Whether the stats are gathered from all players or just self.", defaultGatherFromAllPlayers);

            // With 30 FPS, game lasting 30 minutes, that is 30 * 60 * 30 frames = 54000
            const ulong defaultFramesPerUpdate = 30;
            FramesPerUpdate = Config.Wrap("Display", "FramesPerUpdate", "How many frames are required to pass before a new update is gathered.", defaultFramesPerUpdate);

            // With 30 FPS, game lasting 30 minutes, that is 30 * 60 * 30 frames = 54000
            const ulong defaultServerUpdatesPerClientUpdate = 1;
            ServerUpdatesPerClientUpdate = Config.Wrap("Display", "ServerUpdatesPerClientUpdate", "How many frames are required to pass before a new update is gathered.", defaultServerUpdatesPerClientUpdate);

            const ulong defaultUpdatesPerRecord = 30;
            FramesPerRecord = Config.Wrap("Display", "UpdatesPerRecord", "How many updates are required to be gathered before a new record is created.", defaultUpdatesPerRecord);

        }

        /* Awake() - Called during initialization of the game.
        */
        private void Awake()
		{
			Debug.Log("Loaded StatsDisplayMod");
            recorderList.Clear();

            // Initialize networking
            networking.Init();
        }

        /* Update()
         * Called on each frame of the game.
         * The most interesting entry point of the mod.
         * Determines if an update should be generated, updates the notification, 
         */
        private void Update()
        {
            DebugOnUpdate();
            frames += 1;
            if (!ShouldGatherUpdateByFrame(frames))
            {
                return;
            }

            // Chat.AddMessage($"### Gathering Update ####");

            GatherUpdate();
            InitNotification();
            updates += 1;

            if (ShouldNotifyClients(updates))
            {
                Networking.ServerSendStatsUpdate(gStatsUpdateList);
            }

            if (!ShouldCreateRecordByUpdates(updates))
            {
                return;
            }

            // gStatsUpdateList.RecordUpdates();
        }

        private bool ShouldGatherUpdateByFrame(ulong iFrames)
        {
            if (iFrames % FramesPerUpdate.Value == 0)
            {
                return true;
            }

            return false;
        }

        private bool ShouldCreateRecordByUpdates(ulong iUpdates)
        {
            if (iUpdates % FramesPerRecord.Value == 0)
            {
                return true;
            }

            return false;
        }

        private bool ShouldNotifyClients(ulong iUpdates)
        {
            if ((iUpdates % ServerUpdatesPerClientUpdate.Value == 0) && 
                (NetworkServer.active) &&
                (NetworkUser.readOnlyInstancesList.Count > 0) &&
                (Run.instance != null))
            {
                return true;
            }

            return false;
        }

        private void GatherUpdate()
        { 
            GetCharacterStats(frames, updates);
        }

        private void InitNotification()
        {
            LocalUser localUser = LocalUserManager.GetFirstLocalUser();
            if (localUser == null)
            {
                return;
            }

            if (CachedCharacterBody == null && localUser != null)
            {
                CachedCharacterBody = localUser.cachedBody;
            }

            if (Notification == null && CachedCharacterBody != null)
            {
                Notification = CachedCharacterBody.gameObject.AddComponent<Notification>();
                Notification.transform.SetParent(CachedCharacterBody.transform);
                Notification.SetPosition(new Vector3((float)(Screen.width * x_position_notification.Value / 100f), (float)(Screen.height * Y.Value / 100f), 0));
                Notification.GetTitle = () => title.Value;
                Notification.GetDescription = gStatsUpdateList.BuildUpdateString;
                Notification.GenericNotification.fadeTime = 1f;
                Notification.GenericNotification.duration = 86400f;
                Notification.SetSize(Width.Value, Height.Value);
                Notification.SetFontSize(Notification.GenericNotification.titleText, titleFontSize.Value);
                Notification.SetFontSize(Notification.GenericNotification.descriptionText, descriptionFontSize.Value);
            }

            if (CachedCharacterBody == null && Notification != null)
            {
                Destroy(Notification);
            }

            if (Notification != null && Notification.RootObject != null)
            {
                if (Persistent.Value || (localUser != null && localUser.inputPlayer != null && localUser.inputPlayer.GetButton("info")))
                {
                    Notification.RootObject.SetActive(true);
                }
                else
                {
                    Notification.RootObject.SetActive(false);
                }
            }
        }

		private void OnSceneUnloaded(Scene scene)
		{
			CachedCharacterBody = null;

			if (Notification != null)
			{
				Destroy(Notification);
			}
		}

		public void GetCharacterStats(ulong iFrame, ulong iUpdate)
		{
            if (NetworkServer.active)
            {
                if ((NetworkUser.readOnlyInstancesList.Count) > 0 && 
                    (Run.instance != null))
                {
                    statsPuller.PullSurvivorStats(gStatsUpdateList, iFrame, iUpdate, GatherFromAllPlayers.Value);
                }
            }
            else
            {
                statsPullerClient.PullSurvivorStats(gStatsUpdateList, iFrame, iUpdate, GatherFromAllPlayers.Value);
            }
        }

        public void DebugOnUpdate()
        {
            if (frames % 100 == 0)
            {
                bool runActive = Run.instance != null;
                Debug.Log("DebugOnUpdate Frames: " + frames +
                          " Updates: " + updates +
                          " IsServer: " + NetworkServer.active +
                          " NetworkInstances: " + NetworkUser.readOnlyInstancesList.Count +
                          " RunActive: " + runActive);
            }
        }
    }
}
 