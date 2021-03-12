using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Core.Common.Extensions.String;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Logger = UnityLogger.Logger;

namespace Core.Systems
{
    public interface IStorable
    {
        string FileName { get; }
    }

    /// <summary>
    /// Service helps with serializing / deserializing data.
    /// </summary>
    public class PersistentDataSystem : CoreSystem
    {
        [SerializeField]
        private string persistentDataDirectoryName = "PersistentData";

        [SerializeField]
        private string dataFileExtension = ".core";

        private string _dataDirectory;

        public override void Initialize()
        {
            base.Initialize();

            //Cache this as Application.persistentDataPath cannot be accessed from a thread.
            _dataDirectory = $"{Application.persistentDataPath}/{persistentDataDirectoryName}";

            if (!Directory.Exists(_dataDirectory))
                Directory.CreateDirectory(_dataDirectory);
        }

        /// <summary>
        /// Saves data into a file.
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void Save<T>(T data) where T : IStorable
        {
            //TODO: improve this with async https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/using-async-for-file-access
            if (data is MonoBehaviour)
                throw new Exception($"Persistent Data Service: Monobehaviours cannot be serialized. Aborting.");

            var filename = data.FileName + dataFileExtension;
            var file = File.Open(_dataDirectory + "/" + filename, FileMode.Create);

            try
            {
                var bf = new BinaryFormatter();
                bf.Serialize(file, data); //todo: need to find an async way of doing this
                file.Flush();
                file.Close();
                Logger.Log($"Persistent Data Service: Saving to - {_dataDirectory + "/" + filename}", Colors.LightPink);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async UniTask<T> Load<T>() where T : IStorable
        {
            return await Load<T>(typeof(T).Name);
        }

        /// <summary>
        /// Loads serialized data into an object
        /// </summary>
        /// <param name="filename"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private async UniTask<T> Load<T>(string filename) where T : IStorable
        {
            filename += dataFileExtension;
            if (!File.Exists(_dataDirectory + "/" + filename))
                throw new Exception($"Persistent Data Service: File {_dataDirectory + "/" + filename} does not exists.");

            var file = File.Open(_dataDirectory + "/" + filename, FileMode.Open);
            try
            {
                var bf = new BinaryFormatter();
                var data = (T) bf.Deserialize(file); //todo: need to find an async way of doing this

                Logger.Log($"Persistent Data Service: Reading from - {_dataDirectory + "/" + filename}", Colors.LightPink);

                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}