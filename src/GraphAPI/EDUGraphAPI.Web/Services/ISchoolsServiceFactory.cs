namespace EDUGraphAPI.Web.Services
{
    using System.Threading.Tasks;
    using EDUGraphAPI.Data;

    public interface ISchoolsServiceFactory
    {
        Task<ISchoolsService> GetSchoolsServiceAsync(ApplicationDbContext db);
    }
}
