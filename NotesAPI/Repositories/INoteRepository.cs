using NotesAPI.Models;

namespace NotesAPI.Repositories
{
    public interface INoteRepository
    {
        Task<List<Note>> GetAllAsync();
        Task<Note?> GetByIdAsync(int id);
        Task<Note> CreateAsync(Note note);
        Task<bool> DeleteAsync(int id);
    }
}