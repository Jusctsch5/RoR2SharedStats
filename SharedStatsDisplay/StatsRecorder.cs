namespace SharedStatsDisplay
{
    using EntityStates.Vulture;
    using RoR2;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;
    using UnityEngine;

    public class StatsRecorder
    {
        private List<StatsUpdateList> recorderList = new List<StatsUpdateList> { };

		static private string recorderPath = Application.dataPath + "/RunReports/";
		static private bool useUpdateInsteadOfFrame = true;

		public void RecordUpdates(StatsUpdateList updateList)
        {
			if (updateList.statsUpdateList.Count == 0)
            {
				return;
            }

			StatsUpdateList newUpdateList = new StatsUpdateList();
			foreach (StatsUpdate update in updateList.statsUpdateList)
			{
				newUpdateList.Add(update);
			}
			recorderList.Add(newUpdateList);
        }

		public string ConvertDamageToString()
        {
			// Do Total Damage
			StringBuilder sb = new StringBuilder();

			foreach (StatsUpdateList statsUpdateList in recorderList)
			{
				sb.Append(statsUpdateList.BuildExcelStringDamage() + '\n');
			}
			sb = sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
        }

		public string ConvertKillsToString()
		{
			// Do Total Damage
			StringBuilder sb = new StringBuilder();

			foreach (StatsUpdateList statsUpdateList in recorderList)
			{
				sb.Append(statsUpdateList.BuildExcelStringKills() + '\n');
			}
			sb = sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		public void GenerateStatsRecord()
		{
			Debug.Log("GenerateStatsRecord");

			if (!Directory.Exists(recorderPath))
			{
				Directory.CreateDirectory(recorderPath);
			} 

			string text;
			text = recorderPath + "/" + "damageReport.txt";
			using (System.IO.StreamWriter file = new System.IO.StreamWriter(@text))
			{
				string damage = this.ConvertDamageToString();
				file.WriteLine(damage);
			}

			text = recorderPath + "/" + "killsReport.txt";
			using (System.IO.StreamWriter file = new System.IO.StreamWriter(@text))
			{
				string kills = this.ConvertKillsToString();
				file.WriteLine(kills);
			}
		}
    }
}
