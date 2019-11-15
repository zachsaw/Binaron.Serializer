using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Accessors;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;

namespace Binaron.Serializer
{
    internal static class Serializer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteValue<T>(WriterState writer, T val)
        {
            if (val == null)
            {
                writer.Write((byte) SerializedType.Null);
                return;
            }

            WriteNonNullValue(writer, val);
        }

        public static void WriteValue(WriterState writer, object val)
        {
            if (val == null)
            {
                writer.Write((byte) SerializedType.Null);
                return;
            }

            WriteNonNullValue(writer, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNonNullValue<T>(WriterState writer, T val)
        {
            if (WritePrimitive(writer, val))
                return;

            WriteNonPrimitive(writer, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteNonNullValue(WriterState writer, object val)
        {
            if (WritePrimitive(writer, val))
                return;

            WriteNonPrimitive(writer, val);
        }

        private interface INonPrimitiveWriter<in T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Write(WriterState writer, T val);
        }

        private static class GetNonPrimitiveWriterGeneric<T>
        {
            public static readonly INonPrimitiveWriter<T> Writer = NonPrimitiveWriters.CreateWriter<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNonPrimitive<T>(WriterState writer, T val) => GetNonPrimitiveWriterGeneric<T>.Writer.Write(writer, val);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteNonPrimitive(WriterState writer, object val)
        {
            switch (val)
            {
                case IDictionary _ when GenericWriter.WriteDictionary(writer, val):
                    break;
                case IDictionary dictionary:
                    WriteDictionary(writer, dictionary);
                    break;
                case ICollection _ when GenericWriter.WriteDictionary(writer, val):
                    break;
                case ICollection list:
                    WriteList(writer, list);
                    break;
                case IEnumerable enumerable:
                    WriteEnumerable(writer, enumerable);
                    break;
                default:
                    WriteObject(writer, val);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteObject<T>(WriterState writer, T val)
        {
            writer.Write((byte) SerializedType.Object);

            foreach (var getter in GetterHandler.GetterHandlers<T>.Getters)
                getter.Handle(writer, val);

            writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteObject(WriterState writer, object val)
        {
            writer.Write((byte) SerializedType.Object);

            foreach (var getter in GetterHandler.GetGetterHandlers(val.GetType()))
                getter.Handle(writer, val);

            writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteDictionary(WriterState writer, IDictionary dictionary)
        {
            writer.Write((byte) SerializedType.Dictionary);
            writer.Write(dictionary.Count);
            foreach (var (key, value) in dictionary.Keys.Zip(dictionary.Values))
            {
                WriteValue(writer, key);
                WriteValue(writer, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteList<T>(WriterState writer, ICollection list)
        {
            writer.Write((byte) SerializedType.List);
            writer.Write(list.Count);
            if (GenericWriter.WriteList<T>(writer, list))
                return;

            // fallback to non-generic version
            WriteListFallback(writer, list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteList(WriterState writer, ICollection list)
        {
            writer.Write((byte) SerializedType.List);
            writer.Write(list.Count);
            if (GenericWriter.WriteList(writer, list))
                return;

            // fallback to non-generic version
            WriteListFallback(writer, list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteListFallback(WriterState writer, ICollection list)
        {
            foreach (var item in list)
                WriteValue(writer, item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteEnumerable<T>(WriterState writer, IEnumerable enumerable)
        {
            writer.Write((byte) SerializedType.Enumerable);

            // ReSharper disable once PossibleMultipleEnumeration
            if (GenericWriter.WriteEnumerable<T>(writer, enumerable))
                return;

            // fallback to non-generic version (we are not enumerating twice)
            // ReSharper disable once PossibleMultipleEnumeration
            WriteEnumerableFallback(writer, enumerable);

            writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteEnumerable(WriterState writer, IEnumerable enumerable)
        {
            writer.Write((byte) SerializedType.Enumerable);

            // ReSharper disable once PossibleMultipleEnumeration
            if (GenericWriter.WriteEnumerable(writer, enumerable))
                return;

            // fallback to non-generic version (we are not enumerating twice)
            // ReSharper disable once PossibleMultipleEnumeration
            WriteEnumerableFallback(writer, enumerable);

            writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteEnumerableFallback(WriterState writer, IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                writer.Write((byte) EnumerableType.HasItem);
                WriteValue(writer, item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool WritePrimitive(WriterState writer, object value)
        {
            var type = value.GetType();

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    Writer.Write(writer, (string) value);
                    return true;
                case TypeCode.UInt32:
                    Writer.Write(writer, (uint) value);
                    return true;
                case TypeCode.Int32:
                    Writer.Write(writer, (int) value);
                    return true;
                case TypeCode.Byte:
                    Writer.Write(writer, (byte) value);
                    return true;
                case TypeCode.SByte:
                    Writer.Write(writer, (sbyte) value);
                    return true;
                case TypeCode.UInt16:
                    Writer.Write(writer, (ushort) value);
                    return true;
                case TypeCode.Int16:
                    Writer.Write(writer, (short) value);
                    return true;
                case TypeCode.Int64:
                    Writer.Write(writer, (long) value);
                    return true;
                case TypeCode.UInt64:
                    Writer.Write(writer, (ulong) value);
                    return true;
                case TypeCode.Single:
                    Writer.Write(writer, (float) value);
                    return true;
                case TypeCode.Double:
                    Writer.Write(writer, (double) value);
                    return true;
                case TypeCode.Decimal:
                    Writer.Write(writer, (decimal) value);
                    return true;
                case TypeCode.Boolean:
                    Writer.Write(writer, (bool) value);
                    return true;
                case TypeCode.DateTime:
                    Writer.Write(writer, (DateTime) value);
                    return true;
                case TypeCode.Char:
                    Writer.Write(writer, (char) value);
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
                    return new GenericDictionaryWriter<T>();

                if (typeof(IDictionary).IsAssignableFrom(typeof(T)))
                    return (INonPrimitiveWriter<T>) Activator.CreateInstance(typeof(DictionaryWriter<>).MakeGenericType(typeof(T)));

                if (typeof(ICollection).IsAssignableFrom(typeof(T)))
                    return (INonPrimitiveWriter<T>) Activator.CreateInstance(typeof(ListWriter<>).MakeGenericType(typeof(T)));

                if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                    return (INonPrimitiveWriter<T>) Activator.CreateInstance(typeof(EnumerableWriter<>).MakeGenericType(typeof(T)));

                return new ObjectWriter<T>();
            }

            private class GenericDictionaryWriter<T> : INonPrimitiveWriter<T>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => GenericWriter.WriteDictionary(writer, val);
            }

            private class DictionaryWriter<T> : INonPrimitiveWriter<T> where T : IDictionary
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => WriteDictionary(writer, val);
            }

            private class ListWriter<T> : INonPrimitiveWriter<T> where T : ICollection
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => WriteList<T>(writer, val);
            }

            private class EnumerableWriter<T> : INonPrimitiveWriter<T> where T : IEnumerable
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => WriteEnumerable<T>(writer, val);
            }

            private class ObjectWriter<T> : INonPrimitiveWriter<T>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => WriteObject(writer, val);
            }
        }
    }
}