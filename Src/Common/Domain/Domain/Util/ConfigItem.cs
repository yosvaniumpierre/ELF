namespace Avanade.Domain.Util
{
    public sealed class ConfigItem
    {
        public int Id { get; private set; }

        public string Category { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Comments { get; set; }
    }

    public enum ValueType
    {
        
    }
}
