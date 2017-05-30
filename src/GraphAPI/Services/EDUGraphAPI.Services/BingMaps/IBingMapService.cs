namespace EDUGraphAPI.Services.BingMaps
{
    using System.Threading.Tasks;

    public interface IBingMapService
    {
        Task<string[]> GetLongitudeAndLatitudeByAddressAsync(string address);
    }
}
