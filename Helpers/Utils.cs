using System.Linq.Expressions;

namespace infotecsWebApi.Helpers
{
    public static class Utils
    {
        public static IQueryable<T> WhereBetween<T, TProperty>(
            this IQueryable<T> query,
            Expression<Func<T, TProperty>> selector,
            TProperty? min,
            TProperty? max
        ) where TProperty : struct, IComparable<TProperty>
        {
            if (min.HasValue)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var selectorCall = Expression.Invoke(selector, parameter);
                var comparison = Expression.GreaterThanOrEqual(selectorCall, Expression.Constant(min.Value));
                var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                query = query.Where(lambda);
            }

            if (max.HasValue)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var selectorCall = Expression.Invoke(selector, parameter);
                var comparison = Expression.LessThanOrEqual(selectorCall, Expression.Constant(max.Value));
                var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        public static float GetMedian(List<float> sortedValues)
        {
            if (sortedValues == null || sortedValues.Count == 0)
                return 0;

            sortedValues.Sort(); // make sure it's sorted!

            int mid = sortedValues.Count / 2;

            if (sortedValues.Count % 2 == 0)
                return (sortedValues[mid - 1] + sortedValues[mid]) / 2f;
            else
                return sortedValues[mid];
        }
    }
}
