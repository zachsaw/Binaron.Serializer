using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Binaron.Serializer.Accessors;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;

namespace Binaron.Serializer
{
    internal static class Serializer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask WriteValue<T>(WriterState writer, T val)
        {
            if (val == null)
            {
                await writer.Write((byte) SerializedType.Null);
                return;
            }

            await WriteNonNullValue(writer, val);
        }

        public static async ValueTask WriteValue(WriterState writer, object val)
        {
            if (val == null)
            {
                await writer.Write((byte) SerializedType.Null);
                return;
            }

            await WriteNonNullValue(writer, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask WriteNonNullValue<T>(WriterState writer, T val)
        {
            if (await WritePrimitive(writer, val))
                return;

            await WriteNonPrimitive(writer, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask WriteNonNullValue(WriterState writer, object val)
        {
            if (await WritePrimitive(writer, val))
                return;

            await WriteNonPrimitive(writer, val);
        }

        private interface INonPrimitiveWriter<in T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ValueTask Write(WriterState writer, T val);
        }

        private static class GetNonPrimitiveWriterGeneric<T>
        {
            public static readonly INonPrimitiveWriter<T> Writer = NonPrimitiveWriters.CreateWriter<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask WriteNonPrimitive<T>(WriterState writer, T val) => await GetNonPrimitiveWriterGeneric<T>.Writer.Write(writer, val);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteNonPrimitive(WriterState writer, object val)
        {
            switch (val)
            {
                case IDictionary<string, object> dictAsObj: // treat IDictionary<string, object> as Object type
                    await WriteDictionaryAsObject(writer, dictAsObj);
                    break;
                case IDictionary _ when await GenericWriter.WriteDictionary(writer, val):
                    break;
                case IDictionary dictionary:
                    await WriteDictionary(writer, dictionary);
                    break;
                case ICollection _ when await GenericWriter.WriteDictionary(writer, val):
                    break;
                case ICollection list:
                    await WriteList(writer, list);
                    break;
                case IEnumerable enumerable:
                    await WriteEnumerable(writer, enumerable);
                    break;
                default:
                    await WriteObject(writer, val);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteObject<T>(WriterState writer, T val)
        {
            await writer.Write((byte) SerializedType.Object);

            foreach (var getter in GetterHandler.GetterHandlers<T>.Getters)
                getter.Handle(writer, val);

            await writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteObject(WriterState writer, object val)
        {
            await writer.Write((byte) SerializedType.Object);

            foreach (var getter in GetterHandler.GetGetterHandlers(val.GetType()))
                getter.Handle(writer, val);

            await writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteDictionaryAsObject(WriterState writer, IDictionary<string, object> dictionary)
        {
            await writer.Write((byte) SerializedType.Object);

            foreach (var (key, value) in dictionary.Keys.Zip(dictionary.Values))
            {
                await writer.Write((byte) EnumerableType.HasItem);
                await writer.WriteString(key);
                await WriteValue(writer, value);
            }

            await writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteDictionary(WriterState writer, IDictionary dictionary)
        {
            await writer.Write((byte) SerializedType.Dictionary);
            await writer.Write(dictionary.Count);
            foreach (var (key, value) in dictionary.Keys.Zip(dictionary.Values))
            {
                await WriteValue(writer, key);
                await WriteValue(writer, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteList<T>(WriterState writer, ICollection list)
        {
            await writer.Write((byte) SerializedType.List);
            await writer.Write(list.Count);
            if (await GenericWriter.WriteList<T>(writer, list))
                return;

            // fallback to non-generic version
            await WriteListFallback(writer, list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteList(WriterState writer, ICollection list)
        {
            await writer.Write((byte) SerializedType.List);
            await writer.Write(list.Count);
            if (await GenericWriter.WriteList(writer, list))
                return;

            // fallback to non-generic version
            await WriteListFallback(writer, list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteListFallback(WriterState writer, ICollection list)
        {
            foreach (var item in list)
                await WriteValue(writer, item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteEnumerable<T>(WriterState writer, IEnumerable enumerable)
        {
            await writer.Write((byte) SerializedType.Enumerable);

            // ReSharper disable once PossibleMultipleEnumeration
            if (await GenericWriter.WriteEnumerable<T>(writer, enumerable))
                return;

            // fallback to non-generic version (we are not enumerating twice)
            // ReSharper disable once PossibleMultipleEnumeration
            await WriteEnumerableFallback(writer, enumerable);

            await writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteEnumerable(WriterState writer, IEnumerable enumerable)
        {
            await writer.Write((byte) SerializedType.Enumerable);

            // ReSharper disable once PossibleMultipleEnumeration
            if (await GenericWriter.WriteEnumerable(writer, enumerable))
                return;

            // fallback to non-generic version (we are not enumerating twice)
            // ReSharper disable once PossibleMultipleEnumeration
            await WriteEnumerableFallback(writer, enumerable);

            await writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteEnumerableFallback(WriterState writer, IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                await writer.Write((byte) EnumerableType.HasItem);
                await WriteValue(writer, item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask<bool> WritePrimitive(WriterState writer, object value)
        {
            var type = value.GetType();

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    await Writer.Write(writer, (string) value);
                    return true;
                case TypeCode.UInt32:
                    await Writer.Write(writer, (uint) value);
                    return true;
                case TypeCode.Int32:
                    await Writer.Write(writer, (int) value);
                    return true;
                case TypeCode.Byte:
                    await Writer.Write(writer, (byte) value);
                    return true;
                case TypeCode.SByte:
                    await Writer.Write(writer, (sbyte) value);
                    return true;
                case TypeCode.UInt16:
                    await Writer.Write(writer, (ushort) value);
                    return true;
                case TypeCode.Int16:
                    await Writer.Write(writer, (short) value);
                    return true;
                case TypeCode.Int64:
                    await Writer.Write(writer, (long) value);
                    return true;
                case TypeCode.UInt64:
                    await Writer.Write(writer, (ulong) value);
                    return true;
                case TypeCode.Single:
                    await Writer.Write(writer, (float) value);
                    return true;
                case TypeCode.Double:
                    await Writer.Write(writer, (double) value);
                    return true;
                case TypeCode.Decimal:
                    await Writer.Write(writer, (decimal) value);
                    return true;
                case TypeCode.Boolean:
                    await Writer.Write(writer, (bool) value);
                    return true;
                case TypeCode.DateTime:
                    await Writer.Write(writer, (DateTime) value);
                    return true;
                case TypeCode.Char:
                    await Writer.Write(writer, (char) value);
                    return true;
                default:
                    return false;
            }
        }

        private static class NonPrimitiveWriters
        {
            public static INonPrimitiveWriter<T> CreateWriter<T>()
            {
                var types = GenericType.GetIDictionaryWriterGenericTypes<T>.Types;
                if (types.KeyType != null)
                {
                    if (types.KeyType == typeof(string) && types.ValueType == typeof(object))
                        return new DictionaryAsObjectWriter<T>();

                    return new GenericDictionaryWriter<T>();
                }

                if (typeof(IDictionary).IsAssignableFrom(typeof(T)))
                    return (INonPrimitiveWriter<T>) Activator.CreateInstance(typeof(DictionaryWriter<>).MakeGenericType(typeof(T)));

                if (typeof(ICollection).IsAssignableFrom(typeof(T)))
                    return (INonPrimitiveWriter<T>) Activator.CreateInstance(typeof(ListWriter<>).MakeGenericType(typeof(T)));

                if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                    return (INonPrimitiveWriter<T>) Activator.CreateInstance(typeof(EnumerableWriter<>).MakeGenericType(typeof(T)));

                return new ObjectWriter<T>();
            }

            private sealed class DictionaryAsObjectWriter<T> : INonPrimitiveWriter<T>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public async ValueTask Write(WriterState writer, T val) => await WriteDictionaryAsObject(writer, (IDictionary<string, object>) val);
            }

            private sealed class GenericDictionaryWriter<T> : INonPrimitiveWriter<T>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public async ValueTask Write(WriterState writer, T val) => await GenericWriter.WriteDictionary(writer, val);
            }

            private sealed class DictionaryWriter<T> : INonPrimitiveWriter<T> where T : IDictionary
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public async ValueTask Write(WriterState writer, T val) => await WriteDictionary(writer, val);
            }

            private sealed class ListWriter<T> : INonPrimitiveWriter<T> where T : ICollection
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public async ValueTask Write(WriterState writer, T val) => await WriteList<T>(writer, val);
            }

            private sealed class EnumerableWriter<T> : INonPrimitiveWriter<T> where T : IEnumerable
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public async ValueTask Write(WriterState writer, T val) => await WriteEnumerable<T>(writer, val);
            }

            private sealed class ObjectWriter<T> : INonPrimitiveWriter<T>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public async ValueTask Write(WriterState writer, T val) => await WriteObject(writer, val);
            }
        }
    }
}