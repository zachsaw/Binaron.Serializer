# Binary Object Notation (Version 4)
The Binary Object Notation (Binaron) is a little-endian binary format. Little-endian was chosen for performance reasons to match the most common hardware architecture implementations in the world - i.e. x86 and ARM. Note that ARM can theoretically run in both big or little-endian mode but most implementations and software ecosystem only support little-endian mode.

Strings are encoded in UTF16 (also little-endian) with surrogate pairs. Again, this was chosen for performance reasons as the most widely used high performance languages C# and Java use UTF16 with surrogate pairs their internal string representation. Fortunately, languages like Javascript and Python can also handle UTF16 these days and the performance penalty for them aren't as pronounced given that they would have to do the same work to convert UTF8 to binary. In other words, they would still just be as slow regardless of the encoding type.

The format can be surmised as,

`[Object Type (Byte)] [Object Value (n/a if object type == Null)]`

This type and value pair will be referred to as simply `[Object]` from here on.

Object type is one of the following.
```
    Null            = 0,
    
    Object          = 8 + 0,
    Dictionary      = 8 + 1,
    List            = 8 + 2,
    Enumerable      = 8 + 3,
    String          = 8 + 4,
    
    CustomObject    = 32 + 0,
    HList           = 32 + 1, // homogeneous list
    HEnumerable     = 32 + 2, // homogeneous enumerable
    
    Char            = 64 + 0,
    Byte            = 64 + 1,
    SByte           = 64 + 2,
    UShort          = 64 + 3,
    Short           = 64 + 4,
    UInt            = 64 + 5,
    Int             = 64 + 6,
    ULong           = 64 + 7,
    Long            = 64 + 8,
    Float           = 64 + 9,
    Double          = 64 + 10,
    Decimal         = 64 + 11,
    Bool            = 64 + 12,
    DateTime        = 64 + 13,
    Guid            = 64 + 14
```

EnumerableType (byte) is defined as follows and is used for Object and Enumerable types:
```
End     = 0,
HasItem = 1
```

## Object Field Formats
### Null
Indicates a null object, hence no `[Object]` field.

### Object
`[EnumerableType.HasItem (Byte)] [String] [Object] ... [EnumerableType.End (Byte)]`

e.g. `[HasItem] [PropertyFoo] [Value Object of PropertyFoo] [PropertyBar] [Value Object of PropertyBar] [End]` - 2 items

e.g. `[End]` - no items

### CustomObject
`[Value] [Object]`

Value is a custom identifier of a value of any type supported by the Binary Object Notation.
Object is the same as the object format above.

The CustomObject type is to annotate the object with an identifier so we could deserialize with additional information.
This is typically used to support polymorphism.

e.g. 

### Dictionary
`[Length (Int)] [Key Object] [Value Object] ...`

e.g. `[2] [Key Object1] [Value Object1] [Key Object2] [Value Object2]` - 2 items

e.g. `[0]` - no items

### List
`[Length (Int)] [Object] ...`

e.g. `[2] [Object1] [Object2]` - 2 items

e.g. `[0]` - no items

### Enumerable
`[EnumerableType.HasItem (Byte)] [Object] ... [EnumerableType.End (Byte)]`

e.g. `[HasItem] [Object] [HasItem] [Object] [End]` - 2 items

e.g. `[End]` - no items

### HList (Homogeneous List)
`[Length (Int)] [Object Type (Byte)] [Object Value 0] [Object Value 1] ... [Object Value n]`

A homogeneous list is a list that has elements of the same type.

e.g. `[2] [Int] [123] [234]`

The types that can be in a HList are restricted to the following,
- String
- Char
- Byte
- SByte
- UShort
- Short
- UInt
- Int
- ULong
- Long
- Float
- Double
- Decimal
- Bool
- DateTime
- Guid

### HEnumerable (Homogeneous Enumerable)
`[Object Type] [EnumerableType.HasItem (Byte)] [Object Value] ... [EnumerableType.End (Byte)]`

A homogeneous enumerable is the enumerable equivalent of a HList. It has the same type restrictions as the HList.

e.g. `[Int] [HasItem] [234] [HasItem] [345] [End]` - 2 items

e.g. `[Int] [End]` - no items

### Custom Object
`[String] [Object]`

A custom object is used to provide information about the object proceeding the string information. This is typically used to support polymorphism where the string component can be used to store the actual type of the class.

e.g. `[MySubClass] [Object]`

### String
`[String Length (Int)] [UTF16 encoded string]`

e.g. `[5] [HELLO]`

In a homogeneous List or Enumerable, a negative number for string length indicates null.

### Char
`[UTF encoded char]`

### Byte
`[Byte]`

Unsigned 8-bit integer value

### SByte
`[Signed Byte]`

Signed 8-bit integer value

### UShort
`[Unsigned Short]`

Unsigned 16-bit integer value

### Short
`[Short]`

Signed 16-bit integer value

### UInt
`[Unsigned Integer]`

Unsigned 32-bit integer value

### Int
`[Integer]`

Signed 32-bit integer value

### ULong
`[Unsigned Long]`

Unsigned 64-bit integer value

### Long
`[Long]`

Signed 64-bit integer value

### Float
`[Single Precision Float]`

32-bit floating point value

### Double
`[Double Precision Float]`

64-bit floating point value

### Decimal
`[Decimal 128]`

128-bit decimal floating point value - [IEEE 754-2008](https://en.wikipedia.org/wiki/Decimal128_floating-point_format)

### Bool
`[Boolean]`

8-bit boolean value: 0 (false) - 1 (true)

### DateTime
`[DateTime]`

Unsigned 64-bit integer value representing the number of 100-nanosecond intervals that have elapsed since 12:00:00 midnight, January 1, 0001 (0:00:00 UTC on January 1, 0001, in the Gregorian calendar)

|   |Raw Value|Date Time|
|--:|:-------:|:-------:|
|Min|0|1 Jan 0001 12:00:00 AM|
|Max|3,155,378,975,999,999,999|(31 Dec 9999 11:59:59 PM)|

### Guid
`[Guid]`

128-bit little-endian value representing a GUID

## Type Promotions

It is up to the implementation of the deserializer to implement type promotions. In the case of the default implementation included in this repository, the following types are promoted transparently.

|From|To |
|:--:|:--|
|sbyte  |short, int, long, float, double, or decimal                     |
|byte   |short, ushort, int, uint, long, ulong, float, double, or decimal|
|short  |int, long, float, double, or decimal                            |
|ushort |int, uint, long, ulong, float, double, or decimal               |
|int    |long, double, or decimal                                        |
|uint   |long, ulong, double, or decimal                                 |
|long   |decimal                                                         |
|ulong  |decimal                                                         |
|float  |double                                                          |

In other words, automatic promotions only occur when we can guarantee that there is no precision loss.
