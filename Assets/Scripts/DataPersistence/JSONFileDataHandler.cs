using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

public class JsonFileDataHandler
{
    readonly string encyptionString;
    readonly string dataDirPath;
    readonly string dataFilePath;

    public JsonFileDataHandler(string dataDirPath, string dataFilePath) 
    {
        this.dataDirPath = dataDirPath;
        this.dataFilePath = dataFilePath;

        encyptionString = PlayerPrefs.GetInt(GameManager.Instance.WorldName).ToString();
    }

    public JsonFileDataHandler(string dataDirPath, string dataFilePath, string encyptionString)
    {
        this.dataDirPath = dataDirPath;
        this.dataFilePath = dataFilePath;

        this.encyptionString = encyptionString;
    }


    public async Task<T> LoadDataAsync<T>() where T : class
    {
        string fullPath = Path.Combine(dataDirPath, dataFilePath);
        T deserializedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string data = "";
                using (FileStream stream = new(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new(stream))
                    {
                        data = await reader.ReadToEndAsync();
                    }
                }

                data = await EncryptDecrypt(data);
                deserializedData = JsonUtility.FromJson<T>(data);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error when trying to load data from file: " + fullPath + "\n" + ex);
            }
        }

        return deserializedData;
    }

    public async Task SaveDataAsync(object data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFilePath);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string serializedData = JsonUtility.ToJson(data);
            serializedData = await EncryptDecrypt(serializedData);

            using (FileStream stream = new(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new(stream))
                {
                    writer.Write(serializedData);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error when trying to save data to file: " + fullPath + "\n" + ex);
        }
    }

    async Task<string> EncryptDecrypt(string data)
    {
        if(!GameManager.Instance.UseJsonEncryption) return data;

        StringBuilder encryptedData = new();

        await Task.Run(() =>
        {
            for (int i = 0; i < data.Length; i++)
            {
                encryptedData.Append((char)(data[i] ^ encyptionString[i % encyptionString.Length]));
            }
        });

        return encryptedData.ToString();
    }


    //struct LoadJob<T> : IJob where T : unmanaged
    //{
    //    public NativeArray<T> result; 
    //    public NativeArray<char> dataDirPath;
    //    public NativeArray<char> dataFilePath;
    //    public void Execute()
    //    {
    //        string fullPath = Path.Combine(new string(dataDirPath), new string(dataFilePath));
    //        dataDirPath.Dispose();
    //        dataFilePath.Dispose();

    //        if (File.Exists(fullPath))
    //        {
    //            try
    //            {
    //                string data = "";
    //                using (FileStream stream = new(fullPath, FileMode.Open))
    //                {
    //                    using (StreamReader reader = new StreamReader(stream))
    //                    {
    //                        data = reader.ReadToEnd();
    //                    }
    //                }

    //                data = EncryptDecrypt(data);
    //                result[0] = JsonUtility.FromJson<T>(data);
    //            }
    //            catch (Exception ex)
    //            {
    //                Debug.LogError("Error when trying to load data from file: " + fullPath + "\n" + ex);
    //            }
    //        }
    //    }
    //}

    //struct SaveJob : IJob
    //{
    //    public object data;
    //    public string dataDirPath;
    //    public string dataFilePath;

    //    public void Execute()
    //    {
    //        string fullPath = Path.Combine(dataDirPath, dataFilePath);
    //        try
    //        {
    //            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

    //            string serializedData = JsonUtility.ToJson(data);
    //            serializedData = EncryptDecrypt(serializedData);

    //            using (FileStream stream = new(fullPath, FileMode.Create))
    //            {
    //                using (StreamWriter writer = new StreamWriter(stream))
    //                {
    //                    writer.Write(serializedData);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.LogError("Error when trying to save data to file: " + fullPath + "\n" + ex);
    //        }
    //    }
    //}
}