namespace TSP
{
    public class TabuElement
    {
        // City no 1
        public int From { get; set; }
        // City no 2
        public int To { get; set; }
        // For how long will it be on a taboo list
        public int Cadency { get; set; }
    }
}