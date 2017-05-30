namespace EDUGraphAPI.Services.DifferentialQuery
{
    using EDUGraphAPI.Services.Models.DifferentialQuery;
    using System.Threading.Tasks;

    public interface IDifferentialQueryService
    {
        Task<DeltaResult<Delta<TEntity>>> QueryAsync<TEntity>(string url, string apiVersion) where TEntity : class;
    }
}
