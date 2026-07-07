using System;
using System.Collections.Generic;

namespace JustAnotherWednesday;

public static class PPUTable
{
    private static readonly List<(int Min, int Max, int PPU)> Ranges;

    static PPUTable()
    {
        Ranges = new List<(int, int, int)>();
        const int total = 40_000_000;
        const int buckets = 50;
        int bucketSize = total / buckets; // 800_000
        double ppuStart = 50.0;
        double ppuEnd = 2000.0;
        double step = (ppuEnd - ppuStart) / (buckets - 1);

        for (int i = 0; i < buckets; i++)
        {
            int min = i * bucketSize;
            int max = (i == buckets - 1) ? total : ((i + 1) * bucketSize - 1);
            int ppu = (int)Math.Round(ppuStart + step * i);
            Ranges.Add((min, max, ppu));
        }
    }

    public static int GetBasePPU(int destiny)
    {
        if (destiny < 0) return Ranges[0].PPU;
        foreach (var r in Ranges)
        {
            if (destiny >= r.Min && destiny <= r.Max) return r.PPU;
        }
        // destiny beyond covered range -> return highest PPU
        return Ranges[^1].PPU;
    }
}
