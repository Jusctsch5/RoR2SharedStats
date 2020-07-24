using System.Collections.Generic;
using System.Text;
using RoR2;

namespace SharedStatsDisplay
{
    public interface StatsPullerAbstract
    {
        public abstract StatsUpdateList PullSurvivorStats(StatsUpdateList iList, bool iGatherFromAllPlayers);
        public abstract StatsUpdateList PullSurvivorStats(bool iGatherFromAllPlayers);
    }
}
