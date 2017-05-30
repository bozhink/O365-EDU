namespace EDUGraphAPI.Services.GraphClients
{
    using Microsoft.Azure.ActiveDirectory.GraphClient;
    using Microsoft.Graph;

    public interface IGraphClientFactory
    {
        IGraphClient CreateAADGraphClient(ActiveDirectoryClient activeDirectoryClient);

        IGraphClient CreateMSGraphClient(GraphServiceClient graphServiceClient);
    }
}
