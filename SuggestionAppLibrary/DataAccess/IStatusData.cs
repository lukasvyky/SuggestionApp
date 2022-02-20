namespace SuggestionAppLibrary.DataAccess;

public interface IStatusData
{
   Task CreateStatus(Status status);
   Task<List<Status>> GetStatuses();
}