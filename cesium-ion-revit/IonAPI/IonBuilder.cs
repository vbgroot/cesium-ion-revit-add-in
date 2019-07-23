namespace Cesium.Ion.Revit
{
    public static class IonBuilder
    {
        public static IonCreateAPI Ion(this string TargetModel, string Token)
        {
            return new IonCreateAPI(TargetModel, Token);
        }
    }
}
