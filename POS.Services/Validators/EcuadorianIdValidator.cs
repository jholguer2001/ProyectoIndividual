namespace POS.Services.Validators
{
    public static class EcuadorianIdValidator
    {
        /// <summary>
        /// Validates Ecuadorian identification number (cédula)
        /// </summary>
        public static bool IsValidCedula(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula) || cedula.Length != 10)
                return false;

            if (!cedula.All(char.IsDigit))
                return false;

            // Province code must be between 01 and 24
            int provinceCode = int.Parse(cedula.Substring(0, 2));
            if (provinceCode < 1 || provinceCode > 24)
                return false;

            // Third digit must be less than 6 for natural persons
            int thirdDigit = int.Parse(cedula.Substring(2, 1));
            if (thirdDigit >= 6)
                return false;

            // Validate check digit using algorithm
            int[] coefficients = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                int digit = int.Parse(cedula.Substring(i, 1));
                int product = digit * coefficients[i];

                if (product >= 10)
                    product -= 9;

                sum += product;
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            int lastDigit = int.Parse(cedula.Substring(9, 1));

            return checkDigit == lastDigit;
        }
    }
}
