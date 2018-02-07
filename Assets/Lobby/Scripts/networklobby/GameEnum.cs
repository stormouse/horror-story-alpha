namespace GameEnum
{

    public enum TeamType
    {
        Spectator = 0,
        Hunter = 1,
        Survivor = 2
    }

    public enum PlayerType
    {
        Human,
        AI
    }

    public enum CharacterState
    {
        Alive,
        Dead
    }

    public enum HealthState
    {
        Normal = 0,
        Immobilized = 1,
        Blinded = 2        // Stunned = 3 (== Immobilized | Blinded)
    }

}