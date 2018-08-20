﻿using System;
using System.Collections.Generic;
using GrainInterfaces;
using Orleans;
using System.Threading.Tasks;
using GrainInterfaces.States;
using Orleans.Providers;

namespace Grains
{
    [StorageProvider(ProviderName = "ColorsStorage")]
    public class ColorGrain : Grain<ColorState>, IColorGrain
    {
        public async Task AddTranslation(ColorTranslation translation)
        {
            await ReadStateAsync();

            if (State.Value == null)
            {
                State = new ColorState();
            }

            State.Value.Names.Add(translation);
            await WriteStateAsync();
        }

        public async Task<Color> GetColor()
        {
            await ReadStateAsync();

            if (State.Value != null) return State.Value;

            State.Value = new Color
            {
                Id = this.GetPrimaryKeyString(),
                Names = new List<ColorTranslation>()
            };

            await WriteStateAsync();

            return State.Value;
        }

        public async Task<List<ColorTranslation>> GetTranslations()
        {
            await ReadStateAsync();
            return State.Value.Names;
        }
    }
}
