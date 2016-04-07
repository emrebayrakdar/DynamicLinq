using DynamicExpression;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicLinq
{
    public static class Expressions
    {
        /// <summary>
        /// Dynamic Expression Oluşturmak İçin Kullanılan Class
        /// </summary>
        public class Filter
        {
            public string PropertyName { get; set; }
            public OperationType Operation { get; set; }
            public dynamic Value { get; set; }
        }

        /// <summary>
        /// İki Expression Sorgusunu Birleştiren Metot
        /// </summary>
        /// <param name="LeftExpression"></param>
        /// <param name="Operation"></param>
        /// <param name="RightExpression"></param>
        /// <returns></returns>
        public static Expression CombineExpression(Expression LeftExpression, OperationType Operation, Expression RightExpression)
        {
            switch (Operation)
            {
                case OperationType.AndAlso: return Expression.AndAlso(LeftExpression, RightExpression);
                case OperationType.OrElse: return Expression.OrElse(LeftExpression, RightExpression);
                case OperationType.Equal: return Expression.Equal(LeftExpression, RightExpression);
                case OperationType.NotEqual: return Expression.NotEqual(LeftExpression, RightExpression);
                case OperationType.GreaterThan: return Expression.GreaterThan(LeftExpression, RightExpression);
                case OperationType.GreaterThanOrEqual: return Expression.GreaterThanOrEqual(LeftExpression, RightExpression);
                case OperationType.LessThan: return Expression.LessThan(LeftExpression, RightExpression);
                case OperationType.LessThanOrEqual: return Expression.LessThanOrEqual(LeftExpression, RightExpression);
            }
            return null;
        }

        /// <summary>
        /// Expression Oluşturan Metot (Örnek Çıktı --> x.PropertyName == "Ali")
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Oyleki">x tanımlaması</param>
        /// <param name="PropertyName">x.PropertyName</param>
        /// <param name="Operation"> && ~ || ~ == ~ != </param>
        /// <param name="Value">x.PropertyName (Operation) ??</param>
        /// <returns></returns>
        private static Expression ExpressionCreate<T>(ParameterExpression Oyleki, string PropertyName, OperationType Operation, dynamic Value) where T : class
        {
            T Entity = Activator.CreateInstance<T>();
            PropertyInfo Property = Utils.GetProperty(Entity, PropertyName);
            var ExpRight = Expression.Constant(Value, Property.PropertyType);
            Expression ExpLeft = Expression.Property(Oyleki, Property);
            Expression AllExp = CombineExpression(ExpLeft, Operation, ExpRight);
            return AllExp;
        }

        /// <summary>
        /// Expression Oluşturan Metot (Örnek Çıktı --> x=> x.PropertyName == "Ali") 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> DynamicExpressionCreate<T>(List<Filter> List) where T : class
        {
            ParameterExpression Oyleki = Expression.Parameter(typeof(T), "x");
            Expression AllExpression = null;
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i] != null)
                {
                    if (List[i].PropertyName == null && List[i].Value == null)
                    {
                        if (List[i + 1] != null)
                        {
                            var RightExpression = ExpressionCreate<T>(Oyleki, List[i + 1].PropertyName, List[i + 1].Operation, List[i + 1].Value);

                            AllExpression = CombineExpression(AllExpression, List[i].Operation, RightExpression);
                            List[i + 1] = null;
                        }
                    }
                    else
                    {
                        AllExpression = ExpressionCreate<T>(Oyleki, List[i].PropertyName, List[i].Operation, List[i].Value);
                    }
                }
            }
            //AllExpression --> x=> x.Name = "Emre" && x.Surname == "Bayrakdar"
            return Expression.Lambda<Func<T, bool>>(AllExpression, Oyleki); // x=> x.Name = "Emre" && x.Surname == "Bayrakdar"
        }
    }
}
