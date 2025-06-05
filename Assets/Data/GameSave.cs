using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;
using UnityEngine;

public static class GameSave
{
    private const string SaveName = "/saved_game.dat";

    public static void SaveGame(GameProgress progress)
    {
        var formatter = new BinaryFormatter();
        var fileLocation = GetSaveDataLocation();
        var fileStream = new FileStream(fileLocation, FileMode.OpenOrCreate);

        formatter.Serialize(fileStream, progress);
        fileStream.Close();
    }

    [CanBeNull]
    public static GameProgress LoadGame()
    {
        if (!HasSaveData()) return null;
        
        var savePath = GetSaveDataLocation();
            
        var formatter = new BinaryFormatter();
        var fileStream = new FileStream(savePath, FileMode.Open);

        var gameProgress = formatter.Deserialize(fileStream) as GameProgress;
        fileStream.Close();

        return gameProgress;
    }

    public static void DeleteSave()
    {
         var fileLocation = GetSaveDataLocation();

         if (HasSaveData())
             File.Delete(fileLocation);
    }
    
    public static bool HasSaveData() => File.Exists(GetSaveDataLocation());

    private static string GetSaveDataLocation()
        => Application.persistentDataPath + SaveName;
}
