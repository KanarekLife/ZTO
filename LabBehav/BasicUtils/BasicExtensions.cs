using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicUtils
{
    public static class BasicExtensions
    {
        // Methods
        public static IList<T> AddMany<T>(this IList<T> list, params T[] items)
        {
            return list.AddRange<T>(items);
        }

        public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            items.Each<T>(delegate(T t)
            {
                list.Add(t);
            });
            return list;
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T> eachAction)
        {
            if (values.IsNotEmpty())
            {
                foreach (T local in values)
                {
                    eachAction(local);
                }
            }
            return values;
        }

        public static System.Collections.IEnumerable Each(this System.Collections.IEnumerable values, Action<object> eachAction)
        {
            foreach (object obj2 in values)
            {
                eachAction(obj2);
            }
            return values;
        }

        public static VALUE Get<KEY, VALUE>(this IDictionary<KEY, VALUE> dictionary, KEY key)
        {
            return dictionary.Get<KEY, VALUE>(key, default(VALUE));
        }

        public static VALUE Get<KEY, VALUE>(this IDictionary<KEY, VALUE> dictionary, KEY key, VALUE defaultValue)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            return defaultValue;
        }

        public static bool IsArrayOf<TYPE>(this Type type)
        {
            if (!type.IsArray)
            {
                return false;
            }
            if (!type.GetElementType().Equals(typeof(TYPE)))
            {
                return type.GetElementType().IsSubclassOf(typeof(TYPE));
            }
            return true;
        }

        public static bool IsEmpty(this System.Collections.IEnumerable collection)
        {
            if (collection != null)
            {
                return !collection.GetEnumerator().MoveNext();
            }
            return true;
        }

        public static bool IsEmpty(this string stringValue)
        {
            return string.IsNullOrEmpty(stringValue);
        }

        // public static bool IsEnumerable(this Type type)
        // {
        //     return ((type.GetInterface(typeof(IEnumerable).FullName) != null) || (type.IsInterface && (type.Name.IndexOf("IEnumerable") >= 0)));
        // }

        public static bool IsGenericEnumerable(this Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }
            return ((type.GetInterface(typeof(IEnumerable<>).FullName) != null) || (type.IsInterface && (type.Name.IndexOf("IEnumerable") >= 0)));
        }

        public static bool IsNotEmpty(this System.Collections.IEnumerable collection)
        {
            return ((collection != null) && collection.GetEnumerator().MoveNext());
        }

        public static bool IsNotEmpty(this string stringValue)
        {
            return !string.IsNullOrEmpty(stringValue);
        }

        public static bool IsNullable(this Type theType)
        {
            if (theType.IsValueType)
            {
                return theType.IsNullableOfT();
            }
            return true;
        }

        public static bool IsNullableOf(this Type theType, Type otherType)
        {
            return (theType.IsNullableOfT() && theType.GetGenericArguments()[0].Equals(otherType));
        }

        public static bool IsNullableOfT(this Type theType)
        {
            return (theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }

        public static bool ToBool(this string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return false;
            }
            return bool.Parse(stringValue);
        }

        public static string ToFormat(this string stringFormat, params object[] args)
        {
            return string.Format(stringFormat, args);
        }

        public static string ToJSON(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy HH:mm:ss").Replace("-", "/");
        }
    }
}
