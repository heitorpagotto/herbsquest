using System;
using JetBrains.Annotations;

[Serializable]
public class GameProgress
{
    public int PlayerMaxHealth;
    public int[] UnlockedLevels;
    [CanBeNull] public int[] FinshedLevels;
    [CanBeNull] public int[] CompletedLevels;
    public int LastLevel;
}
