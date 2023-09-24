namespace ApplicationMobilePointeuse.Common
{
    public class Methods
    {
        private const double EarthRadiusKm = 6371.0; // Rayon de la Terre en kilomètres

        public static bool IsWithinRadius(double currentLat, double currentLon, double targetLat, double targetLon, double radiusMeters)
        {
            double dLat = ToRadians(targetLat - currentLat);
            double dLon = ToRadians(targetLon - currentLon);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(currentLat)) * Math.Cos(ToRadians(targetLat)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = EarthRadiusKm * c * 1000; // Convertir en mètres

            return distance <= radiusMeters;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
