namespace Crpg.Domain.Entities.Servers;

public enum GameMode
{
    CRPGBattle,
    CRPGConquest,
    CRPGDTV,
    CRPGDuel,
    CRPGSiege,
    CRPGTeamDeathmatch,
    CRPGSkirmish,
    CRPGUnknownGameMode,
}

public enum GameModeAlias
{
    A, // CRPGBattle
    B, // CRPGConquest
    C, // CRPGDuel
    D, // CRPGSkirmish
    E, // CRPGDTV
    Z, // CRPGUnknownGameMode
}
