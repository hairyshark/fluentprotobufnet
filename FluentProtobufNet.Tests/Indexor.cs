using System.Reflection;

namespace FluentProtobufNet.Tests
{
    public static class Indexor
    {
        private static volatile int _seed = 1;

        public static int GetIndex(this object info)
        {
            return _seed++;
        }
    }
}
