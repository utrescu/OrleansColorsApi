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
                Translations = new List<ColorTranslation>()
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
                        Translations = new List<ColorTranslation>()
                    }
                };
            }

            State.Value.Translations.Add(translation);
            await WriteStateAsync();
        }

        public async Task<bool> ModifyTranslation(ColorTranslation translation)
        {
            var modified = false;

            if (State.Value == null)
            {
                return false;
            }

            foreach (ColorTranslation item in State.Value.Translations)
            {
                if (item.Language == translation.Language)
                {
                    item.Translation = translation.Translation;
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

            var numColors = State.Value.Translations.Count;
            var result = State.Value.Translations.Where(x => x.Language != translation).ToList();

            State.Value.Translations = result;

            await WriteStateAsync();

            return State.Value.Translations.Count != numColors;
        }
    }
}
