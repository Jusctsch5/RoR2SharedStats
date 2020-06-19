using System.Collections.Generic;
using System.Text;
using RoR2;

namespace SharedStatsDisplay
{
    public interface StatsPullerAbstract
    {
        public abstract StatsUpdateList PullSurvivorStats(StatsUpdateList iList, ulong iFrame, ulong iUpdate, bool iGatherFromAllPlayers);
        public abstract StatsUpdateList PullSurvivorStats(ulong iFrame, ulong iUpdate, bool iGatherFromAllPlayers);
    }
}
