using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace ReactiveProgramming_Demo.Extensions
{
    public static class MvvmReactiveExtensions
    {
        public static IObservable<T> GetPropertyAsObservable<T>(this INotifyPropertyChanged @class,
        Expression<Func<T>> property)
        {
            var propName = (property.Body as MemberExpression).Member.Name;

            return Observable.FromEventPattern<PropertyChangedEventArgs>(@class,
            nameof(@class.PropertyChanged))
            .Where(a => a.EventArgs.PropertyName == propName)
            .Select(a => property.Compile().Invoke());
        }
    }
}
