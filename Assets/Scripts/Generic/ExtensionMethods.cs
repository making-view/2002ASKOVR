public static class ExtensionMethods
{
    public static float Map(this float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (((value - oldMin) * newMax - newMin) / oldMax - oldMin) + newMin;
    }

}