using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Se7enRedLines.UI.Converters
{
    public class EnumConverter : IValueConverter
    {
        //======================================================
        #region _Constructors_

        static EnumConverter()
        {
            Mappings = new Dictionary<Type, Dictionary<long, string>>();
        }

        #endregion

        //======================================================
        #region _Public methods_

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException();

            var param = parameter != null ? parameter.ToString() : string.Empty;

            var type = value.GetType();
            if (!_cache.ContainsKey(type))
            {
                var fields = type.GetFields().Where(f => f.IsLiteral);
                var values = new Dictionary<long, string>(fields.Count());
                foreach (var fieldInfo in fields)
                {
                    var key = System.Convert.ToInt64(fieldInfo.GetRawConstantValue());
                    values[key] = GetEnumFieldName(type, key, fieldInfo);
                }

                _cache.Add(type, values);

                if (param == "array")
                    return values.Values;

                if (values.ContainsKey(System.Convert.ToInt64(value)))
                    return values[System.Convert.ToInt64(value)];

                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            if (param == "array")
                return _cache[type].Values;

            if (_cache[type].ContainsKey(System.Convert.ToInt64(value)))
                return _cache[type][System.Convert.ToInt64(value)];

            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var v = value.ToString();

            var fields = targetType.GetFields().Where(f => f.IsLiteral);
            foreach (var fieldInfo in fields)
            {
                var a = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
                if (a != null && a.Description == v)
                    return fieldInfo.GetValue(null);
                if (fieldInfo.GetValue(null).ToString() == v)
                    return fieldInfo.GetValue(null);
            }

            return targetType.IsValueType ? Activator.CreateInstance(targetType) : DependencyProperty.UnsetValue;
        }

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        private static string GetEnumFieldName(Type type, long key, FieldInfo field)
        {
            var a = field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
            if (a != null)
                return a.Description;

            Dictionary<long, string> mappings;
            if (Mappings.TryGetValue(type, out mappings))
            {
                if (mappings.ContainsKey(key))
                    return mappings[key];
            }

            return field.Name;
        }

        #endregion

        //======================================================
        #region _Fields_

        protected static Dictionary<Type, IDictionary<long, string>> _cache = new Dictionary<Type, IDictionary<long, string>>(3);
        protected static Dictionary<Type, Dictionary<long, string>> Mappings;

        #endregion
    }
}