using System;
using System.Collections.Generic;
using GrainInterfaces;
using Orleans;
using System.Threading.Tasks;
using GrainInterfaces.States;
using Orleans.Providers;
using System.Linq;

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

        public async Task ModifyTranslation(ColorTranslation translation)
        {
            await ReadStateAsync();

            if (State.Value == null)
            {
                State = new ColorState();
            }

            foreach (ColorTranslation item in State.Value.Names)
            {
                if (item.Language == translation.Language)
                {
                    item.name = translation.name;
                }
            }

            await WriteStateAsync();
        }

        public async Task DeleteTranslation(ColorTranslation translation)
        {
            await ReadStateAsync();

            if (State.Value == null)
            {
                State = new ColorState();
            }

            var result = State.Value.Names.Where(x => x.Language != translation.Language).ToList();

            State.Value.Names = result;

            await WriteStateAsync();
        }
    }
}
