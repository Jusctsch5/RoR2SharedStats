using System;
using System.Collections.Generic;
using System.Text;
using RoR2;

namespace SharedStatsDisplay
{
    public class StatsPullerTest
    {
        public StatsUpdateList statsList;

        public StatsUpdateList PullSurvivorStats(StatsUpdateList iList, ulong iFrame, ulong iUpdate, bool iGatherFromAllPlayers)
        {
            iList.Clear();
            foreach (StatsUpdate update in statsList.statsUpdateList)
            {
                StatsUpdate newUpdate = new StatsUpdate(update.player, update.userName, iFrame, iUpdate, update.damageDealt, update.totalKills);
                iList.Add(newUpdate);
            }
            return iList;
        }

        public StatsUpdateList PullSurvivorStats(ulong iFrame, ulong iUpdate, bool iGatherFromAllPlayers)
        {
            StatsUpdateList newStatsList = new StatsUpdateList();
            return (PullSurvivorStats(newStatsList, iFrame, iUpdate, iGatherFromAllPlayers));
        }

        public bool CompareStats(StatsUpdateList iStatsList)
        {
            if (iStatsList.statsUpdateList.Count != statsList.statsUpdateList.Count)
            {
                return false;
            }

            int matches = iStatsList.statsUpdateList.Count;
            foreach (StatsUpdate updateA in statsList.statsUpdateList)
            {
                foreach (StatsUpdate updateB in iStatsList.statsUpdateList)
                {
                    if (updateA == updateB)
                    {
                        matches--;
                        break;
                    }
                }
            }
            if (matches != 0)
            {
                return false;
            }
            return true;
        }
    }
}
