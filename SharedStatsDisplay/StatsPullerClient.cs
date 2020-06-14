namespace SharedStatsDisplay
{
    public class StatsPullerClient
    {
        public StatsUpdateList PullSurvivorStats(StatsUpdateList iList, ulong iFrame, ulong iUpdate, bool iGatherFromAllPlayers)
        {
            iList.Clear();

            // pull it from client cache
            foreach (StatsUpdate updateA in StatsUpdateNetworkComponent.updateListCache.statsUpdateList)
            {
                iList.Add(updateA);
            }

            return iList;
        }

        public StatsUpdateList PullSurvivorStats(ulong iFrame, ulong iUpdate, bool iGatherFromAllPlayers)
        {
            StatsUpdateList statsUpdateList = new StatsUpdateList {};
            return PullSurvivorStats(statsUpdateList, iFrame, iUpdate, iGatherFromAllPlayers);
        }
    }
}
