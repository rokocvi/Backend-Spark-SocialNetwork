using Spark.API.DTOs.Spark;

namespace Spark.API.Interfaces
{
    public interface ISparkService
    {
        Task<SparkResponseDto> CreateSpark(CreateSparkDto dto, Guid userId);
        Task<SparkResponseDto?> GetTodaySpark(Guid userId);

        Task<List<SparkResponseDto>> GetAllTodaySparks(string? tag = null);

        Task<IEnumerable<SparkResponseDto>> GetSparkHistory(Guid userId);

        Task<bool> DeleteSpark(Guid sparkId, Guid userId);
    }
}
