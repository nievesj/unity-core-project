using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Services.Data
{
    public interface IStorable
    {
        string FileName { get; }
    }

    public class PersistentDataService : Service
    {
        private readonly PersistentDataServiceConfiguration _configuration;
        private string DataFolder => $"{Application.persistentDataPath}/{_configuration.PersistentDataDirectoryName}";

        public PersistentDataService(ServiceConfiguration config)
        {
            _configuration = config as PersistentDataServiceConfiguration;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);
        }

        public async Task Save<T>(T data) where T : IStorable
        {
            if (data is MonoBehaviour)
            {
                Debug.LogError($"Persistent Data Service: Monobehaviours cannot be serialized. Aborting.".Colored(Colors.LightPink));
                return;
            }
            
            var filename = data.FileName + _configuration.DataFileExtension;

            try
            {
                using (var file = File.Open(DataFolder + "/" + filename, FileMode.Create))
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(file, data);

                    await file.FlushAsync();
                    file.Close();

                    Debug.Log($"Persistent Data Service: Saving to - {DataFolder + "/" + filename}".Colored(Colors.LightPink));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Persistent Data Service: error saving to - {DataFolder + "/" + filename}, {e.Message}");
            }
        }

        public async Task<T> Load<T>() where T : IStorable
        {
            return await Load<T>(typeof(T).Name);
        }

        public async Task<T> Load<T>(string filename) where T : IStorable
        {
            filename += _configuration.DataFileExtension;
            
            if (!File.Exists(DataFolder + "/" + filename))
            {
                Debug.LogError($"Persistent Data Service: File {DataFolder + "/" + filename} does not exists.");
                return default(T);
            }

            try
            {
                using (var file = File.Open(DataFolder + "/" + filename, FileMode.Open))
                {
                    var bf = new BinaryFormatter();
                    var retult = new byte[file.Length];

                    await file.ReadAsync(retult, 0, (int) file.Length);

                    file.Position = 0;
                     var data = (T) bf.Deserialize(file);

                    file.Close();

                    Debug.Log($"Persistent Data Service: Reading from - {DataFolder + "/" + filename}".Colored(Colors.LightPink));
                    
                    return data;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Persistent Data Service: Error - {e.Message}");
            }
            
            return default(T);
        }
    }
}