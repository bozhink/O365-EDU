namespace EDUGraphAPI.Services.DataSync
{
    using System.Threading.Tasks;

    public interface IUserSyncService
    {
        Task SyncAsync();
    }
}
