using System;
using System.Collections.Generic;
using System.Text;

namespace SharedStatsDisplay
{
    public class StatsUpdate
    {
        public int player { get; set; }
        public string identity { get; set; }
        public TimeSpan time { get; set; }
        public ulong damageDealt { get; set; }
        public ulong totalKills { get; set; }

        public StatsUpdate(int iPlayer, string iIdentity, TimeSpan iTime, ulong iDamageDealt, ulong iTotalKills)
        {
            player = iPlayer;
            identity = iIdentity;
            time = iTime;
            damageDealt = iDamageDealt;
            totalKills = iTotalKills;
        }

        public string ConvertToDisplayString(bool appendLine = true)
        {
            StringBuilder sb = new StringBuilder();
            if (appendLine)
            {
                sb.Append("Player: ").AppendLine(player.ToString());
                sb.Append("Identity: ").AppendLine(identity);
                sb.Append("Damage Dealt: ").AppendLine(damageDealt.ToString());
                sb.Append("Kills: ").AppendLine(totalKills.ToString());
            }
            else
            {
                sb.Append("Player: " + player.ToString() + " ");
                sb.Append("Username: " + identity + " ");
                sb.Append("Damage Dealt: " + damageDealt.ToString() + " ");
                sb.Append("Kills: " + totalKills.ToString() + " ");
            }
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is StatsUpdate update &&
                   player == update.player &&
                   identity == update.identity &&
                   time == update.time &&
                   damageDealt == update.damageDealt &&
                   totalKills == update.totalKills;
        }

        public override int GetHashCode()
        {
            int hashCode = 253648895;
            hashCode = hashCode * -1521134295 + player.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(identity);
            hashCode = hashCode * -1521134295 + time.GetHashCode();
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
