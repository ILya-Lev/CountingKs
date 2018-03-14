namespace CountingKs.Models
{
    public class DiaryEntryModel
    {
        public string Url { get; set; }
        public double Quantity { get; set; }
        public string FoodDescription { get; set; }
        public MeasureModel Measure { get; set; }
    }
}