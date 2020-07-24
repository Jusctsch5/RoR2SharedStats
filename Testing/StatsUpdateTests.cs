using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SharedStatsDisplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedStatsDisplay.Tests
{
    [TestClass()]
    public class StatsUpdateTests
    {
        [TestMethod()]
        public void StatsUpdateTestSerializeJson()
        {
            DateTime now = DateTime.Now;
            DateTime then = DateTime.Now;
            then.AddMinutes(10);
            TimeSpan span = then - now;

            StatsUpdate stat = new StatsUpdate(1, "Player 1", span, 11, 111);
            string json = JsonConvert.SerializeObject(stat, Formatting.Indented);
            Assert.IsTrue(json.Length > 0);
            StatsUpdate statNew = JsonConvert.DeserializeObject<StatsUpdate>(json);
            if (stat != statNew)
            {
                Console.WriteLine("Old stats:\n" + stat.ConvertToDisplayString(false) +
                                  "\n not equal to \nNew stats:\n" + statNew.ConvertToDisplayString(false) +
                                  "\nJson:\n" + json);
                Assert.Fail();
            }

            StatsUpdateList statsList = new StatsUpdateList();
            statsList.Add(stat);
            statsList.Add(new StatsUpdate(2, "Player 2", span, 12, 112));
            statsList.Add(new StatsUpdate(3, "Player 3", span, 13, 113));
            statsList.Add(new StatsUpdate(4, "Player 4", span, 14, 114));

            json = JsonConvert.SerializeObject(statsList, Formatting.Indented);
            Assert.IsTrue(json.Length > 0);
            StatsUpdateList statsListNew = JsonConvert.DeserializeObject<StatsUpdateList>(json);
            
            if (!statsList.Equals(statsListNew))
            {
                Console.WriteLine("Old stats:\n" + statsList.BuildUpdateStringOneLine() +
                                  "\n not equal to \nNew stats:\n" + statsListNew.BuildUpdateStringOneLine() +
                                  " Json:" + json);
                Assert.Fail();
            }
        }
    }
}