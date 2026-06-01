using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesAPI.Models;
using NotesAPI.Repositories;
using System.Text;
using System.Text.Json;

namespace NotesAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _noteRepository;

        public NotesController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notes = await _noteRepository.GetAllAsync();
            return Ok(notes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            if (note == null) return NotFound();
            return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Note note)
        {
            if (string.IsNullOrWhiteSpace(note.Title))
                return BadRequest("Title boş olamaz.");

            if (string.IsNullOrWhiteSpace(note.Content))
                return BadRequest("Content boş olamaz.");

            var created = await _noteRepository.CreateAsync(note);
            return Ok(created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _noteRepository.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetSummary(int id)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            if (note == null) return NotFound();

            var client = new HttpClient();
            var requestBody = new
            {
                model = "llama3.2",
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