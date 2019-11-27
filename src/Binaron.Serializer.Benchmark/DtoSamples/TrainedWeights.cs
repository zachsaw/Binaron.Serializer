using System;

namespace BinSerializerTest.DtoSamples
{
    public class TrainedWeights
    {
        public double[] Weights = new double[128*1024]; // 128k weights (e.g. a small CNN model)

        public static TrainedWeights Create()
        {
            var rnd = new Random(15);
            var result = new TrainedWeights();
            for (var i = 0; i < result.Weights.Length; i++)
                result.Weights[i] = rnd.NextDouble();

            return result;
        }
    }
}