using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBetfairAPI
{
    public static class BetfairHelper
    {
        public static string GetCountryName(string countryCode)
        {
            string Country = "";
            switch (countryCode)
            {
                case "GB":
                    Country = "Storbritannien";
                    break;
                case "IE":
                    Country = "Irland";
                    break;
                case "FR":
                    Country = "Frankrike";
                    break;
                case "ES":
                    Country = "Spanien";
                    break;
                case "DE":
                    Country = "Tyskland";
                    break;
                case "IT":
                    Country = "Italien";
                    break;
                case "PT":
                    Country = "Portugal";
                    break;
                case "NL":
                    Country = "Nederländerna";
                    break;
                case "BE":
                    Country = "Belgien";
                    break;
                case "CH":
                    Country = "Schweiz";
                    break;
                case "AT":
                    Country = "Österrike";
                    break;
                case "SE":
                    Country = "Sverige";
                    break;
                case "NO":
                    Country = "Norge";
                    break;
                case "FI":
                    Country = "Finland";
                    break;
                case "DK":
                    Country = "Danmark";
                    break;
                case "IS":
                    Country = "Island";
                    break;
                default:
                    Country = "Okänt";
                    break;
            }
            return Country;
        }



    }
}
