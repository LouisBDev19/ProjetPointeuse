using APIPointeuse.Models;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.WebPages;

namespace ApplicationWebPointeuse.Common
{
    public class Methods
    {
        public string ConvertDateTimeLetters(DateTime date)
        {
            return date.ToString("dddd dd MMMM yyyy", new CultureInfo("fr-FR")) ?? "-";
        }

        public DateTime ConvertDateTimeLettersToDateTime(string date)
        {
            return DateTime.ParseExact(date, "dddd dd MMMM yyyy", new CultureInfo("fr-FR"));
        }

        public List<Dictionary<string, string>> datesPeriods(List<Dictionary<string, DateTime?>> uniqueDates, ICollection<Periods>? datesPeriod = null, PdfPTable? table = null)
        {
            List<DateTime> daysPeriodsList = new List<DateTime>();
            List<Dictionary<string, string>> arrivalDays = new List<Dictionary<string, string>>();
            if (datesPeriod?.Count > 0)
            {
                List<DayOfWeek> excludedDays = new List<DayOfWeek>();

                DateTime startDate = default(DateTime);
                DateTime endDate = default(DateTime);

                List<DateTime> dateRange = new List<DateTime>();
                foreach (var dates in datesPeriod)
                {
                    startDate = dates.BeginningPeriod;
                    endDate = dates.EndingPeriod < DateTime.Today ? dates.EndingPeriod : DateTime.Today;

                    while (startDate <= endDate)
                    {
                        if (!excludedDays.Contains(startDate.DayOfWeek))
                        {
                            daysPeriodsList.Add(startDate.Date);
                        }
                        dateRange.Add(startDate);
                        startDate = startDate.AddDays(1);
                    }
                }
                foreach (var dayPeriods in daysPeriodsList)
                {
                    bool hasArrival = false;
                    foreach(var signingDate in uniqueDates)
                    {
                        DateTime? arrivalMorning = signingDate["morning"];
                        DateTime? arrivalAfternoon = signingDate["afternoon"];
                        if (dayPeriods == arrivalMorning?.Date || dayPeriods == arrivalAfternoon?.Date)
                        {
                            arrivalDays.Add(new Dictionary<string, string>
                            {
                                { "day" , ConvertDateTimeLetters(dayPeriods) },
                                { "morning", arrivalMorning != null ? arrivalMorning?.Hour.ToString("00") + ':' + arrivalMorning?.Minute.ToString("00") + ':' + arrivalMorning?.Second.ToString("00") : "" },
                                { "afternoon", arrivalAfternoon != null ? arrivalAfternoon?.Hour.ToString("00") + ':' + arrivalAfternoon?.Minute.ToString("00") + ':' + arrivalAfternoon?.Second.ToString("00") : "" },
                            });
                            hasArrival = true;
                        }
                    }
                    if (!hasArrival)
                    {
                        arrivalDays.Add(new Dictionary<string, string>
                            {
                                { "day" , ConvertDateTimeLetters(dayPeriods) },
                                { "morning", "" },
                                { "afternoon", "" },
                            });
                    }
                }
            }
            else
            {
                foreach (var signingDate in uniqueDates)
                {
                    DateTime? arrivalMorning = signingDate["morning"];
                    DateTime? arrivalAfternoon = signingDate["afternoon"];
                    DateTime dateToConvert = (DateTime)(arrivalMorning ?? arrivalAfternoon);

                    arrivalDays.Add(new Dictionary<string, string>
                    {
                        { "day" , ConvertDateTimeLetters(dateToConvert) },
                        { "morning", arrivalMorning != null ? arrivalMorning?.Hour.ToString("00") + ':' + arrivalMorning?.Minute.ToString("00") + ':' + arrivalMorning?.Second.ToString("00") : "" },
                        { "afternoon", arrivalAfternoon != null ? arrivalAfternoon?.Hour.ToString("00") + ':' + arrivalAfternoon?.Minute.ToString("00") + ':' + arrivalAfternoon?.Second.ToString("00") : "" },
                    });
                }
            }
            if(table != null)
            {
                fillUpPDF(table, arrivalDays);
            }
            else
            {
                return arrivalDays;
            }
            return arrivalDays;
        }

        public void fillUpPDF(PdfPTable table, List<Dictionary<string, string>> arrivalDays)
        {
            foreach(var arrivalDay in arrivalDays)
            {
                foreach(var day  in arrivalDay)
                {
                    table.AddCell(day.Value);
                }
            }
        }

        public string getFullPeriodDuration(ICollection<Periods>? periods)
        {
            string fullPeriodDuration = "(pas de période définie) <span title='Seuls les émargements sont renseignés'><i class='fa-solid fa-circle-info' style='color:#0080ff'></i></span>";
            Dictionary<string, string> periodDuration = new Dictionary<string, string>();
            List<Periods>? periodsList = periods?.ToList();
            if (periodsList?.Count > 0)
            {
                periodDuration.Add("beginning", periodsList[0].BeginningPeriod.ToShortDateString());
                periodDuration.Add("ending", periodsList[periodsList.Count - 1].EndingPeriod.ToShortDateString());

                if(periodDuration.Count == 2)
                {
                    fullPeriodDuration = " du " + periodDuration["beginning"] + " au " + periodDuration["ending"];
                }
            }
            return fullPeriodDuration;
        }

        public string FormatPhoneNumber(string phoneNumber)
        {
            string formattedPhoneNumber = string.Empty;

            for (int i = 0; i < phoneNumber.Length; i++)
            {
                if (i > 0 && i % 2 == 0)
                {
                    formattedPhoneNumber += " ";
                }

                formattedPhoneNumber += phoneNumber[i];
            }

            return formattedPhoneNumber;
        }
    }
}
