using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;

namespace SharedStatsDisplay.Tests
{
    [TestClass()]
    public class TestStatsPuller
    {
        [TestMethod()]
        public void Success()
        {
        }

        [TestMethod()]
        public void PullSurvivorStatsTest()
        {
            // Do a simple pull from a new list. Expect success.
            StatsPullerTest statsPuller = new StatsPullerTest();
            statsPuller.statsList = new StatsUpdateList();
            statsPuller.statsList.Add(new StatsUpdate(1, "Player 1", 1, 1, 11, 111));
            statsPuller.statsList.Add(new StatsUpdate(2, "Player 2", 1, 1, 12, 112));
            statsPuller.statsList.Add(new StatsUpdate(3, "Player 3", 1, 1, 13, 113));
            statsPuller.statsList.Add(new StatsUpdate(4, "Player 4", 1, 1, 14, 114));

            StatsUpdateList newStats = statsPuller.PullSurvivorStats(1, 1, true);
            if (statsPuller.CompareStats(newStats) == false)
            {
                Console.WriteLine("Stats don't compare. Old:" + statsPuller.statsList.BuildUpdateString() +
                                                      " New:" + newStats.BuildUpdateString());
                Assert.Fail();
            }

            // Modify the list, pull, and compare. Expect success.
            statsPuller.statsList.Clear();
            statsPuller.statsList.Add(new StatsUpdate(1, "Player 1", 1, 1, 21, 121));
            statsPuller.statsList.Add(new StatsUpdate(2, "Player 2", 1, 1, 22, 122));
            statsPuller.statsList.Add(new StatsUpdate(3, "Player 3", 1, 1, 23, 123));
            statsPuller.statsList.Add(new StatsUpdate(4, "Player 4", 1, 1, 24, 124));

            newStats = statsPuller.PullSurvivorStats(1, 1, true);
            if (statsPuller.CompareStats(newStats) == false)
            {
                Console.WriteLine("Stats don't compare. Old:" + statsPuller.statsList.BuildUpdateString() +
                                                      " New:" + newStats.BuildUpdateString());
                Assert.Fail();
            }

            // Modify the list and compare. Expect failure
            statsPuller.statsList.Add(new StatsUpdate(1, "Player 5", 1, 1, 21, 121));
            if (statsPuller.CompareStats(newStats) == true)
            {
                Console.WriteLine("Stats compare and they shouldn't");
                Assert.Fail();
            }
        }
    }
}