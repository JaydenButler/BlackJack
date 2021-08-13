namespace Structs
{
    public class Card
    {
        public string Suit { get; set; }
        public string DisplayValue { get; set; }
        public int Value { get; set; }
        public UnityEngine.GameObject CardObject { get; set; }
        public bool DisplayCard { get; set; }
    }
}