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

    public class StatsUpdateList
    {
        public List<StatsUpdate> statsUpdateList = new List<StatsUpdate> { };

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

        public void Add(StatsUpdate update)
        {
            statsUpdateList.Add(update);
        }

        public void Clear()
        {
            statsUpdateList.Clear();
        }

        public void RecordUpdates(SharedStatsRecorder recorder)
        {
            foreach (StatsUpdate update in statsUpdateList)
            {
                recorder.RecordUpdate(update);
            }
        }
    }

    public class StatsUpdate
    {
        public int player { get; private set; }
        public string userName { get; private set; }
        public ulong frame { get; private set; }
        public ulong update { get; private set; }
        public ulong damageDealt { get; private set; }
        public ulong totalKills { get; private set; }

        public StatsUpdate(int iPlayer, string iUserName, ulong iFrame, ulong iUpdate, ulong iDamageDealt, ulong iTotalKills)
        {
            player = iPlayer;
            userName = iUserName;
            frame = iFrame;
            update = iUpdate;
            damageDealt = iDamageDealt;
            totalKills = iTotalKills;
        }

        public string ConvertToDisplayString(bool appendLine = true)
        {
            StringBuilder sb = new StringBuilder();
            if (appendLine)
            {
                sb.Append("Player: ").AppendLine(player.ToString());
                sb.Append("Username: ").AppendLine(userName);
                sb.Append("Damage Dealt: ").AppendLine(damageDealt.ToString());
                sb.Append("Kills: ").AppendLine(totalKills.ToString());
            }
            else
            {
                sb.Append("Player: " + player.ToString() + " ");
                sb.Append("Username: " + userName + " ");
                sb.Append("Damage Dealt: " + damageDealt.ToString() + " ");
                sb.Append("Kills: " + totalKills.ToString() + " ");
            }
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is StatsUpdate update &&
                   player == update.player &&
                   userName == update.userName &&
                   frame == update.frame &&
                   this.update == update.update &&
                   damageDealt == update.damageDealt &&
                   totalKills == update.totalKills;
        }

        public override int GetHashCode()
        {
            int hashCode = 253648895;
            hashCode = hashCode * -1521134295 + player.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(userName);
            hashCode = hashCode * -1521134295 + frame.GetHashCode();
            hashCode = hashCode * -1521134295 + update.GetHashCode();
            hashCode = hashCode * -1521134295 + damageDealt.GetHashCode();
            hashCode = hashCode * -1521134295 + totalKills.GetHashCode();
            return hashCode;
        }

        public static bool operator !=(StatsUpdate update1, StatsUpdate update2)
        {
            return !(update1 == update2);
        }

        public static bool operator ==(StatsUpdate update1, StatsUpdate update2)
        {
            if (object.ReferenceEquals(update1, null))
            {
                return object.ReferenceEquals(update2, null);
            }

            return update1.Equals(update2);
        }

    }
}
