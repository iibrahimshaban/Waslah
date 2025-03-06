namespace Waslah.Entities
{
    public sealed class StationType
    {
        public string Name { get; set; }=string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<Station> Stations { get; set; } = [];
    }
}
