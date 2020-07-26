using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedStatsDisplay
{
    [Serializable]

    public class StatsUpdateList
    {
        public List<StatsUpdate> statsUpdateList = new List<StatsUpdate> { };

        public string BuildHeaderString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Time,");
            foreach (StatsUpdate update in statsUpdateList)
            {
                sb.Append(update.identity + ",");
            }
            sb = sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        public string BuildUpdateString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (StatsUpdate update in statsUpdateList)
            {
                sb.Append(update.ConvertToDisplayString(true));
            }

            return sb.ToString();
        }

        public string BuildUpdateStringOneLine()
        {
            StringBuilder sb = new StringBuilder();
            foreach (StatsUpdate update in statsUpdateList)
            {
                sb.Append(update.ConvertToDisplayString(false));
            }

            return sb.ToString();
        }

        public string BuildExcelStringDamage()
        {
            StringBuilder sb = new StringBuilder();
            if (statsUpdateList.Count() > 0)
            {
                TimeSpan span = statsUpdateList.First().time;
                sb.Append(string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds) + ",");
                foreach (StatsUpdate update in statsUpdateList)
                {
                    sb.Append(update.damageDealt + ",");
                }
                sb = sb.Remove(sb.Length - 1, 1);
            }
                        
            return sb.ToString();
        }

        public string BuildExcelStringKills()
        {
            StringBuilder sb = new StringBuilder();
            if (statsUpdateList.Count() > 0)
            {
                TimeSpan span = statsUpdateList.First().time;
                sb.Append(string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds) + ",");
                foreach (StatsUpdate update in statsUpdateList)
                {
                    sb.Append(update.totalKills + ",");
                }
                sb = sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        public void Add(StatsUpdate update)
        {
            statsUpdateList.Add(update);
        }

        public void Clear()
        {
            statsUpdateList.Clear();
        }

        public void RecordUpdates(StatsRecorder recorder)
        {
            recorder.RecordUpdates(this);
        }

        public bool Equals(StatsUpdateList statsList)
        {

            if (statsUpdateList.Count != statsList.statsUpdateList.Count)
            { 
                return false;
            }

            int matches = statsList.statsUpdateList.Count;
            foreach (StatsUpdate updateA in statsUpdateList)
            {
                foreach (StatsUpdate updateB in statsList.statsUpdateList)
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
