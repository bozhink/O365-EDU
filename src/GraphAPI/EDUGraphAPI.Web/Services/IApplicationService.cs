namespace EDUGraphAPI.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using EDUGraphAPI.Data.Models;
    using EDUGraphAPI.Services.Models.GraphClients;
    using EDUGraphAPI.Web.Models;

    public interface IApplicationService
    {
        Task CreateOrUpdateOrganizationAsync(TenantInfo tenant, bool adminConsented);

        Task<AdminContext> GetAdminContextAsync();

        ApplicationUser GetCurrentUser();

        Task<ApplicationUser> GetCurrentUserAsync();

        Task<ApplicationUser[]> GetLinkedUsers(Expression<Func<ApplicationUser, bool>> predicate = null);

        Task<ApplicationUser> GetUserAsync(string id);

        Task<ApplicationUser> GetUserByEmailAsync(string email);

        UserContext GetUserContext();

        Task<UserContext> GetUserContextAsync();

        Task<bool> IsO365AccountLinkedAsync(string o365UserId);

        Task<int> SaveSeatingArrangements(List<SeatingViewModel> seatingArrangements);

        Task UnlinkAccountsAsync(string id);

        Task UnlinkAllAccounts(string tenantId);

        Task UpdateLocalUserAsync(ApplicationUser localUser, UserInfo o365User, TenantInfo tenant);

        Task UpdateOrganizationAsync(string tenantId, bool adminConsented);

        void UpdateUserFavoriteColor(string color);
    }
}
