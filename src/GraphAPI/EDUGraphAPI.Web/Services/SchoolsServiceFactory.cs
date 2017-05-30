namespace EDUGraphAPI.Web.Services
{
    using System;
    using System.Threading.Tasks;
    using EDUGraphAPI.Data;
    using EDUGraphAPI.Services.BingMaps;
    using EDUGraphAPI.Utils;

    public class SchoolsServiceFactory : ISchoolsServiceFactory
    {
        private readonly IBingMapService mapService;

        public SchoolsServiceFactory(IBingMapService mapService)
        {
            if (mapService == null)
            {
                throw new ArgumentNullException(nameof(mapService));
            }

            this.mapService = mapService;
        }

        public async Task<ISchoolsService> GetSchoolsServiceAsync(ApplicationDbContext db)
        {
            var educationServiceClient = await AuthenticationHelper.GetEducationServiceClientAsync();
            return new SchoolsService(educationServiceClient, this.mapService, db);
        }
    }
}
