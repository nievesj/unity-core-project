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

    /// <summary>
    /// Service helps with serializing / deserializing data.
    /// </summary>
    public class PersistentDataService : Service
    {
        private readonly PersistentDataServiceConfiguration _configuration;
        private string _dataDirectory;

        public PersistentDataService(ServiceConfiguration config)
        {
            _configuration = config as PersistentDataServiceConfiguration;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            //Cache this as Application.persistentDataPath cannot be accessed from a thread.
            _dataDirectory = $"{Application.persistentDataPath}/{_configuration.PersistentDataDirectoryName}";

            if (!Directory.Exists(_dataDirectory))
                Directory.CreateDirectory(_dataDirectory);
        }

        /// <summary>
        /// Saves data into a file.
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task Save<T>(T data) where T : IStorable
        {
            await Task.Run(() => 
            { 
                if (data is MonoBehaviour)
                {
                    Debug.LogError($"Persistent Data Service: Monobehaviours cannot be serialized. Aborting.".Colored(Colors.LightPink));
                    return;
                }
            
                var filename = data.FileName + _configuration.DataFileExtension;

                try
                {
                    using (var file = File.Open(_dataDirectory + "/" + filename, FileMode.Create))
                    {
                        var bf = new BinaryFormatter();
                        bf.Serialize(file, data); //todo: need to find an async way of doing this

                        file.Flush();
                        file.Close();

                        Debug.Log($"Persistent Data Service: Saving to - {_dataDirectory + "/" + filename}".Colored(Colors.LightPink));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Persistent Data Service: error saving to - {_dataDirectory + "/" + filename}, {e.Message}");
                }
            });
        }

        public async Task<T> Load<T>() where T : IStorable
        {
            return await Load<T>(typeof(T).Name);
        }

        /// <summary>
        /// Loads serialized data into an object
        /// </summary>
        /// <param name="filename"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> Load<T>(string filename) where T : IStorable
        {
            return await Task.Run(() => 
            { 
                filename += _configuration.DataFileExtension;
                if (!File.Exists(_dataDirectory + "/" + filename))
                {
                    Debug.LogError($"Persistent Data Service: File {_dataDirectory + "/" + filename} does not exists.");
                    return default(T);
                }

                try
                {
                    using (var file = File.Open(_dataDirectory + "/" + filename, FileMode.Open))
                    {
                        var bf = new BinaryFormatter();
                        var data = (T) bf.Deserialize(file); //todo: need to find an async way of doing this
                        file.Close();

                        Debug.Log($"Persistent Data Service: Reading from - {_dataDirectory + "/" + filename}".Colored(Colors.LightPink));
                    
                        return data;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Persistent Data Service: Error - {e.Message}");
                }
            
                return default(T);
            });
        }
    }
}