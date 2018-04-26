using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Routing;
using System.Web.Security;
using BatBreaker2.Models;
using System.Security.Principal;

namespace CommonLibrary
{
    public static class ExtensionMethods
    {
        public static double ToDouble(this int integer)
        {
            return double.Parse(integer.ToString());
        }

        public static double ToDouble(this decimal input)
        {
            return double.Parse(input.ToString());
        }

        public static int plainUserId(this IPrincipal User)
        {
            if (User != null)
            {
                AccountContext db = new AccountContext();
                aspnet_Users user = db.aspnet_Users.Where(u => u.UserName.Equals(User.Identity.Name)).FirstOrDefault();

                if (user != null)
                {
                    return user.plainUserId;
                }
            }
            return -1;
        }

        public static decimal valModifier(this int input, int mod = 0)
        {
            decimal inDec = input.ToDecimal();

            inDec = 1 + ((inDec - 65M) / 100M);

            if (mod > 0)
            {
                inDec = Math.Pow(inDec.ToDouble(), mod.ToDouble()).ToDecimal();
            }
            else if (mod < 0)
            {
                inDec = Math.Pow(inDec.ToDouble(), (1d / (mod * -1M).ToDouble())).ToDecimal();
            }

            return inDec;
        }

        public static decimal valModifier(this decimal input, int mod = 0)
        {
            input = 1 + ((input - 65M) / 100M);

            if (mod > 0)
            {
                input = Math.Pow(input.ToDouble(), mod.ToDouble()).ToDecimal();
            }
            else if (mod < 0)
            {
                input = Math.Pow(input.ToDouble(), (1d / (mod * -1M).ToDouble())).ToDecimal();
            }

            return input;
        }

        public static decimal valReversal(this int input, int mod = 0)
        {
            decimal inDec = input.ToDecimal();

            inDec = 1 - ((inDec - 65M) / 100M);

            if (mod > 0)
            {
                inDec = Math.Pow(inDec.ToDouble(), mod.ToDouble()).ToDecimal();
            }
            else if (mod < 0)
            {
                inDec = Math.Pow(inDec.ToDouble(), (1d / (mod * -1M).ToDouble())).ToDecimal();
            }

            return inDec;
        }

        public static decimal valReversal(this decimal input, int mod = 0)
        {
            input = 1 - ((input - 65M) / 100M);

            if (mod > 0)
            {
                input = Math.Pow(input.ToDouble(), mod.ToDouble()).ToDecimal();
            }
            else if (mod < 0)
            {
                input = Math.Pow(input.ToDouble(), (1d / (mod * -1M).ToDouble())).ToDecimal();
            }

            return input;
        }

        public static decimal toSwingNum(this int input, decimal center, decimal max)
        {
            decimal fromCenter = center - input;
            if (fromCenter < 0)
            {
                fromCenter = fromCenter * -1;
            }

            decimal perPixel = .8M / max;

            return .1M + (fromCenter * perPixel);
        }

        public static string ToStringOrNull(this DateTime? dt)
        {
            if (dt.HasValue)
            {
                return dt.Value.ToString("d");
            }

            return "";
        }

        public static DateTime? ToDateTimeOrNull(this string dt)
        {
            DateTime output = new DateTime();
            if (DateTime.TryParse(dt, out output))
            {
                return output;
            }

            return null;
        }

        public static bool ToBool(this string str)
        {
            bool output = false;
            bool.TryParse(str, out output);
            return output;
        }

        public static int ToInt(this string str)
        {
            int output = 0;
            int.TryParse(str, out output);
            return output;
        }

        public static int ToInt(this decimal str)
        {
            int output = 0;
            int.TryParse(str.ToString("#"), out output);
            return output;
        }

        public static decimal ToDecimal(this string str)
        {
            decimal output = 0;
            decimal.TryParse(str, out output);
            return output;
        }

        public static decimal ToDecimal(this int str)
        {
            decimal output = 0;
            decimal.TryParse(str.ToString(), out output);
            return output;
        }

        public static decimal ToDecimal(this double str)
        {
            decimal output = 0;
            decimal.TryParse(str.ToString(), out output);
            return output;
        }

        public static int ToInt(this double input)
        {
            int output = 0;
            int.TryParse(input.ToString(), out output);

            return output;
        }

        public static IQueryable<T> WhereIn<T>(this IQueryable<T> mainQuery, Expression<Func<T, object>> subSelector, IEnumerable<object> collection)
        {
            // Check for invalid parameters passed in.
            if (subSelector == null) throw new ArgumentNullException("subSelector");
            if (collection == null) throw new ArgumentNullException("collection");
            if (!collection.Any()) return mainQuery;

            // Grab the variable name used in the lambda expression.
            ParameterExpression p = subSelector.Parameters.Single();

            // Put the list of values into a form we can better use.
            IEnumerable<Expression> equals = collection.Select(value =>
               (Expression)Expression.Equal(subSelector.Body,
                    Expression.Constant(value, typeof(object))));

            // Create the Or expression.
            Expression body = equals.Aggregate((accumulate, equal) =>
                Expression.Or(accumulate, equal));

            // Apply the p variable to the new formula and return the results.
            return mainQuery.Where(Expression.Lambda<Func<T, bool>>(body, p));
        }

        public static Expression<Func<TElement, bool>> BuildContainsExpression<TElement, TValue>(
    Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector) { throw new ArgumentNullException("valueSelector"); }
            if (null == values) { throw new ArgumentNullException("values"); }
            ParameterExpression p = valueSelector.Parameters.Single();
            // p => valueSelector(p) == values[0] || valueSelector(p) == ...
            if (!values.Any())
            {
                return e => false;
            }
            var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        public static Func<TElement, bool> BuildContainsExpressionEnum<TElement, TValue>(
    Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector) { throw new ArgumentNullException("valueSelector"); }
            if (null == values) { throw new ArgumentNullException("values"); }
            ParameterExpression p = valueSelector.Parameters.Single();
            // p => valueSelector(p) == values[0] || valueSelector(p) == ...
            if (!values.Any())
            {
                return e => false;
            }
            var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
            return Expression.Lambda<Func<TElement, bool>>(body, p).Compile();
        }

        public static Expression<Func<TElement, bool>> BuildNotContainsExpression<TElement, TValue>(
    Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector) { throw new ArgumentNullException("valueSelector"); }
            if (null == values) { throw new ArgumentNullException("values"); }
            ParameterExpression p = valueSelector.Parameters.Single();
            // p => valueSelector(p) == values[0] || valueSelector(p) == ...
            if (!values.Any())
            {
                return e => false;
            }
            var equals = values.Select(value => (Expression)Expression.NotEqual(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.And(accumulate, equal));
            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }
    }
}