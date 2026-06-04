using System.Collections.Frozen;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MyTelegram.Messenger.Services.Impl;

public class CountryHelper : ICountryHelper, ISingletonDependency
{
    private static readonly string CountriesText = """
                                                   [
                                                       {
                                                           "iso2": "AD",
                                                           "default_name": "Andorra",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "376",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AE",
                                                           "default_name": "United Arab Emirates",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "971",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AF",
                                                           "default_name": "Afghanistan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "93",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AG",
                                                           "default_name": "Antigua & Barbuda",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1268",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AI",
                                                           "default_name": "Anguilla",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1264",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AL",
                                                           "default_name": "Albania",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "355",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AM",
                                                           "default_name": "Armenia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "374",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AO",
                                                           "default_name": "Angola",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "244",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AR",
                                                           "default_name": "Argentina",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "54",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AS",
                                                           "default_name": "American Samoa",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1684",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AT",
                                                           "default_name": "Austria",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "43",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AU",
                                                           "default_name": "Australia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "61",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AW",
                                                           "default_name": "Aruba",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "297",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "AZ",
                                                           "default_name": "Azerbaijan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "994",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BA",
                                                           "default_name": "Bosnia & Herzegovina",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "387",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BB",
                                                           "default_name": "Barbados",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1246",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BD",
                                                           "default_name": "Bangladesh",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "880",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BE",
                                                           "default_name": "Belgium",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "32",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BF",
                                                           "default_name": "Burkina Faso",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "226",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BG",
                                                           "default_name": "Bulgaria",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "359",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BH",
                                                           "default_name": "Bahrain",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "973",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BI",
                                                           "default_name": "Burundi",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "257",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BJ",
                                                           "default_name": "Benin",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "229",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BM",
                                                           "default_name": "Bermuda",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1441",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BN",
                                                           "default_name": "Brunei Darussalam",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "673",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BO",
                                                           "default_name": "Bolivia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "591",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BQ",
                                                           "default_name": "Bonaire, Sint Eustatius & Saba",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "599",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BR",
                                                           "default_name": "Brazil",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "55",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BS",
                                                           "default_name": "Bahamas",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1242",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BT",
                                                           "default_name": "Bhutan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "975",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BW",
                                                           "default_name": "Botswana",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "267",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BY",
                                                           "default_name": "Belarus",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "375",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "BZ",
                                                           "default_name": "Belize",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "501",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CA",
                                                           "default_name": "Canada",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1",
                                                                   "prefixes": [
                                                                       "403",
                                                                       "587",
                                                                       "780",
                                                                       "825",
                                                                       "236",
                                                                       "250",
                                                                       "604",
                                                                       "672",
                                                                       "778",
                                                                       "204",
                                                                       "431",
                                                                       "506",
                                                                       "709",
                                                                       "902",
                                                                       "782",
                                                                       "226",
                                                                       "249",
                                                                       "289",
                                                                       "343",
                                                                       "365",
                                                                       "416",
                                                                       "437",
                                                                       "519",
                                                                       "548",
                                                                       "613",
                                                                       "647",
                                                                       "705",
                                                                       "807",
                                                                       "905",
                                                                       "418",
                                                                       "438",
                                                                       "450",
                                                                       "514",
                                                                       "579",
                                                                       "581",
                                                                       "819",
                                                                       "873",
                                                                       "306",
                                                                       "639",
                                                                       "867"
                                                                   ],
                                                                   "patterns": [
                                                                       "XXX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CD",
                                                           "default_name": "Congo (Dem. Rep.)",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "243",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CF",
                                                           "default_name": "Central African Rep.",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "236",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CG",
                                                           "default_name": "Congo (Rep.)",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "242",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CH",
                                                           "default_name": "Switzerland",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "41",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CI",
                                                           "default_name": "Cote d'Ivoire",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "225",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XX XX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CK",
                                                           "default_name": "Cook Islands",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "682",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CL",
                                                           "default_name": "Chile",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "56",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CM",
                                                           "default_name": "Cameroon",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "237",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CN",
                                                           "default_name": "China",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "86",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CO",
                                                           "default_name": "Colombia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "57",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CR",
                                                           "default_name": "Costa Rica",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "506",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CU",
                                                           "default_name": "Cuba",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "53",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CV",
                                                           "default_name": "Cape Verde",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "238",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CW",
                                                           "default_name": "Curacao",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "599",
                                                                   "prefixes": [
                                                                       "9"
                                                                   ],
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CY",
                                                           "default_name": "Cyprus",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "357",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "CZ",
                                                           "default_name": "Czech Republic",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "420",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "DE",
                                                           "default_name": "Germany",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "49",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "DJ",
                                                           "default_name": "Djibouti",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "253",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "DK",
                                                           "default_name": "Denmark",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "45",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "DM",
                                                           "default_name": "Dominica",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1767",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "DO",
                                                           "default_name": "Dominican Rep.",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1809",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               },
                                                               {
                                                                   "country_code": "1829",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               },
                                                               {
                                                                   "country_code": "1849",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "DZ",
                                                           "default_name": "Algeria",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "213",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "EC",
                                                           "default_name": "Ecuador",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "593",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "EE",
                                                           "default_name": "Estonia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "372",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "EG",
                                                           "default_name": "Egypt",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "20",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "ER",
                                                           "default_name": "Eritrea",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "291",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "ES",
                                                           "default_name": "Spain",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "34",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "ET",
                                                           "default_name": "Ethiopia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "251",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "FI",
                                                           "default_name": "Finland",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "358",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "FJ",
                                                           "default_name": "Fiji",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "679",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "FK",
                                                           "default_name": "Falkland Islands",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "500",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "FM",
                                                           "default_name": "Micronesia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "691",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "FO",
                                                           "default_name": "Faroe Islands",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "298",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "FR",
                                                           "default_name": "France",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "33",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XX XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "FT",
                                                           "default_name": "Anonymous Numbers",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "888",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "8 XXX",
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GA",
                                                           "default_name": "Gabon",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "241",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GB",
                                                           "default_name": "United Kingdom",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "44",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GD",
                                                           "default_name": "Grenada",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1473",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GE",
                                                           "default_name": "Georgia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "995",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GF",
                                                           "default_name": "French Guiana",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "594",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GH",
                                                           "default_name": "Ghana",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "233",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GI",
                                                           "default_name": "Gibraltar",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "350",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GL",
                                                           "default_name": "Greenland",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "299",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GM",
                                                           "default_name": "Gambia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "220",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GN",
                                                           "default_name": "Guinea",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "224",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GP",
                                                           "default_name": "Guadeloupe",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "590",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GQ",
                                                           "default_name": "Equatorial Guinea",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "240",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GR",
                                                           "default_name": "Greece",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "30",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GT",
                                                           "default_name": "Guatemala",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "502",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GU",
                                                           "default_name": "Guam",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1671",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GW",
                                                           "default_name": "Guinea-Bissau",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "245",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "GY",
                                                           "default_name": "Guyana",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "592",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "HK",
                                                           "default_name": "Hong Kong",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "852",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "HN",
                                                           "default_name": "Honduras",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "504",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "HR",
                                                           "default_name": "Croatia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "385",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "HT",
                                                           "default_name": "Haiti",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "509",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "HU",
                                                           "default_name": "Hungary",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "36",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "ID",
                                                           "default_name": "Indonesia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "62",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "IE",
                                                           "default_name": "Ireland",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "353",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "IL",
                                                           "default_name": "Israel",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "972",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "IN",
                                                           "default_name": "India",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "91",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXXX XXXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "IO",
                                                           "default_name": "Diego Garcia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "246",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "IQ",
                                                           "default_name": "Iraq",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "964",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "IR",
                                                           "default_name": "Iran",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "98",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "IS",
                                                           "default_name": "Iceland",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "354",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "IT",
                                                           "default_name": "Italy",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "39",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "JM",
                                                           "default_name": "Jamaica",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1876",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "JO",
                                                           "default_name": "Jordan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "962",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "JP",
                                                           "default_name": "Japan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "81",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "KE",
                                                           "default_name": "Kenya",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "254",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "KG",
                                                           "default_name": "Kyrgyzstan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "996",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "KH",
                                                           "default_name": "Cambodia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "855",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "KI",
                                                           "default_name": "Kiribati",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "686",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "KM",
                                                           "default_name": "Comoros",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "269",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "KN",
                                                           "default_name": "Saint Kitts & Nevis",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1869",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "KP",
                                                           "default_name": "North Korea",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "850",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "KR",
                                                           "default_name": "South Korea",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "82",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "KW",
                                                           "default_name": "Kuwait",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "965",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "KY",
                                                           "default_name": "Cayman Islands",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1345",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "KZ",
                                                           "default_name": "Kazakhstan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "7",
                                                                   "prefixes": [
                                                                       "6",
                                                                       "7"
                                                                   ],
                                                                   "patterns": [
                                                                       "XXX XXX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "LA",
                                                           "default_name": "Laos",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "856",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "LB",
                                                           "default_name": "Lebanon",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "961",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "LC",
                                                           "default_name": "Saint Lucia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1758",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "LI",
                                                           "default_name": "Liechtenstein",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "423",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "LK",
                                                           "default_name": "Sri Lanka",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "94",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "LR",
                                                           "default_name": "Liberia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "231",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "LS",
                                                           "default_name": "Lesotho",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "266",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "LT",
                                                           "default_name": "Lithuania",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "370",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "LU",
                                                           "default_name": "Luxembourg",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "352",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "LV",
                                                           "default_name": "Latvia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "371",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "LY",
                                                           "default_name": "Libya",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "218",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MA",
                                                           "default_name": "Morocco",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "212",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MC",
                                                           "default_name": "Monaco",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "377",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MD",
                                                           "default_name": "Moldova",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "373",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "ME",
                                                           "default_name": "Montenegro",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "382",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MG",
                                                           "default_name": "Madagascar",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "261",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XX XXX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MH",
                                                           "default_name": "Marshall Islands",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "692",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MK",
                                                           "default_name": "North Macedonia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "389",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXXXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "ML",
                                                           "default_name": "Mali",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "223",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MM",
                                                           "default_name": "Myanmar",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "95",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MN",
                                                           "default_name": "Mongolia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "976",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MO",
                                                           "default_name": "Macau",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "853",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MP",
                                                           "default_name": "Northern Mariana Islands",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1670",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MQ",
                                                           "default_name": "Martinique",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "596",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MR",
                                                           "default_name": "Mauritania",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "222",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MS",
                                                           "default_name": "Montserrat",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1664",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MT",
                                                           "default_name": "Malta",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "356",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MU",
                                                           "default_name": "Mauritius",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "230",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MV",
                                                           "default_name": "Maldives",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "960",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MW",
                                                           "default_name": "Malawi",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "265",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MX",
                                                           "default_name": "Mexico",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "52",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MY",
                                                           "default_name": "Malaysia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "60",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "MZ",
                                                           "default_name": "Mozambique",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "258",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "NA",
                                                           "default_name": "Namibia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "264",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "NC",
                                                           "default_name": "New Caledonia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "687",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "NE",
                                                           "default_name": "Niger",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "227",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "NF",
                                                           "default_name": "Norfolk Island",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "672",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "NG",
                                                           "default_name": "Nigeria",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "234",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "NI",
                                                           "default_name": "Nicaragua",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "505",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "NL",
                                                           "default_name": "Netherlands",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "31",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XX XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "NO",
                                                           "default_name": "Norway",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "47",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "NP",
                                                           "default_name": "Nepal",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "977",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "NR",
                                                           "default_name": "Nauru",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "674",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "NU",
                                                           "default_name": "Niue",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "683",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "NZ",
                                                           "default_name": "New Zealand",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "64",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "OM",
                                                           "default_name": "Oman",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "968",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PA",
                                                           "default_name": "Panama",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "507",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PE",
                                                           "default_name": "Peru",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "51",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PF",
                                                           "default_name": "French Polynesia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "689",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PG",
                                                           "default_name": "Papua New Guinea",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "675",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PH",
                                                           "default_name": "Philippines",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "63",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PK",
                                                           "default_name": "Pakistan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "92",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PL",
                                                           "default_name": "Poland",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "48",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PM",
                                                           "default_name": "Saint Pierre & Miquelon",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "508",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PR",
                                                           "default_name": "Puerto Rico",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1787",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               },
                                                               {
                                                                   "country_code": "1939",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PS",
                                                           "default_name": "Palestine",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "970",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PT",
                                                           "default_name": "Portugal",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "351",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PW",
                                                           "default_name": "Palau",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "680",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "PY",
                                                           "default_name": "Paraguay",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "595",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "QA",
                                                           "default_name": "Qatar",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "974",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "RE",
                                                           "default_name": "Reunion",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "262",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "RO",
                                                           "default_name": "Romania",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "40",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "RS",
                                                           "default_name": "Serbia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "381",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "RU",
                                                           "default_name": "Russian Federation",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "7",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "RW",
                                                           "default_name": "Rwanda",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "250",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SA",
                                                           "default_name": "Saudi Arabia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "966",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SB",
                                                           "default_name": "Solomon Islands",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "677",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SC",
                                                           "default_name": "Seychelles",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "248",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SD",
                                                           "default_name": "Sudan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "249",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SE",
                                                           "default_name": "Sweden",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "46",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SG",
                                                           "default_name": "Singapore",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "65",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SH",
                                                           "default_name": "Saint Helena",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "247",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               },
                                                               {
                                                                   "country_code": "290",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SI",
                                                           "default_name": "Slovenia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "386",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SK",
                                                           "default_name": "Slovakia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "421",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SL",
                                                           "default_name": "Sierra Leone",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "232",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SM",
                                                           "default_name": "San Marino",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "378",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SN",
                                                           "default_name": "Senegal",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "221",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SO",
                                                           "default_name": "Somalia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "252",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SR",
                                                           "default_name": "Suriname",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "597",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SS",
                                                           "default_name": "South Sudan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "211",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "ST",
                                                           "default_name": "Sao Tome & Principe",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "239",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SV",
                                                           "default_name": "El Salvador",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "503",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SX",
                                                           "default_name": "Sint Maarten",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1721",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SY",
                                                           "default_name": "Syria",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "963",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "SZ",
                                                           "default_name": "Eswatini",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "268",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TC",
                                                           "default_name": "Turks & Caicos Islands",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1649",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TD",
                                                           "default_name": "Chad",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "235",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TG",
                                                           "default_name": "Togo",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "228",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TH",
                                                           "default_name": "Thailand",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "66",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TJ",
                                                           "default_name": "Tajikistan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "992",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TK",
                                                           "default_name": "Tokelau",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "690",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TL",
                                                           "default_name": "Timor-Leste",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "670",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TM",
                                                           "default_name": "Turkmenistan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "993",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXXXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TN",
                                                           "default_name": "Tunisia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "216",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TO",
                                                           "default_name": "Tonga",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "676",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TR",
                                                           "default_name": "Turkey",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "90",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TT",
                                                           "default_name": "Trinidad & Tobago",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1868",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TV",
                                                           "default_name": "Tuvalu",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "688",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TW",
                                                           "default_name": "Taiwan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "886",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "TZ",
                                                           "default_name": "Tanzania",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "255",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "UA",
                                                           "default_name": "Ukraine",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "380",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "UG",
                                                           "default_name": "Uganda",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "256",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "US",
                                                           "default_name": "USA",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "UY",
                                                           "default_name": "Uruguay",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "598",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "X XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "UZ",
                                                           "default_name": "Uzbekistan",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "998",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XX XX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "VC",
                                                           "default_name": "Saint Vincent & the Grenadines",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1784",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "VE",
                                                           "default_name": "Venezuela",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "58",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "VG",
                                                           "default_name": "British Virgin Islands",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1284",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "VI",
                                                           "default_name": "US Virgin Islands",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "1340",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "VN",
                                                           "default_name": "Vietnam",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "84",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "VU",
                                                           "default_name": "Vanuatu",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "678",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "WF",
                                                           "default_name": "Wallis & Futuna",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "681",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "WS",
                                                           "default_name": "Samoa",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "685",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "XG",
                                                           "default_name": "Global Mobile Satellite System",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "881",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "XK",
                                                           "default_name": "Kosovo",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "383",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "XV",
                                                           "default_name": "International Networks",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "882",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               },
                                                               {
                                                                   "country_code": "883",
                                                                   "prefixes": null,
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "YE",
                                                           "default_name": "Yemen",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "967",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XXX XXX XXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "YL",
                                                           "default_name": "Y-land",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "42",
                                                                   "prefixes": [
                                                                       "4",
                                                                       "7"
                                                                   ],
                                                                   "patterns": null
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "ZA",
                                                           "default_name": "South Africa",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "27",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "ZM",
                                                           "default_name": "Zambia",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "260",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       },
                                                       {
                                                           "iso2": "ZW",
                                                           "default_name": "Zimbabwe",
                                                           "name": null,
                                                           "country_codes": [
                                                               {
                                                                   "country_code": "263",
                                                                   "prefixes": null,
                                                                   "patterns": [
                                                                       "XX XXX XXXX"
                                                                   ]
                                                               }
                                                           ]
                                                       }
                                                   ]
                                                   """;

