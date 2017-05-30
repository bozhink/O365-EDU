namespace Microsoft.Education.Services
{
    using System.Threading.Tasks;
    using Microsoft.Education.Services.Models;

    public interface IEducationServiceClient
    {
        Task<ArrayResult<Section>> GetAllSectionsAsync(string schoolId, int top, string nextLink);

        Task<ArrayResult<SectionUser>> GetMembersAsync(string objectId, int top, string nextLink);

        Task<Section[]> GetMySectionsAsync(bool loadMembers = false);

        Task<Section[]> GetMySectionsAsync(string schoolId);

        Task<School> GetSchoolAsync(string objectId);

        Task<School[]> GetSchoolsAsync();

        Task<Section> GetSectionAsync(string sectionId);

        Task<Student> GetStudentAsync();

        Task<ArrayResult<SectionUser>> GetStudentsAsync(string schoolId, int top, string nextLink);

        Task<Teacher> GetTeacherAsync();

        Task<ArrayResult<SectionUser>> GetTeachersAsync(string schoolId, int top, string nextLink);
    }
}
