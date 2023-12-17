using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.Core;

namespace Crpg.Module.GUI.HudExtension;
public class TeamsBannerAndNameModel
{
    public ImageIdentifierVM Banner1;
    public ImageIdentifierVM Banner2;
    public string Team1Name;
    public string Team2Name;
    public TeamsBannerAndNameModel(ImageIdentifierVM banner1, ImageIdentifierVM banner2, string team1Name, string team2Name)
    {
        Banner1 = banner1;
        Banner2 = banner2;
        Team1Name = team1Name;
        Team2Name = team2Name;
    }
}
