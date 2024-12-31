using System;
using System.IO;
using UnityEngine;

public class BinaryDataHandler
{
    readonly string dataDirPath;
    readonly string dataFilePath;

    public BinaryDataHandler(string dataDirPath, string dataFilePath)
    {
        this.dataDirPath = dataDirPath;
        this.dataFilePath = dataFilePath;
    }

    public bool FileExists()
    {
        return File.Exists(Path.Combine(dataDirPath, dataFilePath));
    }

    public void SaveData(Action<BinaryWriter> writeAction, FileMode fileMode = FileMode.Create)
    {
        string fullPath = Path.Combine(dataDirPath, dataFilePath);
        if(!File.Exists(fullPath)) fileMode = FileMode.Create;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using var stream = new FileStream(fullPath, fileMode ,FileAccess.Write);
            using var writer = new BinaryWriter(stream);
            writeAction(writer);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error when trying to save data to file: {fullPath} \n {ex}");
        }
    }

    public void LoadData(Action<BinaryReader> readAction)
    {
        string fullPath = Path.Combine(dataDirPath, dataFilePath);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(stream);
            readAction(reader);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error when trying to load data from file: {fullPath} \n {ex}" );
        }
    }

    public void ModifyData(Action<BinaryReader, BinaryWriter> modifyAction)
    {
        string fullPath = Path.Combine(dataDirPath, dataFilePath);
        string tempPath = fullPath + ".tmp";

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (var reader = new BinaryReader(File.Open(fullPath, FileMode.Open)))
            {
                using var writer = new BinaryWriter(File.Open(tempPath, FileMode.Create));
                modifyAction(reader, writer);
            }

            //replace old file with the updated one
            File.Replace(tempPath, fullPath, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error when trying to load and save data from and to file: {fullPath} \n using temporary file: {tempPath} \n {ex}");
        }
    }
}