namespace Cesium.Ion
{
    public static class IonScopeRepresentation
    {
        public static string ToRest(this IonScope scope)
        {
            switch (scope)
            {
                case IonScope.LIST:
                    return "assets:list";
                case IonScope.READ:
                    return "assets:read";
                case IonScope.WRITE:
                    return "assets:write";
                case IonScope.GEOCODE:
                    return "assets:geocode";
                default:
                    return null;
            }
        }
    }

    public enum IonScope
    {
        LIST,
        READ,
        WRITE,
        GEOCODE
    }
}
