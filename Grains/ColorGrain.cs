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

        public async Task<Color> GetColor()
        {
            // Sembla que "await ReadStateAsync()" no és necessari perquè ho fa
            // en el mètode "OnActivateAsync" que es crida automàticament.

            if (State.Value != null) return State.Value;

            State.Value = new Color
            {
                Id = this.GetPrimaryKeyString(),
                Names = new List<ColorTranslation>()
            };

            await WriteStateAsync();

            return State.Value;
        }

        public async Task<bool> DeleteColor()
        {
            if (State.Value != null)
            {
                await ClearStateAsync();
                return true;
            }
            return false;
        }

        public async Task AddTranslation(ColorTranslation translation)
        {
            if (State.Value == null)
            {
                // Un color que no existeix, cal crear l'estat
                State = new ColorState
                {
                    Value = new Color
                    {
                        Id = this.GetPrimaryKeyString(),
                        Names = new List<ColorTranslation>()
                    }
                };
            }

            State.Value.Names.Add(translation);
            await WriteStateAsync();
        }

        public async Task<bool> ModifyTranslation(ColorTranslation translation)
        {
            var modified = false;

            if (State.Value == null)
            {
                return false;
            }

            foreach (ColorTranslation item in State.Value.Names)
            {
                if (item.Language == translation.Language)
                {
                    item.name = translation.name;
                    modified = true;
                }
            }

            await WriteStateAsync();

            return modified;
        }

        public async Task<bool> DeleteTranslation(string translation)
        {

            if (State.Value == null)
            {
                return false;
            }

            var numColors = State.Value.Names.Count;
            var result = State.Value.Names.Where(x => x.Language != translation).ToList();

            State.Value.Names = result;

            await WriteStateAsync();

            return State.Value.Names.Count != numColors;
        }
    }
}
