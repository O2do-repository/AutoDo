namespace AutoDo.RFPStorageComponent;

public interface IStorageComponent
{
    void SaveToFile<T>(T obj, string fileName);
    T LoadFromFile<T>(string fileName);
}