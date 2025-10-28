using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XDFLib.Extensions.Reflections
{
    public static class ReflectionExtensions
    {
        public static object GetFieldValue(this object obj, string fieldName)
        {
            var fieldInfo = obj.GetType().GetField(fieldName);
            if (fieldInfo != null)
            {
                var objRef = __makeref(obj);
                return fieldInfo.GetValueDirect(objRef);
            }
            return null;
        }

        public static (bool, T) GetFieldValue<T>(this object obj, string fieldName)
        {
            var fieldInfo = obj.GetType().GetField(fieldName);
            if (fieldInfo != null)
            {
                var objRef = __makeref(obj);
                if (typeof(T).IsAssignableFrom(fieldInfo.FieldType))
                {
                    return (true, (T)fieldInfo.GetValueDirect(objRef));
                }
            }
            return (false, default(T));
        }

        public static bool SetFieldValue(this object obj, string fieldName, object newValue)
        {
            if (obj != null)
            {
                var fieldInfo = obj.GetType().GetField(fieldName);
                if (fieldInfo != null)
                {
                    if (fieldInfo.FieldType.IsAssignableFrom(newValue.GetType()))
                    {
                        fieldInfo.SetValue(obj, newValue);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool SetFieldValue<T>(this object obj, string fieldName, T newValue)
        {
            if (obj != null)
            {
                var fieldInfo = obj.GetType().GetField(fieldName);
                if (fieldInfo != null)
                {
                    if (fieldInfo.FieldType.IsAssignableFrom(newValue.GetType()))
                    {
                        fieldInfo.SetValue(obj, newValue);
                        return true;
                    }
                }
            }
            return false;
        }

        public static object GetPropertyValue(this object obj, string propertyName)
        {
            var propInfo = obj.GetType().GetProperty(propertyName);
            if (propInfo != null)
            {
                return propInfo.GetValue(obj);
            }
            return null;
        }

        public static (bool, T) GetPropertyValue<T>(this object obj, string propertyName)
        {
            var propInfo = obj.GetType().GetProperty(propertyName);
            if (propInfo != null)
            {
                if (typeof(T).IsAssignableFrom(propInfo.PropertyType))
                {
                    return (true, (T)propInfo.GetValue(obj));

                }
            }
            return (false, default(T));
        }

        public static bool SetPropertyValue(this object obj, string propertyName, object newProp)
        {
            var propInfo = obj.GetType().GetProperty(propertyName);
            if (propInfo != null)
            {
                if (propInfo.PropertyType.IsAssignableFrom(newProp.GetType()))
                {
                    propInfo.SetValue(obj, newProp);
                    return true;
                }
            }
            return false;
        }

        public static bool SetPropertyValue<T>(this object obj, string propertyName, T newProp)
        {
            var propInfo = obj.GetType().GetProperty(propertyName);
            if (propInfo != null)
            {
                if (propInfo.PropertyType.IsAssignableFrom(newProp.GetType()))
                {
                    propInfo.SetValue(obj, newProp);
                    return true;
                }
            }
            return false;
        }

        public static TInfo GetAttributeInfo<Tobj, Tatt, TInfo>(Tobj e, Func<Tatt, TInfo> infoGetter) where Tatt : Attribute
        {
            var type = e.GetType();
            var memInfo = type.GetMember(e.ToString());
            var attribute = memInfo[0].GetCustomAttribute(typeof(Tatt), false) as Tatt;
            if (attribute != null)
            {
                return infoGetter(attribute);
            }
            return default;
        }

        public static List<Type> GetAllDerivedTypes(this Type baseType)
        {
            var derivedTypes = new List<Type>();

            // 遍历当前应用程序域中所有已加载的程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    // 获取程序集中所有类型
                    var types = assembly.GetTypes();

                    // 筛选出派生自 baseType 的类型
                    var derived = types.Where(t =>
                        t != baseType && // 排除自身
                        baseType.IsGenericTypeDefinition ? // 处理泛型基类
                            (t.BaseType?.IsGenericType == true && t.BaseType.GetGenericTypeDefinition() == baseType) ||
                            t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType)
                        :
                        baseType.IsAssignableFrom(t) && // t 是 baseType 的子类或实现
                        !t.IsInterface && // 排除接口（可选）
                        !t.IsAbstract // 排除抽象类（可选）
                    );

                    derivedTypes.AddRange(derived);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // 某些程序集可能无法加载所有类型，忽略错误
                    Console.WriteLine($"无法加载程序集 {assembly.FullName} 中的所有类型: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // 其他异常处理
                    Console.WriteLine($"处理程序集 {assembly.FullName} 时出错: {ex.Message}");
                }
            }

            return derivedTypes;
        }
    }
}
