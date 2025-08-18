using System.Linq.Expressions;
using System.Reflection;

namespace Company.Project.Infrastructure.Filter
{
    public static class DynamicFilterHelper
    {
        public static IQueryable<T> ApplyFilters<T>(IQueryable<T> source, string[] filters)
        {
            if (filters == null || filters.Length == 0) return source;

            var parameter = Expression.Parameter(typeof(T), "entity");
            Expression? combinedExpression = null;

            foreach (var filterGroup in filters)
            {
                Expression? groupExpression = null;
                var conditions = filterGroup.Split(',');

                foreach (var condition in conditions)
                {
                    try
                    {
                        var parts = ParseCondition(condition);
                        if (parts == null || parts.Length != 3) continue;

                        var propertyName = parts[0];
                        var operatorSymbol = parts[1];
                        var value = parts[2];

                        var property = typeof(T).GetProperty(propertyName,
                            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (property == null) continue;

                        var propertyExpression = Expression.Property(parameter, property);
                        var constantValue = Convert.ChangeType(value, property.PropertyType);

                        Expression? conditionExpression = operatorSymbol switch
                        {
                            "=" => Expression.Equal(propertyExpression, Expression.Constant(constantValue)),
                            ">" => Expression.GreaterThan(propertyExpression, Expression.Constant(constantValue)),
                            "<" => Expression.LessThan(propertyExpression, Expression.Constant(constantValue)),
                            ">=" => Expression.GreaterThanOrEqual(propertyExpression,
                                Expression.Constant(constantValue)),
                            "<=" => Expression.LessThanOrEqual(propertyExpression, Expression.Constant(constantValue)),
                            "!=" => Expression.NotEqual(propertyExpression, Expression.Constant(constantValue)),
                            "<>" => Expression.Call(propertyExpression,
                                typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                                Expression.Constant(value, typeof(string))),
                            _ => null
                        };

                        if (conditionExpression == null) continue;

                        groupExpression = groupExpression == null
                            ? conditionExpression
                            : Expression.AndAlso(groupExpression, conditionExpression);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing condition '{condition}': {ex.Message}");
                    }
                }

                combinedExpression = combinedExpression == null
                    ? groupExpression
                    : Expression.OrElse(combinedExpression, groupExpression);
            }

            if (combinedExpression == null) return source;

            try
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                return source.Where(lambda);
            }
            catch (Exception ex)
            {
                // Log exception if necessary
                Console.WriteLine($"Error creating lambda expression: {ex.Message}");
                return source;
            }
        }

        private static string[] ParseCondition(string condition)
        {
            string[] operators = { ">=", "<=", "!=", "<>", "=", ">", "<" };
            foreach (var op in operators)
            {
                var parts = condition.Split(new[] { op }, 2, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    return new[] { parts[0].Trim(), op, parts[1].Trim() };
                }
            }

            return null;
        }
    }
}