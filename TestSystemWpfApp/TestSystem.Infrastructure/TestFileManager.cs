using System.IO;
using Newtonsoft.Json;
using TestSystem.Dto;

namespace TestSystem.Infrastructure;

public static class TestFileManager
{
    public static bool Save(Quiz quiz, string filePath)
    {
        try
        {
            var json = JsonConvert.SerializeObject(quiz, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public static Quiz Load(string testPath)
    {
        using var sr = new StreamReader(testPath);
        var json = sr.ReadToEnd();
        var deserialized = JsonConvert.DeserializeObject<Quiz>(json);
        return deserialized;
    }

    public static bool Delete(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);

        return !File.Exists(filePath);
    }
    // public static void ExportWordToFile(DictionaryPart dictPart, string path)
    // {
    //     using var sw = new StreamWriter(path);
    //     var json = JsonConvert.SerializeObject(dictPart, Formatting.Indented, new StringEnumConverter());
    //     sw.WriteLine(json);
    // }
}