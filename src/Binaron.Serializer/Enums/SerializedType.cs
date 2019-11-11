namespace Binaron.Serializer.Enums
{
    internal enum SerializedType : byte
    {
        Null       = 0,
        Object     = 8 + 0,
        Dictionary = 8 + 1,
        List       = 8 + 2,
        Enumerable = 8 + 3,
        String     = 8 + 4,
        Char       = 64 + 0,
        Byte       = 64 + 1,
        SByte      = 64 + 2,
        UShort     = 64 + 3,
        Short      = 64 + 4,
        UInt       = 64 + 5,
        Int        = 64 + 6,
        ULong      = 64 + 7,
        Long       = 64 + 8,
        Float      = 64 + 9,
        Double     = 64 + 10,
        Decimal    = 64 + 11,
        Bool       = 64 + 12,
        DateTime   = 64 + 13
    }
}