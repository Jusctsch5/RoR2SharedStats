using System;
using System.Collections.Generic;
using System.Text;
using RoR2;

namespace SharedStatsDisplay
{
    public class StatsPullerTest
    {
        public StatsUpdateList statsList;
        public DateTime timeZero;
        public DateTime timeNew;


        public StatsUpdateList PullSurvivorStats(StatsUpdateList iList, bool iGatherFromAllPlayers)
        {
            iList.Clear();

            DateTime now = DateTime.Now;
            DateTime then = DateTime.Now;

            TimeSpan span = now - then;
            foreach (StatsUpdate update in statsList.statsUpdateList)
            {
                
                StatsUpdate newUpdate = new StatsUpdate(update.player, update.identity, span, update.damageDealt, update.totalKills);
                iList.Add(newUpdate);
            }
            return iList;
        }

        public StatsUpdateList PullSurvivorStats(bool iGatherFromAllPlayers)
        {
            StatsUpdateList newStatsList = new StatsUpdateList();
            return (PullSurvivorStats(newStatsList, iGatherFromAllPlayers));
        }
        public bool CompareStats(StatsUpdateList iStatsList)
        {
            return (statsList.Equals(iStatsList));
        }
    }
}
