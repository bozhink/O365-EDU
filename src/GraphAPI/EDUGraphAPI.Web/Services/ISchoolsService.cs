namespace EDUGraphAPI.Web.Services
{
    using System.Threading.Tasks;
    using EDUGraphAPI.Web.Models;
    using EDUGraphAPI.Web.ViewModels;
    using Microsoft.Graph;

    public interface ISchoolsService
    {
        Task<string[]> GetMyClassesAsync();

        Task<SchoolUsersViewModel> GetSchoolStudentsAsync(string objectId, int top, string nextLink);

        Task<SchoolsViewModel> GetSchoolsViewModelAsync(UserContext userContext);

        Task<SchoolUsersViewModel> GetSchoolTeachersAsync(string objectId, int top, string nextLink);

        Task<SchoolUsersViewModel> GetSchoolUsersAsync(string objectId, int top);

        Task<SchoolUsersViewModel> GetSchoolUsersAsync(string objectId, int top, string nextLink);

        Task<SectionDetailsViewModel> GetSectionDetailsViewModelAsync(string schoolId, string classId, IGroupRequestBuilder group);

        Task<SectionsViewModel> GetSectionsViewModelAsync(UserContext userContext, string objectId, int top);

        Task<SectionsViewModel> GetSectionsViewModelAsync(UserContext userContext, string objectId, int top, string nextLink);
    }
}
