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
        public class BinaronVsJson
        {
            private readonly Book book = Book.Create();

            [GlobalSetup]
            public void Setup()
            {
                // warm-up
                for (var i = 0; i < 50; i++)
                {
                    NewtonsoftJsonTest(book);
                    BinaronTest(book);
                }
            }

            [Benchmark]
            public void Json()
            {
                for (var i = 0; i < 50; i++)
                    NewtonsoftJsonTest(book);
            }

            [Benchmark]
            public void Binaron()
            {
                for (var i = 0; i < 50; i++)
                    BinaronTest(book);
            }
        }

        public static void Main()
        {
            BenchmarkRunner.Run<BinaronVsJson>();
        }

        private static void BinaronTest(Book input)
        {
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(input, stream);
            stream.Position = 0;
            var book = BinaronConvert.Deserialize<Book>(stream);
            
            Trace.Assert(book.Changes.Count == 3);

            Trace.Assert((string) book.Metadata["Matrix"] == "Neo");

            foreach (var page in book.Pages)
            foreach (var note in page.Notes)
            {
                Trace.Assert(note.Headnote.Note != null);
            }

            Trace.Assert(book.Genres[0] == Genre.Action);
            Trace.Assert(book.Genres[1] == Genre.Comedy);
        }

        private static void NewtonsoftJsonTest(Book input)
        {
            using var stream = new MemoryStream();
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                using var jsonWriter = new JsonTextWriter(writer);
                {
                    var ser = new JsonSerializer();
                    ser.Serialize(jsonWriter, input);
                    jsonWriter.Flush();
                }
            }
            stream.Position = 0;
            {
                using var reader = new StreamReader(stream, leaveOpen: true);
                using var jsonReader = new JsonTextReader(reader);
                var ser = new JsonSerializer();
                var book = ser.Deserialize<Book>(jsonReader);

                Trace.Assert(book.Changes.Count == 3);

                Trace.Assert((string) book.Metadata["Matrix"] == "Neo");

                foreach (var page in book.Pages)
                foreach (var note in page.Notes)
                {
                    Trace.Assert(note.Headnote.Note != null);
                }

                Trace.Assert(book.Genres[0] == Genre.Action);
                Trace.Assert(book.Genres[1] == Genre.Comedy);
            }
        }
    }
}
