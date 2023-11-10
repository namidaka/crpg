using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common;

internal class CrpgBattleScoreboardData : IScoreboardData
{
    public MissionScoreboardComponent.ScoreboardHeader[] GetScoreboardHeaders()
    {
        return new MissionScoreboardComponent.ScoreboardHeader[]
        {
            new("ping", missionPeer => TaleWorlds.Library.MathF.Round(missionPeer.GetNetworkPeer().AveragePingInMilliseconds).ToString(), _ => "BOT"),
            new("level", missionPeer => missionPeer.GetComponent<CrpgPeer>().User?.Character.Level.ToString() ?? string.Empty, _ => string.Empty),
            new("clan", missionPeer =>
                {
                    var crpgPeer = missionPeer.GetComponent<CrpgPeer>();
                    if (crpgPeer.Clan == null)
                    {
                        return string.Empty;
                    }

                    if (crpgPeer.Clan.Name.Length <= 10)
                    {
                        return crpgPeer.Clan.Name;
                    }
                    else
                    {
                        return crpgPeer.Clan.Tag;
                    }
                },
                _ => string.Empty),
            new("name", missionPeer => missionPeer.DisplayedName, _ => new TextObject("{=hvQSOi79}Bot").ToString()),
            new("kill", missionPeer => missionPeer.KillCount.ToString(), bot => bot.KillCount.ToString()),
            new("death", missionPeer => missionPeer.DeathCount.ToString(), bot => bot.DeathCount.ToString()),
            new("assist", missionPeer => missionPeer.AssistCount.ToString(), bot => bot.AssistCount.ToString()),
            //new("life", missionPeer => (missionPeer.ControlledAgent.Health + "/" + missionPeer.ControlledAgent.HealthLimit).ToString(), _ => string.Empty),
            new("score", missionPeer => missionPeer.Score.ToString(), bot => bot.Score.ToString()),
        };
    }
}