    private static List<CountryItem> _allCountries = [];
    private static FrozenDictionary<string, CountryCodeItem>? _countryCodeToPatterns;
    private static FrozenDictionary<string, string>? _countryCodeToCountryIso2;
    public bool TryGetCountryCodeItem(string countryCode, [NotNullWhen(true)] out CountryCodeItem? countryCodeItem)
    {
        InitAllCountries();

        return _countryCodeToPatterns!.TryGetValue(countryCode, out countryCodeItem);
    }

    public string GetCountryCodeByPhoneNumber(string phoneNumber)
    {
        InitAllCountries();

        if (string.IsNullOrEmpty(phoneNumber))
        {
            return string.Empty;
        }
        if (phoneNumber.StartsWith("+"))
        {
            phoneNumber = phoneNumber[1..];
            var items = phoneNumber.Split(" ");
            if (_countryCodeToPatterns!.TryGetValue(items[0], out var countryCodeItem))
            {
                return countryCodeItem.CountryCode;
            }
        }

        if (phoneNumber.Length < 4)
        {
            return string.Empty;
        }

        var code4 = phoneNumber[..4];
        var code3 = phoneNumber[..3];
        var code2 = phoneNumber[..2];
        var code1 = phoneNumber[..1];

        if (_countryCodeToPatterns!.TryGetValue(code4, out var item4))
        {
            return item4.CountryCode;
        }

        if (_countryCodeToPatterns!.TryGetValue(code3, out var item3))
        {
            return item3.CountryCode;
        }

        if (_countryCodeToPatterns!.TryGetValue(code2, out var item2))
        {
            return item2.CountryCode;
        }

        if (_countryCodeToPatterns!.TryGetValue(code1, out var item1))
        {
            return item1.CountryCode;
        }

        return string.Empty;
    }

