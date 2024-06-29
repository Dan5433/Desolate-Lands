using CustomExtensions;
using System;
using System.IO;
using System.Net.Http.Headers;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


public class JSONFileDataHandler
{
    string dataDirPath = "";
    string dataFilePath = "";
    static string seed = "";

    public JSONFileDataHandler(string dataDirPath, string dataFilePath)
    {
        this.dataDirPath = dataDirPath;
        this.dataFilePath = dataFilePath;

        seed = PlayerPrefs.GetInt(GameManager.Instance.WorldName).ToString();
    }

    public T LoadData<T>()
    {
        string fullPath = Path.Combine(dataDirPath, dataFilePath);
        T deserializedData = default;

        if (File.Exists(fullPath))
        {
            try
            {
                string data = "";
                using (FileStream stream = new(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        data = reader.ReadToEnd();
                    }
                }

                data = EncryptDecrypt(data);
                deserializedData = JsonUtility.FromJson<T>(data);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error when trying to load data from file: " + fullPath + "\n" + ex);
            }
        }

        return deserializedData;
    }

    public void SaveData(object data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFilePath);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string serializedData = JsonUtility.ToJson(data);
            serializedData = EncryptDecrypt(serializedData);

            using (FileStream stream = new(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
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

    static string EncryptDecrypt(string data)
    {
        string encryptedData = "";

        for (int i = 0; i < data.Length; i++)
        {
            encryptedData += (char)(data[i] ^ seed[i % seed.Length]);
        }

        return encryptedData;
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