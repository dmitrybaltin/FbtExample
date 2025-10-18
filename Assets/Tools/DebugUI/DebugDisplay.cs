namespace Examples.UnitaskFbtExample.example2a
{
    public class DebugDisplay
    {
        public string Category { get; }
        public string Name { get; }
        public object Value { get; set; }

        public DebugDisplay(string category, string name, object value)
        {
            Category = category;
            Name = name;
            Value = value;
        }
    }

}