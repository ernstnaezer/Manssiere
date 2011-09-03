namespace Manssiere.Core.Helpers
{
    public static class MathHelper
    {
        /// <summary>
        /// Test if the double is NaN or reaches Infinity.
        /// </summary>
        /// <param name="number">The number to test.</param>
        /// <returns>True is the double is not a NaN and is not Infinit.</returns>
        public static bool IsValid(double number)
        {
            return !(double.IsNaN(number) || double.IsInfinity(number));
        }
    }
}