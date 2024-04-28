using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Runtime;
using AutoDo.StorageComponent;


namespace AutoDo.RFPStorageComponent
{
    public class StorageComponent : IStorageComponent
    {
        private readonly string _basePath;

        public StorageComponent(IOptions<StoragePathSettings> settings)
        {
            _basePath = settings.Value.ConnectionString;
        }
        public void SaveToFile<T>(T obj, string fileName)
        {
            string filePath = Path.Combine(_basePath, fileName);
            string json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(filePath, json);
        }

        public T LoadFromFile<T>(string fileName)
        {
            string filePath = Path.Combine(_basePath, fileName);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                throw new FileNotFoundException("File not found", filePath);
            }
        }
    }
}
