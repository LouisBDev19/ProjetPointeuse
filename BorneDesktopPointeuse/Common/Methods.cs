using APIPointeuse.Models;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorneDesktopPointeuse.Common
{
    public class Methods
    {
        public static ArrivalDateTime ExplodeStringToArrivalDateTime(string input, char delimiter)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            string[] parts = input.Split(delimiter);

            if (parts.Length != 2)
            {
                return null;
            }

            if (!int.TryParse(parts[0], out int id))
            {
                return null;
            }

            if (!DateTime.TryParseExact(parts[1], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime arrivalDate))
            {
                return null;
            }

            ArrivalDateTime arrivalDateTime = new ArrivalDateTime
            {
                IdStudent = id,
                ArrivalSavedDate = arrivalDate,
            };

            return arrivalDateTime;
        }
    }
}
