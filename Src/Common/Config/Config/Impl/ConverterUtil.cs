using System;
using System.Reflection;

namespace Avanade.Config.Impl
{
    /// <summary>
    /// ConverterUtil class for basic types.
    /// </summary>
    internal static class ConverterUtil
    {
        #region Methods

        /// <summary>
        /// Checks whether or not the object can be converted to a type.
        /// </summary>
        /// <param name="val">The value to test for conversion to the type
        /// associated with the property</param>
        /// <returns>True if the object can be converted to a type.</returns>
        public static bool CanConvertTo<T>(string val)
        {
            return CanConvertTo(typeof (T), val);
        }

        /// <summary>
        /// Checks whether or not the object can be converted to a type.
        /// </summary>
        /// <param name="type">The property represnting the type to convert 
        /// val to</param>
        /// <param name="val">The value to test for conversion to the type
        /// associated with the property</param>
        /// <returns>True if the object can be converted to a type.</returns>
        public static bool CanConvertTo(Type type, string val)
        {
            // Data could be passed as string value.
            // Try to change type to check type safety.
            try
            {
                if (type == typeof (int))
                {
                    int result;
                    if (int.TryParse(val, out result)) return true;

                    return false;
                }
                if (type == typeof (string))
                {
                    return true;
                }
                if (type == typeof (double))
                {
                    double d;
                    if (double.TryParse(val, out d)) return true;

                    return false;
                }
                if (type == typeof (long))
                {
                    long l;
                    if (long.TryParse(val, out l)) return true;

                    return false;
                }
                if (type == typeof (float))
                {
                    float f;
                    if (float.TryParse(val, out f)) return true;

                    return false;
                }
                if (type == typeof (bool))
                {
                    bool b;
                    if (bool.TryParse(val, out b)) return true;

                    return false;
                }
                if (type == typeof (DateTime))
                {
                    DateTime d;
                    return DateTime.TryParse(val, out d);
                }
                if (type.BaseType == typeof (Enum))
                {
                    Enum.Parse(type, val, true);
                }
            }
            catch (Exception)
            {
                return false;
            }

            //Conversion worked.
            return true;
        }

        /// <summary>
        /// Check to see if can convert to appropriate type
        /// </summary>
        /// <param name="propInfo">Information of property to check.</param>
        /// <param name="val">Object to convert to.</param>
        /// <returns>True if the object can be converted to the property type.</returns>
        public static bool CanConvertToCorrectType(PropertyInfo propInfo, object val)
        {
            // Data could be passed as string value.
            // Try to change type to check type safety.
            try
            {
                if (propInfo.PropertyType == typeof (int))
                {
                    Convert.ToInt32(val);
                }
                else if (propInfo.PropertyType == typeof (double))
                {
                    Convert.ToDouble(val);
                }
                else if (propInfo.PropertyType == typeof (long))
                {
                    double l = Convert.ToInt64(val);
                }
                else if (propInfo.PropertyType == typeof (float))
                {
                    double f = Convert.ToSingle(val);
                }
                else if (propInfo.PropertyType == typeof (bool))
                {
                    bool b = Convert.ToBoolean(val);
                }
                else if (propInfo.PropertyType == typeof (DateTime))
                {
                    DateTime d = Convert.ToDateTime(val);
                }
                else if (propInfo.PropertyType.BaseType == typeof (Enum) && val is string)
                {
                    Enum.Parse(propInfo.PropertyType, (string) val, true);
                }
            }
            catch (Exception)
            {
                return false;
            }

            //Conversion worked.
            return true;
        }

        /// <summary>
        /// Checks whether or not the string can be converted to a propert type.
        /// </summary>
        /// <param name="propInfo">The property represnting the type to convert 
        /// val to</param>
        /// <param name="val">The value to test for conversion to the type
        /// associated with the property</param>
        /// <returns>True if the string can be converted.</returns>
        public static bool CanConvertToCorrectType(PropertyInfo propInfo, string val)
        {
            // Data could be passed as string value.
            // Try to change type to check type safety.
            try
            {
                if (propInfo.PropertyType == typeof (int))
                {
                    int result;
                    if (int.TryParse(val, out result)) return true;

                    return false;
                }
                if (propInfo.PropertyType == typeof (string))
                {
                    return true;
                }
                if (propInfo.PropertyType == typeof (double))
                {
                    double d;
                    if (double.TryParse(val, out d)) return true;

                    return false;
                }
                else if (propInfo.PropertyType == typeof (long))
                {
                    long l;
                    if (long.TryParse(val, out l)) return true;

                    return false;
                }
                else if (propInfo.PropertyType == typeof (float))
                {
                    float f;
                    if (float.TryParse(val, out f)) return true;

                    return false;
                }
                else if (propInfo.PropertyType == typeof (bool))
                {
                    bool b;
                    if (bool.TryParse(val, out b)) return true;

                    return false;
                }
                else if (propInfo.PropertyType == typeof (DateTime))
                {
                    DateTime d = DateTime.MinValue;
                    if (DateTime.TryParse(val, out d)) return true;

                    return false;
                }
                else if (propInfo.PropertyType.BaseType == typeof (Enum))
                {
                    Enum.Parse(propInfo.PropertyType, val, true);
                }
            }
            catch (Exception)
            {
                return false;
            }

            //Conversion worked.
            return true;
        }

