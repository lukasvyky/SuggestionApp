namespace SuggestionAppLibrary.DataAccess;

public interface ICategoryData
{
   Task CreateCategory(Category category);
   Task<List<Category>> GetCategories();
}