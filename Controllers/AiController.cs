using Microsoft.AspNetCore.Mvc;
using MyGYM.Models; // Model namespace'ini kontrol et
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyGym.Controllers
{
    public class AiController : Controller
    {
        private const string ApiKey = "AIzaSyDXUEq_hJH0N9-HomD7UijN3cMa28Nbvbo";
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";
        [HttpGet]
        public IActionResult Index()
        {
            return View(new AiKocViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> Index(AiKocViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Lütfen bilgileri eksiksiz doldurunuz.";
                return View(model);
            }

            // 1. Prompt (İstem) Hazırlama
            string prompt = $@"
                Sen profesyonel, motive edici ve bilgili bir spor salonu antrenörüsün.
                
                Kullanıcı Bilgileri:
                - Yaş: {model.Yas}
                - Boy: {model.Boy} cm
                - Kilo: {model.Kilo} kg
                - Cinsiyet: {model.Cinsiyet}
                - Aktivite Seviyesi: {model.Aktivite}
                - Hedef: {model.Hedef}

                İstekler:
                1. Günlük kalori ihtiyacını hesapla.
                2. Bu hedefe uygun haftalık antrenman programı yaz.
                3. Beslenme için 3 önemli madde öner.
                
                Lütfen cevabı temiz, anlaşılır ve Türkçe olarak ver.";

            using (var client = new HttpClient())
            {
                // 2. Gemini İçin JSON Verisi Hazırlama
                var requestData = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

                try
                {
                    // 3. İsteği Gönder (API Key URL içinde gönderilir)
                    string urlWithKey = $"{BaseUrl}?key={ApiKey}";
                    var response = await client.PostAsync(urlWithKey, jsonContent);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // 4. Gemini Cevabını Çözümle
                        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString);

                        // Cevap hiyerarşisi: Candidates -> Content -> Parts -> Text
                        string advice = geminiResponse?.Candidates?[0]?.Content?.Parts?[0]?.Text;

                        if (!string.IsNullOrEmpty(advice))
                        {
                            model.YapayZekaCevabi = advice;
                        }
                        else
                        {
                            ViewBag.Error = "Yapay zeka boş bir cevap döndürdü.";
                        }
                    }
                    else
                    {
                        // Hata detayını göster
                        ViewBag.Error = $"Hata: {response.StatusCode} - Mesaj: {responseString}";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Bağlantı hatası: " + ex.Message;
                }
            }

            return View(model);
        }
    }

    // --- GEMINI JSON MODELLERİ ---

    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content Content { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; }
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}