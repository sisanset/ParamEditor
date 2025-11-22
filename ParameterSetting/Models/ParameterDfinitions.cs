
namespace ParamEditor.Models
{
    public class ParameterDefinition
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public string Group { get; set; } = "General";
        public double[]? Range { get; set; }
        public List<string>? Values { get; set; }
        public string? Unit { get; set; }
        public int? Decimalplaces { get; set; }
        public string Relation { get; set; } = "";
        public int Order { get; set; } = 0;

    }

    public class SchemaRoot
    {
        public List<ParameterDefinition> Parameters { get; set; } = new();
    }
}
