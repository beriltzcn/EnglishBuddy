namespace EnglishBuddy.Models
{
    public class WordCard
    {
        public int Id { get; set; }
        public string Word { get; set; } = "";
        public string Meaning { get; set; } = "";
        public string ExampleSentence { get; set; } = "";
        public int ReviewCount { get; set; }
        public int CorrectCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
