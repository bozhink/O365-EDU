namespace EDUGraphAPI.Services.DifferentialQuery
{
    using System;
    using System.Threading.Tasks;

    public interface IDifferentialQueryServiceFactory
    {
        IDifferentialQueryService CreateDifferentialQueryService(Func<Task<string>> accessTokenGetter);
    }
}
