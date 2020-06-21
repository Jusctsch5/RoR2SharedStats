using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Stats;

namespace SharedStatsDisplay
{
    public class StatsPullerServer
    {
        public StatsUpdateList PullSurvivorStats(StatsUpdateList iList, ulong iFrame, ulong iUpdate, bool iGatherFromAllPlayers)
        {
            iList.Clear();
            RunReport runReport = RunReport.Generate(Run.instance, GameResultType.Unknown);
            List<RunReport.PlayerInfo> playerInfoList = new List<RunReport.PlayerInfo> { };

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
                                                         iFrame,
                                                         iUpdate,
                                                         totalDamageDealt,
                                                         playerInfo.statSheet.GetStatValueULong(StatDef.totalKills));
                    iList.Add(update);
                }
            }

            return iList;
        }

        public StatsUpdateList PullSurvivorStats(ulong iFrame, ulong iUpdate, bool iGatherFromAllPlayers)
        {
            StatsUpdateList statsUpdateList = new StatsUpdateList { };
            return PullSurvivorStats(statsUpdateList, iFrame, iUpdate, iGatherFromAllPlayers);
        }
    }
}
