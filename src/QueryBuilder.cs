using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace DapperBuilder;

public class QueryBuilder<TEntity> where TEntity : class
{
    private readonly List<string> _columns = new();

    public QueryBuilder<TEntity> Select<TProperty>(Expression<Func<TEntity, TProperty>> selector)
    {
        var members = GetMembersFromExpression(selector);
        foreach (var member in members)
        {
            _columns.Add(GetColumnName(member));
        }
        return this;
    }

    public string Build()
    {
        var tableName = typeof(TEntity).GetCustomAttributes(false)
            .OfType<TableAttribute>()
            .FirstOrDefault()?.Name ?? typeof(TEntity).Name;

        var columns = string.Join(", ", _columns);
        return $"SELECT {columns} FROM {tableName}";
    }

    private static IEnumerable<MemberInfo> GetMembersFromExpression<TProperty>(Expression<Func<TEntity, TProperty>> selector)
    {
        var expression = selector.Body;

        if (expression is MemberInitExpression memberInitExpression && memberInitExpression.Bindings != null)
        {
            return memberInitExpression.Bindings.Select(b => b.Member);
        }

        if (expression is MemberExpression memberExpression && memberExpression?.Member != null)
        {
            return new[] { memberExpression.Member };
        }

        if (expression is UnaryExpression unaryExpression && unaryExpression?.Operand is MemberExpression operandMemberExpression)
        {
            return new[] { operandMemberExpression.Member };
        }

        throw new ArgumentException("Invalid expression type", nameof(expression));
    }

    private static string GetColumnName(MemberInfo member)
    {
        return member.GetCustomAttribute<ColumnAttribute>()?.Name ?? member.Name;
    }
}