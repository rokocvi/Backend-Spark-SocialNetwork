using Spark.API.DTOs.Match;

namespace Spark.API.Interfaces
{
    public interface IMatchService
    {
        Task<List<MatchResponseDto>> GetMyMatches(Guid userId);
        Task<MatchResponseDto?> GetMatchById(Guid matchId, Guid userId);
        Task RunDailyMatching();
        Task<MatchResponseDto> SaveMatch(Guid matchId, Guid userId);

        Task<List<MatchResponseDto>> GetMatchHistory(Guid userId);
    }
}
