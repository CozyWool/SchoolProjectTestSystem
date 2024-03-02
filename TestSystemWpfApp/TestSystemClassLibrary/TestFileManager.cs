using System.IO;
using Newtonsoft.Json;
using TestSystemClassLibrary.Models;

namespace TestSystemClassLibrary;

public static class TestFileManager
{
    public static bool Save(Test test, string savePath)
    {
        try
        {
            FileInfo fileInfo = new(savePath);

            var json = JsonConvert.SerializeObject(test, Formatting.Indented);
            using var sw = new StreamWriter(savePath);
            sw.WriteLine(json);
        }
        catch (Exception)
        {
            return false;
        }
        
        return true;
    }
    public static Test Load(string testPath)
    {
        using var sr = new StreamReader(testPath);
        var json = sr.ReadToEnd();
        var deserialized = JsonConvert.DeserializeObject<Test>(json);
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
