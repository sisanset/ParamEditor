using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParamEditor.Models
{
    public class ParameterDefinition
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public double[]? Range { get; set; }
        public List<string>? Values { get; set; }
        public string? Unit { get; set; }
    }

    public class SchemaRoot
    {
        public List<ParameterDefinition> Parameters { get; set; } = new();
    }
}
