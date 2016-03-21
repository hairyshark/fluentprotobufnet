namespace FluentProtobufNet.Mapping
{
    public static class SeededIndexor
    {
        private static volatile int _seed = 1;

        public static int GetIndex(this object info)
        {
            return _seed++;
        }
    }
}
