using System;
using System.Collections.Generic;

namespace GrainInterfaces.States
{
    [Serializable]
    public class ColorTranslation
    {

        public string Language { get; set; }

        public string Translation { get; set; }

    }

    [Serializable]
    public class Color
    {
        public string Id { get; set; }

        public List<ColorTranslation> Translations { get; set; }

    }

    [Serializable]
    public class ColorState
    {
        public Color Value { get; set; }
    }
}