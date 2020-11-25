using System;

namespace BinSerializerTest.DtoSamples
{
    public class TrainedWeights
    {
        public double[] Weights;

        public static TrainedWeights Create()
        {
            var rnd = new Random(15);
            var result = new TrainedWeights {Weights = new double[64 * 1024]}; // 64k weights (e.g. a small CNN model)
            for (var i = 0; i < result.Weights.Length; i++)
                result.Weights[i] = rnd.NextDouble();

            return result;
        }
    }
}