    public string? GetCountryIso2ByPhoneNumber(string phoneNumber)
    {
        var countryCode = GetCountryCodeByPhoneNumber(phoneNumber);
        if (_countryCodeToCountryIso2?.TryGetValue(countryCode, out var iso2) ?? false)
        {
            return iso2;
        }

        return null;
        //return "Unknown";
    }

    public IReadOnlyCollection<CountryItem> GetAllCountryList()
    {
        if (_allCountries.Count > 0)
        {
            return _allCountries;
        }

        InitAllCountries();

        return _allCountries;
    }

    public void InitAllCountries()
    {
        if (_countryCodeToPatterns == null || _allCountries.Count == 0)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            var countriesList =
                JsonSerializer.Deserialize<List<CountryItem>>(CountriesText, jsonOptions);

            _allCountries = countriesList ?? [];

            _countryCodeToPatterns = _allCountries.SelectMany(p => p.CountryCodes)
                    .GroupBy(p => p.CountryCode)
                    .ToDictionary(k => k.Key,
                        v => new CountryCodeItem(v.Key, v.SelectMany(x => x.Prefixes ?? []).ToList(),
                            v.SelectMany(x => x.Patterns ?? []).ToList(),
                            v.SelectMany(x => x.Patterns ?? []).Select(p => p.Replace(" ", string.Empty).Length)
                                .ToList()))
                    .ToFrozenDictionary()
                ;

            //_countryCodeToCountryIso2 = _allCountries
            //    .SelectMany(p => p.CountryCodes.Select(x => new { x.CountryCode, p.Iso2 }))
            //    .DistinctBy(p=>p.CountryCode)
            //    .ToFrozenDictionary(k => k.CountryCode, v => v.Iso2);

            var countryCodeToIso2 = new Dictionary<string, string>();
            //var countryCodeToPatterns = new Dictionary<string, CountryCodeItem>();
            foreach (var countryItem in _allCountries)
            {
                foreach (var countryCodeItem in countryItem.CountryCodes)
                {
                    if (countryCodeItem.Prefixes?.Count > 0)
                    {
                        foreach (var prefix in countryCodeItem.Prefixes)
                        {
                            countryCodeToIso2.TryAdd($"{countryCodeItem.CountryCode}{prefix}", countryItem.Iso2);
                        }
                    }
                    else
                    {
                        countryCodeToIso2.TryAdd(countryCodeItem.CountryCode, countryItem.Iso2);
                    }
                }
            }

            _countryCodeToCountryIso2 = countryCodeToIso2.ToFrozenDictionary();

        }
    }
}