using System;

namespace Leaderboard
{
    [Serializable]
    public class GeoIpData
    {
        public string geoplugin_request;

        public string geoplugin_status;

        public string geoplugin_credit;

        public string geoplugin_city;

        public string geoplugin_region;

        public string geoplugin_areaCode;

        public string geoplugin_dmaCode;

        public string geoplugin_countryCode = "ND";

        public string geoplugin_countryName = "No disp";

        public string geoplugin_continentCode = "ND";

        public string geoplugin_latitude;

        public string geoplugin_longitude;

        public string geoplugin_regionCode;

        public string geoplugin_regionName;

        public string geoplugin_currencyCode;

        public string geoplugin_currencySymbol;

        public string geoplugin_currencySymbol_UTF8;

        public string geoplugin_currencyConverter;
    }
}
