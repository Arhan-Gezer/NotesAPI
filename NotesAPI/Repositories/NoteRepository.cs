using Microsoft.EntityFrameworkCore;
using NotesAPI.Models;

namespace NotesAPI.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _db;

        public NoteRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Note>> GetAllAsync()
        {
            return await _db.Notes.ToListAsync();
        }

        public async Task<Note?> GetByIdAsync(int id)
        {
            return await _db.Notes.FindAsync(id);
        }

        public async Task<Note> CreateAsync(Note note)
        {
            _db.Notes.Add(note);
            await _db.SaveChangesAsync();
            return note;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return false;
            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}