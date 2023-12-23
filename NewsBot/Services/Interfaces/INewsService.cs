using NewsBot.Models.Base;

namespace NewsBot.Services.Interfaces
{
    public interface INewsService
    {
        public Task<ServiceResult> GetNewsById(int id,CancellationToken cancellationToken);
    }
}
