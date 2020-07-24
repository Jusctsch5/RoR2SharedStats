using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Stats;

namespace SharedStatsDisplay
{
    public class StatsPullerServer
    {

        public DateTime timeStart;

        public StatsUpdateList PullSurvivorStats(StatsUpdateList iList, bool iGatherFromAllPlayers)
        {
            iList.Clear();
            RunReport runReport = RunReport.Generate(Run.instance, GameResultType.Unknown);
            List<RunReport.PlayerInfo> playerInfoList = new List<RunReport.PlayerInfo> { };
            DateTime now = DateTime.Now;
            TimeSpan delta = now - timeStart;
            for (int i = 0; i < runReport.playerInfoCount; i++)
            {
                if ((iGatherFromAllPlayers) ||
                    (runReport.GetPlayerInfo(i).isLocalPlayer))
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

        public StatsUpdateList PullSurvivorStats(bool iGatherFromAllPlayers)
        {
            StatsUpdateList statsUpdateList = new StatsUpdateList { };
            return PullSurvivorStats(statsUpdateList, iGatherFromAllPlayers);
        }
    }
}
