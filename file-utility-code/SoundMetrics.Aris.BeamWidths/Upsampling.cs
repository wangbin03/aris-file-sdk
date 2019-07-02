﻿// Copyright 2014-2019 Sound Metrics Corp. All Rights Reserved.

using System;
using System.Linq;

namespace SoundMetrics.Aris.BeamWidths
{
    using UpsampleWidth = Int32;

    public static class Upsampling
    {
        /// <summary>
        /// Creates an array of widths per beam for upsampled display.
        /// </summary>
        /// <param name="scale">
        /// The multiplier for upsampling.
        /// </param>
        /// <param name="metrics">
        /// Beam width information from SoundMetrics.Aris.BeamWidths.
        /// </param>
        /// <param name="normalOrdering">
        /// Returns the widths in normal ordering, which puts beam zero on the right
        /// (normal for ARIS).
        /// Passing false will reverse the ordering, putting beam zero on the left.
        /// </param>
        /// <returns>An ordered array of upsampel widths.</returns>
        public static UpsampleWidth[] CalculateUpsampleWidths(
            int scale,
            BeamInfo[] metrics,
            bool normalOrdering = true)
        {
            if (scale < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(scale));
            }

            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            if (metrics.Length == 0)
            {
                throw new ArgumentException("empty array", nameof(metrics));
            }


            var beamCount = metrics.Length;
            var upsampledBeamCount = scale * beamCount;

            var leftmostAngle = metrics.First().Left;
            var rightmostAngle = metrics.Last().Right;

            var fov = rightmostAngle - leftmostAngle;
            var upsampledPixelWidth = fov / upsampledBeamCount;

            var upsampleWidths = new UpsampleWidth[beamCount]; // contains # of new pixels mapped to each original beam

            // From 0 to N-1, calculate the width from far left to the right of the
            // current beam; divide that by the upsampled pixel width to get a pixel count.
            // Subtract previously assigned pixels.Assign the remainder to the current beam.

            var pixelsAssigned = 0;

            for (var beamNum = 0; beamNum < beamCount; ++beamNum)
            {
                var localRight = metrics[beamNum].Right - leftmostAngle;
                var allPixelsToLeft = (int)(Math.Round(localRight / upsampledPixelWidth));
                var pixelsThisBeam = allPixelsToLeft - pixelsAssigned;

                upsampleWidths[beamNum] = pixelsThisBeam;
                pixelsAssigned += pixelsThisBeam;
            }

            if (upsampleWidths.Sum() != upsampledBeamCount)
            {
                throw new ApplicationException("invalid total count of upsamples");
            }

            Array.Reverse(upsampleWidths);
            return upsampleWidths;
        }
    }
}
