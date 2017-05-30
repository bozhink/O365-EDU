namespace EDUGraphAPI.Services.DifferentialQuery
{
    using System;
    using System.Threading.Tasks;

    public class DifferentialQueryServiceFactory : IDifferentialQueryServiceFactory
    {
        public IDifferentialQueryService CreateDifferentialQueryService(Func<Task<string>> accessTokenGetter) => new DifferentialQueryService(accessTokenGetter);
    }
}
