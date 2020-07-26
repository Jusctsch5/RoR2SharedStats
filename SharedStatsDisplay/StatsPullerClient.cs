using RoR2;
using RoR2.Stats;
using System;
using System.Collections.Generic;

namespace SharedStatsDisplay
{
    public class StatsPullerClient
    {
        int client_mode_counter = 0;
        static int client_mode_counter_max = 10;
        bool client_mode_enabled = false;
        public System.DateTime timeStart;

        public StatsUpdateList PullSurvivorStats(StatsUpdateList iList, bool iGatherFromAllPlayers)
        {
            iList.Clear();

            // If the server hasn't sent stats for a period of time, 
            // assume that he doesn't have the mod installed and start tracking locally.
            if (StatsUpdateNetworkComponent.updateListCache.statsUpdateList.Count == 0)
            {
                client_mode_counter++;
                if (client_mode_counter > client_mode_counter_max)
                {
                    client_mode_enabled = true;
                } 
                else
                {
                    return iList;
                }
                
            }
            if (client_mode_enabled)
            {
                return PullSurvivorStatsClientMode(iList);
            }

            // pull it from client cache  
            client_mode_counter = 0;
            client_mode_enabled = false;
            foreach (StatsUpdate updateA in StatsUpdateNetworkComponent.updateListCache.statsUpdateList)
            {
                iList.Add(updateA);
            }

            return iList;
        }

        public StatsUpdateList PullSurvivorStats(bool iGatherFromAllPlayers)
        {
            StatsUpdateList statsUpdateList = new StatsUpdateList{};
            return PullSurvivorStats(statsUpdateList, iGatherFromAllPlayers);
        }

        public StatsUpdateList PullSurvivorStatsClientMode(StatsUpdateList iList)
        {
            iList.Clear();
            RunReport runReport = RunReport.Generate(Run.instance, GameResultType.Unknown);
            List<RunReport.PlayerInfo> playerInfoList = new List<RunReport.PlayerInfo> { };
            DateTime now = DateTime.Now;
            TimeSpan delta = now - timeStart;

            for (int i = 0; i < runReport.playerInfoCount; i++)
            {
                if ((runReport.GetPlayerInfo(i).isLocalPlayer))
                {
                    RunReport.PlayerInfo playerInfo = runReport.GetPlayerInfo(i);
                    ulong totalDamageDealt = playerInfo.statSheet.GetStatValueULong(StatDef.totalDamageDealt) +
                                                playerInfo.statSheet.GetStatValueULong(StatDef.totalMinionDamageDealt);


                    // The server doesn't have access to playerInfo.networkUser.userName, playerInfo.name, or playerInfo.networkUser.GetNetworkPlayerName().GetResolvedName();

                    string nameIdentifier = playerInfo.bodyName;

                    StatsUpdate update = new StatsUpdate(i + 1,
                                                         nameIdentifier,
                                                         delta,
                                                         totalDamageDealt,
                                                         playerInfo.statSheet.GetStatValueULong(StatDef.totalKills));
                    iList.Add(update);
                }
            }

            return iList;
        }
    }
}
