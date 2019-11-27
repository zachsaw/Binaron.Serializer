using System;
using System.Diagnostics;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Binaron.Serializer;
using BinSerializerTest.DtoSamples;
using Newtonsoft.Json;

namespace BinSerializerTest
{
    public static class Program
    {
        public class BinaronVsJsonTrainedWeights
        {
            private const int Loop = 5;
            private readonly TrainedWeights trainedWeights = TrainedWeights.Create();
            
            [GlobalSetup]
            public void Setup()
            {
                // warm-up
                NewtonsoftJsonTest_Validate(trainedWeights, Validate);
                BinaronTest_Validate(trainedWeights, Validate);
            }
            
            [Benchmark]
            public void Json_Serialize() => NewtonsoftJsonTest_Serialize(trainedWeights, Loop);

            [Benchmark]
            public void Binaron_Serialize() => BinaronTest_Serialize(trainedWeights, Loop);

            [Benchmark]
            public void Json_Deserialize() => NewtonsoftJsonTest_Deserialize(trainedWeights, Loop);

            [Benchmark]
            public void Binaron_Deserialize() => BinaronTest_Deserialize(trainedWeights, Loop);
        }

        public class BinaronVsJsonBook
        {
            private const int Loop = 200;
            private readonly Book book = Book.Create();

            [GlobalSetup]
            public void Setup()
            {
                // warm-up
                for (var i = 0; i < Loop; i++)
                {
                    NewtonsoftJsonTest_Validate(book, Validate);
                    BinaronTest_Validate(book, Validate);
                }
            }

            [Benchmark]
            public void Json_Serialize() => NewtonsoftJsonTest_Serialize(book, Loop);

            [Benchmark]
            public void Binaron_Serialize() => BinaronTest_Serialize(book, Loop);

            [Benchmark]
            public void Json_Deserialize() => NewtonsoftJsonTest_Deserialize(book, Loop);

            [Benchmark]
            public void Binaron_Deserialize() => BinaronTest_Deserialize(book, Loop);
        }

        public static void Main()
        {
            BenchmarkRunner.Run<BinaronVsJsonTrainedWeights>();
            BenchmarkRunner.Run<BinaronVsJsonBook>();
        }

        private static void BinaronTest_Serialize<T>(T input, int loop)
        {
            using var stream = new MemoryStream();
            for (var i = 0; i < loop; i++)
            {
                BinaronConvert.Serialize(input, stream, new SerializerOptions {SkipNullValues = true});
                stream.Position = 0;
            }
        }

        private static void BinaronTest_Deserialize<T>(T input, int loop)
        {
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(input, stream, new SerializerOptions {SkipNullValues = true});
            stream.Position = 0;

            for (var i = 0; i < loop; i++)
            {
                var _ = BinaronConvert.Deserialize<T>(stream);
                stream.Position = 0;
            }
        }

        private static void BinaronTest_Validate<T>(T input, Action<T> validate)
        {
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(input, stream, new SerializerOptions {SkipNullValues = true});
            stream.Position = 0;

            var value = BinaronConvert.Deserialize<T>(stream);
            validate(value);
        }

        private static void NewtonsoftJsonTest_Serialize<T>(T input, int loop)
        {
            using var stream = new MemoryStream();
            for (var i = 0; i < loop; i++)
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                using var jsonWriter = new JsonTextWriter(writer);
                {
                    var ser = new JsonSerializer {NullValueHandling = NullValueHandling.Ignore};
                    ser.Serialize(jsonWriter, input);
                    jsonWriter.Flush();
                }
                stream.Position = 0;
            }
        }

        private static void NewtonsoftJsonTest_Deserialize<T>(T input, int loop)
        {
            using var stream = new MemoryStream();
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                using var jsonWriter = new JsonTextWriter(writer);
                {
                    var ser = new JsonSerializer {NullValueHandling = NullValueHandling.Ignore};
                    ser.Serialize(jsonWriter, input);
                    jsonWriter.Flush();
                }
                stream.Position = 0;
            }
            for (var i = 0; i < loop; i++)
            {
                using var reader = new StreamReader(stream, leaveOpen: true);
                using var jsonReader = new JsonTextReader(reader);
                var ser = new JsonSerializer();
                var _ = ser.Deserialize<T>(jsonReader);
                stream.Position = 0;
            }
        }

        private static void NewtonsoftJsonTest_Validate<T>(T input, Action<T> validate)
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, leaveOpen: true);
            using var jsonWriter = new JsonTextWriter(writer);
            {
                var ser = new JsonSerializer {NullValueHandling = NullValueHandling.Ignore};
                ser.Serialize(jsonWriter, input);
                jsonWriter.Flush();
            }
            stream.Position = 0;
            {
                using var reader = new StreamReader(stream, leaveOpen: true);
                using var jsonReader = new JsonTextReader(reader);
                var ser = new JsonSerializer();
                var value = ser.Deserialize<T>(jsonReader);
                validate(value);
            }
        }
        
        private static void Validate(TrainedWeights value)
        {
            var source = TrainedWeights.Create();
            Trace.Assert(value.Weights.Length == source.Weights.Length);

            for (var i = 0; i < source.Weights.Length; i++) 
                Trace.Assert(Math.Abs(value.Weights[i] - source.Weights[i]) < double.Epsilon);
        }
        
        private static void Validate(Book value)
        {
            Trace.Assert(value.Changes.Count == 3);

            Trace.Assert((string) value.Metadata["Matrix"] == "Neo");

            foreach (var page in value.Pages)
            foreach (var note in page.Notes)
            {
                Trace.Assert(note.Headnote.Note != null);
            }

            Trace.Assert(value.Genres[0] == Genre.Action);
            Trace.Assert(value.Genres[1] == Genre.Comedy);
        }
    }
}
