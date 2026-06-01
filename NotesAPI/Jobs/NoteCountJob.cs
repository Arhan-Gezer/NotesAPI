using Quartz;
using NotesAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace NotesAPI.Jobs
{
    [DisallowConcurrentExecution]
    public class NoteCountJob : IJob
    {
        private readonly AppDbContext _db;

        public NoteCountJob(AppDbContext db)
        {
            _db = db;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var unsummarizedNotes = await _db.Notes
                .Where(n => n.Summary == null)
                .ToListAsync();

            if (!unsummarizedNotes.Any())
            {
                Console.WriteLine($"[{DateTime.Now}] Özetlenecek not yok.");
                return;
            }

            var client = new HttpClient();
            foreach (var note in unsummarizedNotes)
            {
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
                note.Summary = result.GetProperty("response").GetString();

                await _db.SaveChangesAsync(); // her notu özetleyince hemen kaydet
                Console.WriteLine($"[{DateTime.Now}] Not {note.Id} özetlendi.");
            }

            await _db.SaveChangesAsync();
        }
    }
}