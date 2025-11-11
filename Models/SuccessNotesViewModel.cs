using IMEAutomationDBOperations.Models; // Bu satırı ekleyin

namespace KonyaTeknikÜniversitesi_IMEOtomasyonu.Models
{
    public class SuccessNotesViewModel
    {
        public decimal ImeSorumlusuDegerlendirmeNotu { get; set; } // IME Sorumlusu Değerlendirme Formu Genel Notu
        public decimal ImeOgretimElemaniDegerlendirmeNotu { get; set; } // İME Sorumlu Öğretim Elemanı Değerlendirme Formu Genel Notu
        public decimal HaftalikVideoSunumNotu { get; set; } // Haftalık Video Sunumlarının Genel Notu
        public decimal BolumImeKomisyonuNotu { get; set; } // Bölüm İME Komisyonu Genel Notu
        public EvaluationPersonel SupervisorEvaluation { get; set; }
        public string? InstructorFeedback { get; set; }
        public string? CommissionFeedback { get; set; }

        // Weights (as decimals for calculation)
        public const decimal ImeSorumlusuDegerlendirmeWeight = 0.20m; // 20%
        public const decimal ImeOgretimElemaniDegerlendirmeWeight = 0.26m; // 26%
        public const decimal HaftalikVideoSunumWeight = 0.14m; // 14%
        public const decimal BolumImeKomisyonuWeight = 0.40m; // 40%

        public decimal CalculateFinalGrade()
        {
            return (ImeSorumlusuDegerlendirmeNotu * ImeSorumlusuDegerlendirmeWeight) +
                   (ImeOgretimElemaniDegerlendirmeNotu * ImeOgretimElemaniDegerlendirmeWeight) +
                   (HaftalikVideoSunumNotu * HaftalikVideoSunumWeight) +
                   (BolumImeKomisyonuNotu * BolumImeKomisyonuWeight);
        }
    }
}
