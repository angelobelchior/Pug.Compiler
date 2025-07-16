using Pug.Compiler.Runtime;

namespace Pug.Compiler.Editor.Endpoints.Models;

public record Variable(
    string Name,
    DataTypes DataType,
    object Value)
{
    public static IReadOnlyList<Variable> ToList(IDictionary<string, Identifier> identifiers)
    {
        var variables = new List<Variable>();
        foreach (var (name, identifier) in identifiers)
        {
            var variable = new Variable(name, identifier.DataType, identifier.Value);
            variables.Add(variable);
        }

        return variables;
    }
}