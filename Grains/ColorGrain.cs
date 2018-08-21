using System.Collections.Generic;
using GrainInterfaces;
using Orleans;
using System.Threading.Tasks;
using GrainInterfaces.States;
using Orleans.Providers;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace Grains
{
    [StorageProvider(ProviderName = "ColorsStorage")]
    public class ColorGrain : Grain<ColorState>, IColorGrain
    {
        public override Task OnActivateAsync()
        {
            string id = this.GetPrimaryKeyString();
            if (!IsRGBCorrect(id))
            {
                throw new ColorsException("RGB code incorrect");
            }
            return base.OnActivateAsync();
        }

        private bool IsRGBCorrect(string value)
        {
            Match result = Regex.Match(value, @"^[0-9A-F]{6}$");
            return result.Success;
        }

        private bool IsValidTranslation(ColorTranslation c) => c.Language != null && c.Translation != null;

        private void Validate(ColorTranslation translation)
        {
            if (!IsValidTranslation(translation))
            {
                throw new ColorsException("Invallid values");
            }
        }

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

        public async Task DeleteColor()
        {
            if (State.Value == null)
            {
                throw new ColorsException("RGB value does not exist");
            }
            await ClearStateAsync();
        }

        public async Task AddTranslation(ColorTranslation translation)
        {
            Validate(translation);

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

            if (State.Value.Translations.Count(it => it.Language == translation.Language) != 0)
            {
                throw new ColorsException("Translation already exists");
            }
            State.Value.Translations.Add(translation);
            await WriteStateAsync();
        }



        public async Task ModifyTranslation(ColorTranslation translation)
        {
            Validate(translation);

            if (State.Value == null)
            {
                throw new ColorsException("Translation does not exists");
            }

            var modified = false;

            foreach (ColorTranslation item in State.Value.Translations)
            {
                if (item.Language == translation.Language)
                {
                    item.Translation = translation.Translation;
                    modified = true;
                }
            }
            if (modified == false)
            {
                throw new ColorsException("Translation does not exists");
            }

            await WriteStateAsync();
        }

        public async Task DeleteTranslation(string translation)
        {

            if (State.Value == null)
            {
                throw new ColorsException("Translation does not exists");
            }

            var numColors = State.Value.Translations.Count;
            var result = State.Value.Translations.Where(x => x.Language != translation).ToList();

            if (State.Value.Translations.Count != numColors)
            {
                throw new ColorsException("Translation does not exists");
            }

            State.Value.Translations = result;
            await WriteStateAsync();
        }
    }
}
