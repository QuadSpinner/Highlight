using System;

namespace Lighthouse.Utilities
{
    [Serializable]
    public enum CornerStyle
    {
        Square = 0,
        RoundedCorner = 2,
        Soft = 6
    }

    [Serializable]
    public enum BlurType
    {
        NoBlur = 0,
        BlurAll = 1,
        Selective = 2
    }

    [Serializable]
    public enum BlurIntensity
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Ultra = 4
    }
}