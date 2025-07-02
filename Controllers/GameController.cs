using Microsoft.AspNetCore.Mvc;
using KelimeTahminWeb.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace KelimeTahminWeb.Controllers
{
    public class GameController : Controller
    {
        private static GameModel _game = new GameModel();

        private static readonly string apiUrl = "https://random-word-api.vercel.app/api?words=1";

        private readonly IHttpClientFactory _httpClientFactory;

        public GameController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(_game.GizliKelime))
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.GetStringAsync(apiUrl);
                    var kelimeler = JsonSerializer.Deserialize<List<string>>(response);
                    _game = new GameModel
                    {
                        GizliKelime = kelimeler[0].ToLower()
                    };
                }
                catch (Exception ex)
                {
                    _game = new GameModel();
                    _game.Mesaj = "Kelime alınırken bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                }
            }

            return View(_game);
        }

        [HttpPost]
public IActionResult TahminYap(string[] harfler)
{
    string tahmin = string.Join("", harfler).ToLower();

    if (_game.DenemeSayisi >= 5 || _game.GizliKelime == null)
        return RedirectToAction("Index");

    if (string.IsNullOrWhiteSpace(tahmin) || !tahmin.All(char.IsLetter))
    {
        _game.Mesaj = "Geçersiz tahmin.";
        return RedirectToAction("Index");
    }

    List<string> renkSatiri = new List<string>();
    int uzunluk = Math.Min(tahmin.Length, _game.GizliKelime.Length);

    for (int i = 0; i < uzunluk; i++)
    {
        if (tahmin[i] == _game.GizliKelime[i])
            renkSatiri.Add("yesil");
        else if (_game.GizliKelime.Contains(tahmin[i]))
            renkSatiri.Add("sari");
        else
            renkSatiri.Add("gri");
    }

    for (int i = uzunluk; i < _game.GizliKelime.Length; i++)
    {
        renkSatiri.Add("gri");
    }

    _game.Tahminler.Add(tahmin.PadRight(_game.GizliKelime.Length));
    _game.Renkler.Add(renkSatiri);
    _game.DenemeSayisi++;

    if (tahmin == _game.GizliKelime)
        _game.Mesaj = "Tebrikler! Bildiniz 🎉";
    else if (_game.DenemeSayisi >= 5)
        _game.Mesaj = $"Kaybettiniz! Kelime: {_game.GizliKelime}";

    return RedirectToAction("Index");
}

public async Task<IActionResult> YeniOyun()
        {
            string kelime = "";
            var client = _httpClientFactory.CreateClient();

            // Küfürlü kelimeleri filtrelemek için blacklist
            var blacklist = new HashSet<string>
    {
        "fuck", "shit", "bitch", "asshole", "dick", "bastard",
        "piss", "cunt", "slut", "faggot", "damn", "whore",
        "retard", "gay", "lesbian", "nigger", "idiot", "stupid"

    };

            do
            {
                var response = await client.GetStringAsync(apiUrl);
                var kelimeler = JsonSerializer.Deserialize<List<string>>(response);
                kelime = kelimeler[0].ToLower();
            }
            while (
                blacklist.Contains(kelime) ||
                !kelime.All(char.IsLetter) ||
                kelime.Length < 4 || kelime.Length > 10
            );

            _game = new GameModel
            {
                GizliKelime = kelime
            };

            return RedirectToAction("Index");
        }

        public IActionResult OyundanCik()
        {
            return Content("Oyundan çıkıldı. Tarayıcıyı kapatabilirsiniz.");
        }
    }
}
