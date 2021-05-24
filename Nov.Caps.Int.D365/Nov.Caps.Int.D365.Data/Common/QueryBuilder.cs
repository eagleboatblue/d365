using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace Nov.Caps.Int.D365.Data.Common
{
    public static class QueryBuilder
    {
        public static (string Query, DynamicParameters Parameters) BuildWhere(List<KeyValuePair<string, List<object>>> valuePairs)
        {
            var result = (Query: string.Empty, Parameters: new DynamicParameters());
            var sb = new List<string>();

            valuePairs.ForEach((pair) =>
            {
                var line = pair.Key;
                var paramName = pair.Key.Split('.').Last().ToLowerInvariant();

                if (pair.Value.Count > 1)
                {
                    var first = pair.Value.First();

                    if (first is string)
                    {
                        line += $" IN (@{paramName})";
                        result.Parameters.Add(paramName, string.Join("', '", pair.Value));
                    }
                    else
                    {
                        // by some reason numbers do not work well through params
                        // result.Parameters.Add(paramName, pair.Value.Select(i => (int)i).ToArray());
                        line += $" IN ({string.Join(", ", pair.Value.Select(i => i.ToString()))})";
                    }
                }
                else
                {
                    line += $" = @{paramName}";
                    result.Parameters.Add(paramName, pair.Value.First());
                }

                sb.Add(line);
            });

            result.Query = string.Join(" AND ", sb);

            return result;
        }
    }
}
