using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// SaveData를 JSON으로 저장하고 불러오는 간단한 시스템.
/// 파일 경로: Application.persistentDataPath/save.json
/// </summary>
public static class SaveSystem
{
    public static SaveData gameData = new SaveData();

    private static string FilePath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, "save.json");
            // Application.persistentDataPath : 내부 저장소의 경로. 유니티에서 제공하는 경로.
        }
    }
        
    public static void SaveVolume(float masterVolume, float bgmVolume, float sfxVolume)
    {
        gameData.masterVolume = masterVolume;
        gameData.bgmVolume = bgmVolume;
        gameData.sfxVolume = sfxVolume;

        string json = JsonUtility.ToJson(gameData, true); // 보기 좋게 들여쓰기
        File.WriteAllText(FilePath, json);

        //BinaryFormatter rouletteBf = new BinaryFormatter();
        //FileStream rouletteFile = File.Open(FilePath, FileMode.Create);

        //rouletteBf.Serialize(rouletteFile, gameData);
        //rouletteFile.Close();

        Debug.Log("저장 완료: " + FilePath);
    }

    /// <summary>파일이 있으면 SaveData를 반환, 없으면 null.</summary>
    public static SaveData Load()
    {
        if (File.Exists(FilePath) == false)
        {
            return null;
        }

        string json = File.ReadAllText(FilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Open(FilePath, FileMode.Open);
        //SaveData saveData = (SaveData)bf.Deserialize(file);
        //file.Close();

        return data;
    }

    /// <summary>저장 파일 삭제.</summary>
    public static void Delete()
    {
        if (File.Exists(FilePath) == true)
        {
            File.Delete(FilePath);
        }
    }
}
