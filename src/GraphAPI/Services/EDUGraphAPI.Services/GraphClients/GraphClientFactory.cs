namespace EDUGraphAPI.Services.GraphClients
{
    using Microsoft.Azure.ActiveDirectory.GraphClient;
    using Microsoft.Graph;

    public class GraphClientFactory : IGraphClientFactory
    {
        public IGraphClient CreateAADGraphClient(ActiveDirectoryClient activeDirectoryClient) => new AADGraphClient(activeDirectoryClient);

        public IGraphClient CreateMSGraphClient(GraphServiceClient graphServiceClient) => new MSGraphClient(graphServiceClient);
    }
}
