using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesAPI.Models;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace NotesAPI.Controllers
{
    
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public NotesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notes = await _db.Notes.ToListAsync();
            return Ok(notes);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Note note)
        {
            if (string.IsNullOrWhiteSpace(note.Title))
                return BadRequest("Title boş olamaz.");

            if (string.IsNullOrWhiteSpace(note.Content))
                return BadRequest("Content boş olamaz.");

            _db.Notes.Add(note);
            await _db.SaveChangesAsync();
            return Ok(note);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound();
            return Ok(note);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound();
            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetSummary(int id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound();

            var client = new HttpClient();
            var requestBody = new
            {
                model = "deepseek-r1:14b",
                prompt = $"Şu metni kısaca özetle: {note.Content}",
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:11434/api/generate", content);
            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<JsonElement>(responseString);
            var summary = result.GetProperty("response").GetString();

            return Ok(new { noteId = id, summary });
        }


    }
}