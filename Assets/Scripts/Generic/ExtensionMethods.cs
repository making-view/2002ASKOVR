public static class ExtensionMethods
{
    public static float Map(this float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (value - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;

        
    }

}