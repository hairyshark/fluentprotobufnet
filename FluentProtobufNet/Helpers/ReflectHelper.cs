using System;
using System.Reflection;

namespace FluentProtobufNet.Helpers
{
    public class ReflectHelper
    {
        public const BindingFlags AnyVisibilityInstance = BindingFlags.Instance | BindingFlags.Public |
                                                          BindingFlags.NonPublic;
        
        private static readonly Type[] MapParameters = Type.EmptyTypes;

        public static ConstructorInfo GetClassMapConstructor(Type type)
        {
            if (IsAbstractClass(type))
                return null;

            try
            {
                ConstructorInfo constructor =
                    type.GetConstructor(AnyVisibilityInstance, null, CallingConventions.HasThis, MapParameters, null);

                return constructor;
            }
            catch (Exception e)
            {
                throw new InstantiationException("A default (no-arg) constructor could not be found for: ", e, type);
            }
        }

        public static bool IsAbstractClass(Type type)
        {
            return (type.IsAbstract || type.IsInterface);
        }

        public static ConstructorInfo GetSubclassMapConstructor(Type type)
        {
            if (IsAbstractClass(type))
                return null;

            try
            {
                ConstructorInfo constructor = type.GetConstructor(new[] { typeof(int) });

                return constructor;
            }
            catch (Exception e)
            {
                throw new InstantiationException("A default (no-arg) constructor could not be found for: ", e, type);
            }
        }
    }
}