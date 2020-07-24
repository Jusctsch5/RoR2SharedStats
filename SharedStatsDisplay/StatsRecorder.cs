namespace SharedStatsDisplay
{
    using EntityStates.Vulture;
    using RoR2;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using UnityEngine;

    public class StatsRecorder
    {
        private List<StatsUpdateList> recorderList = new List<StatsUpdateList> { };
		public DateTime gameStartTime { get; set; }
		static private string recorderPath = Application.dataPath + "/RunReports/";
		public void ReInit(DateTime iGameStartTime)
        {
			gameStartTime = iGameStartTime;
			recorderList.Clear();
		}

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

		public string GenerateHeader()
        {
			return recorderList.First().BuildHeaderString() + +'\n';
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

		private void GenerateDamageReport(string iRecorderPath, Guid iGuid)
        {
			string filename;
			filename = iRecorderPath + "/" + "damageReport_" + iGuid.ToString("N") + ".csv";
			using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filename))
			{
				string header = GenerateHeader();
				file.WriteLine(header);
				string damage = this.ConvertDamageToString();
				file.WriteLine(damage);
			}
		}

		private void GenerateKillsReport(string iRecorderPath, Guid iGuid)
		{
			string filename;
			filename = iRecorderPath + "/" + "killsReport_" + iGuid.ToString("N") + ".csv";
			using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filename))
			{
				string header = GenerateHeader();
				file.WriteLine(header);
				string kills = this.ConvertKillsToString();
				file.WriteLine(kills);
			}
		}

		public void GenerateStatsRecord()
		{
			Debug.Log("GenerateStatsRecord");

			if (!Directory.Exists(recorderPath))
			{
				Directory.CreateDirectory(recorderPath);
			}

			Guid myGUID = System.Guid.NewGuid();
			GenerateDamageReport(recorderPath, myGUID);
			GenerateKillsReport(recorderPath, myGUID);
		}
    }
}
