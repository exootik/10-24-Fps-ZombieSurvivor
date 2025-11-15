using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public static class Save
{
    public static bool LocalSaveData<T>([NotNull] string fileName, T objectToWrite) where T : new()
    {
        var path = Path.Combine(Application.persistentDataPath, fileName);
        var fs = File.Create(path);
        fs.Close();

        TextWriter writer = null;
        try //?
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };
            var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, settings);
            writer = new StreamWriter(path);
            writer.Write(contentsToWriteToFile);
        }
        finally
        {
            writer?.Close();
        }

        return true;
    }


    public static bool LocalLoadData<T>([NotNull] string fileName, out T result) where T : new()
    {
        var path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogException(new FileNotFoundException());
            result = new T();
            return false;
        }

        TextReader reader = null;
        try
        {
            reader = new StreamReader(path);
            var fileContents = reader.ReadToEnd();
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };
            var temp = JsonConvert.DeserializeObject<T>(fileContents, settings);
            if (temp == null)
            {
                result = new T();
                return false;
            }

            result = temp;
            return true;
        }
        finally
        {
            reader?.Close();
        }
    }
}