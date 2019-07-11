using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace SoundMetrics.Aris.BeamWidths.Tests
{
    [TestClass]
    public class UpsamplingTests
    {
        [TestMethod]
        public void ValidateKnownConfigsDontThrow()
        {
            const uint scale = 8;

            foreach (var config in Metrics.AllBeamConfigurations)
            {
                var (_, beamCount, _) = config;

                var metrics = Metrics.GetBeamInformation(config);
                var upsampleInfo = Upsampling.CalculateUpsampleWidths(scale, metrics);

                var msg = $"in {config}";
                Assert.IsNotNull(upsampleInfo.UpsampledWidths, msg);
                Assert.AreEqual(beamCount, (uint)upsampleInfo.UpsampledWidths.Length, msg);
                Assert.AreEqual(
                    beamCount * scale,
                    (uint)upsampleInfo.UpsampledWidths.Cast<int>().Sum(), // no Sum for uint??
                    msg);

                Console.Write($"Upsamples for {config}: ");
                Console.WriteLine(String.Join(", ", upsampleInfo.UpsampledWidths));
                Console.WriteLine();
            }
        }
    }
}
