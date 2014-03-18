﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.Mvc
{
    internal static class TypeExtensions
    {
#if NETFX_CORE || K10
        private static bool EqualTo([NotNull] this Type[] t1, [NotNull] Type[] t2)
        {
            if (t1.Length != t2.Length)
            {
                return false;
            }

            for (int idx = 0; idx < t1.Length; ++idx)
            {
                if (t1[idx] != t2[idx])
                {
                    return false;
                }
            }

            return true;
        }

        public static ConstructorInfo GetConstructor([NotNull] this Type type, Type[] types)
        {
            return type.GetTypeInfo().DeclaredConstructors
                                     .Where(c => c.IsPublic)
                                     .SingleOrDefault(c => c.GetParameters()
                                                            .Select(p => p.ParameterType).ToArray().EqualTo(types));
        }
#endif

        public static Type ExtractGenericInterface([NotNull] this Type queryType, Type interfaceType)
        {
            Func<Type, bool> matchesInterface = t => t.IsGenericType() && t.GetGenericTypeDefinition() == interfaceType;
            return (matchesInterface(queryType)) ? queryType : queryType.GetInterfaces().FirstOrDefault(matchesInterface);
        }

#if NETFX_CORE || K10
        public static Type[] GetGenericArguments([NotNull] this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        public static Type[] GetInterfaces([NotNull] this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
        }
#endif

#if NETFX_CORE || K10
        public static bool IsAssignableFrom([NotNull] this Type type, [NotNull] Type c)
        {
            return type.GetTypeInfo().IsAssignableFrom(c.GetTypeInfo());
        }
#endif

        public static bool IsGenericType([NotNull] this Type type)
        {
#if NETFX_CORE || K10
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        public static bool IsInterface([NotNull] this Type type)
        {
#if NETFX_CORE || K10
            return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
        }

        public static bool IsValueType([NotNull] this Type type)
        {
#if NETFX_CORE || K10
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        public static bool IsCompatibleWith([NotNull] this Type type, object value)
        {
            return (value == null && AllowsNullValue(type)) ||
                (value != null && type.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()));
        }

        public static bool IsNullableValueType([NotNull] this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool AllowsNullValue([NotNull] this Type type)
        {
            return (!type.GetTypeInfo().IsValueType || IsNullableValueType(type));
        }

        public static bool HasStringConverter([NotNull] this Type type)
        {
            // TODO: This depends on TypeConverter which does not exist in the CoreCLR.
            // return TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
            TypeInfo typeInfo = type.GetTypeInfo();
            if (typeInfo.IsPrimitive || type == typeof(string))
            {
                return true;
            }
            if (IsNullableValueType(type) && HasStringConverter(type.GenericTypeArguments[0]))
            {
                // Nullable<T> where T is a primitive type or has a type converter
                return true;
            }
            return false;
        }

        public static Type[] GetTypeArgumentsIfMatch([NotNull] Type closedType, Type matchingOpenType)
        {
            TypeInfo closedTypeInfo = closedType.GetTypeInfo();
            if (!closedTypeInfo.IsGenericType)
            {
                return null;
            }

            Type openType = closedType.GetGenericTypeDefinition();
            return (matchingOpenType == openType) ? closedTypeInfo.GenericTypeArguments : null;
        }
    }
}