        /// <summary>
        /// Convert to correct type.
        /// </summary>
        /// <typeparam name="T">Type of object to convert.</typeparam>
        /// <param name="input">Object to convert.</param>
        /// <returns>Converted object.</returns>
        public static object ConvertObj<T>(object input)
        {
            if (input == null) return default(T);

            if (typeof (T) == typeof (int))
                return Convert.ToInt32(input);
            else if (typeof (T) == typeof (long))
                return Convert.ToInt64(input);
            else if (typeof (T) == typeof (string))
                return Convert.ToString(input);
            else if (typeof (T) == typeof (bool))
                return Convert.ToBoolean(input);
            else if (typeof (T) == typeof (double))
                return Convert.ToDouble(input);
            else if (typeof (T) == typeof (DateTime))
                return Convert.ToDateTime(input);

            return default(T);
        }

        /// <summary>
        /// Convert to correct type.
        /// </summary>
        /// <typeparam name="T">Type of object whose type is to be converted.</typeparam>
        /// <param name="input">Object whose type is to be converted.</param>
        /// <returns>Type of converted object.</returns>
        public static T ConvertTo<T>(object input)
        {
            object result = default(T);
            if (input == null || input == DBNull.Value) return (T) result;

            if (typeof (T) == typeof (int))
                result = Convert.ToInt32(input);
            else if (typeof (T) == typeof (long))
                result = Convert.ToInt64(input);
            else if (typeof (T) == typeof (string))
                result = Convert.ToString(input);
            else if (typeof (T) == typeof (bool))
                result = Convert.ToBoolean(input);
            else if (typeof (T) == typeof (double))
                result = Convert.ToDouble(input);
            else if (typeof (T) == typeof (DateTime))
                result = Convert.ToDateTime(input);

            return (T) result;
        }

        /// <summary>
        /// Convert to correct type.
        /// </summary>
        /// <param name="type">Type of object whose type is to be converted.</param>
        /// <param name="input">Object whose type is to be converted.</param>
        /// <returns>Type of converted object.</returns>
        public static object ConvertTo(Type type, object input)
        {
            object result = null;
            if (input == null || input == DBNull.Value) return null;

            if (type == typeof (int))
                result = Convert.ToInt32(input);
            else if (type == typeof (long))
                result = Convert.ToInt64(input);
            else if (type == typeof (string))
                result = Convert.ToString(input);
            else if (type == typeof (bool))
                result = Convert.ToBoolean(input);
            else if (type == typeof (double))
                result = Convert.ToDouble(input);
            else if (type == typeof (DateTime))
                result = Convert.ToDateTime(input);

            return result;
        }

        /// <summary>
        /// Convert the val from string type to the same time as the property.
        /// </summary>
        /// <param name="propInfo">Property representing the type to convert to</param>
        /// <param name="val">val to convert</param>
        /// <returns>converted value with the same time as the property</returns>
        public static object ConvertToSameType(PropertyInfo propInfo, object val)
        {
            object convertedType = null;

            if (propInfo.PropertyType == typeof (int))
            {
                convertedType = Convert.ChangeType(val, typeof (int));
            }
            else if (propInfo.PropertyType == typeof (double))
            {
                convertedType = Convert.ChangeType(val, typeof (double));
            }
            else if (propInfo.PropertyType == typeof (long))
            {
                convertedType = Convert.ChangeType(val, typeof (long));
            }
            else if (propInfo.PropertyType == typeof (float))
            {
                convertedType = Convert.ChangeType(val, typeof (float));
            }
            else if (propInfo.PropertyType == typeof (bool))
            {
                convertedType = Convert.ChangeType(val, typeof (bool));
            }
            else if (propInfo.PropertyType == typeof (DateTime))
            {
                convertedType = Convert.ChangeType(val, typeof (DateTime));
            }
            else if (propInfo.PropertyType == typeof (string))
            {
                convertedType = Convert.ChangeType(val, typeof (string));
            }
            else if (propInfo.PropertyType.BaseType == typeof (Enum) && val is string)
            {
                convertedType = Enum.Parse(propInfo.PropertyType, (string) val, true);
            }
            return convertedType;
        }

        /// <summary>
        /// Determine if the type of the property and the val are the same type.
        /// </summary>
        /// <param name="propInfo">Property info to check.</param>
        /// <param name="val">Value to check against property info.</param>
        /// <returns>True if the property and the value are of the same type.</returns>
        public static bool IsSameType(PropertyInfo propInfo, object val)
        {
            // Quick Validation.
            if (propInfo.PropertyType == typeof (int) && val is int)
            {
                return true;
            }
            if (propInfo.PropertyType == typeof (bool) && val is bool)
            {
                return true;
            }
            if (propInfo.PropertyType == typeof (string) && val is string)
            {
                return true;
            }
            if (propInfo.PropertyType == typeof (double) && val is double)
            {
                return true;
            }
            if (propInfo.PropertyType == typeof (long) && val is long)
            {
                return true;
            }
            if (propInfo.PropertyType == typeof (float) && val is float)
            {
                return true;
            }
            if (propInfo.PropertyType == typeof (DateTime) && val is DateTime)
            {
                return true;
            }
            if (propInfo.PropertyType is object && propInfo.PropertyType.GetType() == val.GetType())
            {
                return true;
            }

            return false;
        }

        #endregion Methods
    }
}