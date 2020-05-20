namespace Kingo.MicroServices
{

    public interface IDataContractSerializer
    {
        DataContractBlob Serialize(object content);

        object Deserialize(DataContractBlob blob, bool updateToLatestVersion = false);
    }
}
