# NotesAPI

ASP.NET Core Web API projesi. Not yönetimi ve yapay zeka entegrasyonu içerir.

## Özellikler

- Not ekleme, listeleme, silme
- JWT ile kimlik doğrulama
- Ollama + DeepSeek ile otomatik AI özet
- Quartz background job
- SQLite veritabanı

## Teknolojiler

- .NET 8
- Entity Framework Core
- SQLite
- Quartz.NET
- Ollama (DeepSeek-r1:14b)
- JWT Authentication

## Kurulum

1. Ollama kur ve DeepSeek modelini indir:  ollama run deepseek-r1:14b

2. Projeyi çalıştır: dotnet run

3. Swagger: `https://localhost:7111/swagger`

## API Endpoints

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| POST | /api/auth/register | Kayıt ol |
| POST | /api/auth/login | Giriş yap, token al |
| GET | /api/Notes | Tüm notları listele |
| POST | /api/Notes | Not ekle |
| GET | /api/Notes/{id} | Tek not getir |
| DELETE | /api/Notes/{id} | Not sil |
| GET | /api/Notes/{id}/summary | AI özet üret |
