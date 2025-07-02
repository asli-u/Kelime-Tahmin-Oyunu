using System.Collections.Generic;

namespace KelimeTahminWeb.Models
{
    public class GameModel
    {
        public string GizliKelime { get; set; }
        public int DenemeSayisi { get; set; }
        public List<string> Tahminler { get; set; } = new List<string>();
        public List<List<string>> Renkler { get; set; } = new List<List<string>>();
        public string Mesaj { get; set; }
    }
}
