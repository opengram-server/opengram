using System.Collections.Frozen;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MyTelegram.Messenger.Services.Impl;

public class TimezoneHelper : ITimezoneHelper, ISingletonDependency
{
    private static readonly string AllTimezonesJsonText = @"[
        { 
          ""id"": ""Africa/Abidjan"" ,
          ""name"": ""Africa/Abidjan"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Accra"" ,
          ""name"": ""Africa/Accra"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Addis_Ababa"" ,
          ""name"": ""Africa/Addis Ababa"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Africa/Algiers"" ,
          ""name"": ""Africa/Algiers"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/Asmara"" ,
          ""name"": ""Africa/Asmara"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Africa/Bamako"" ,
          ""name"": ""Africa/Bamako"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Bangui"" ,
          ""name"": ""Africa/Bangui"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/Banjul"" ,
          ""name"": ""Africa/Banjul"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Bissau"" ,
          ""name"": ""Africa/Bissau"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Blantyre"" ,
          ""name"": ""Africa/Blantyre"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Brazzaville"" ,
          ""name"": ""Africa/Brazzaville"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/Bujumbura"" ,
          ""name"": ""Africa/Bujumbura"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Cairo"" ,
          ""name"": ""Africa/Cairo"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Casablanca"" ,
          ""name"": ""Africa/Casablanca"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Ceuta"" ,
          ""name"": ""Africa/Ceuta"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Conakry"" ,
          ""name"": ""Africa/Conakry"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Dakar"" ,
          ""name"": ""Africa/Dakar"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Dar_es_Salaam"" ,
          ""name"": ""Africa/Dar es Salaam"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Africa/Djibouti"" ,
          ""name"": ""Africa/Djibouti"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Africa/Douala"" ,
          ""name"": ""Africa/Douala"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/El_Aaiun"" ,
          ""name"": ""Africa/El Aaiun"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Freetown"" ,
          ""name"": ""Africa/Freetown"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Gaborone"" ,
          ""name"": ""Africa/Gaborone"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Harare"" ,
          ""name"": ""Africa/Harare"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Johannesburg"" ,
          ""name"": ""Africa/Johannesburg"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Juba"" ,
          ""name"": ""Africa/Juba"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Kampala"" ,
          ""name"": ""Africa/Kampala"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Africa/Khartoum"" ,
          ""name"": ""Africa/Khartoum"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Kigali"" ,
          ""name"": ""Africa/Kigali"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Kinshasa"" ,
          ""name"": ""Africa/Kinshasa"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/Lagos"" ,
          ""name"": ""Africa/Lagos"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/Libreville"" ,
          ""name"": ""Africa/Libreville"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/Lome"" ,
          ""name"": ""Africa/Lome"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Luanda"" ,
          ""name"": ""Africa/Luanda"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/Lubumbashi"" ,
          ""name"": ""Africa/Lubumbashi"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Lusaka"" ,
          ""name"": ""Africa/Lusaka"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Malabo"" ,
          ""name"": ""Africa/Malabo"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/Maputo"" ,
          ""name"": ""Africa/Maputo"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Maseru"" ,
          ""name"": ""Africa/Maseru"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Mbabane"" ,
          ""name"": ""Africa/Mbabane"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Mogadishu"" ,
          ""name"": ""Africa/Mogadishu"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Africa/Monrovia"" ,
          ""name"": ""Africa/Monrovia"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Nairobi"" ,
          ""name"": ""Africa/Nairobi"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Africa/Ndjamena"" ,
          ""name"": ""Africa/Ndjamena"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/Niamey"" ,
          ""name"": ""Africa/Niamey"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/Nouakchott"" ,
          ""name"": ""Africa/Nouakchott"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Ouagadougou"" ,
          ""name"": ""Africa/Ouagadougou"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Porto-Novo"" ,
          ""name"": ""Africa/Porto-Novo"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/Sao_Tome"" ,
          ""name"": ""Africa/Sao Tome"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Africa/Tripoli"" ,
          ""name"": ""Africa/Tripoli"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Africa/Tunis"" ,
          ""name"": ""Africa/Tunis"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Africa/Windhoek"" ,
          ""name"": ""Africa/Windhoek"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""America/Adak"" ,
          ""name"": ""America/Adak"" ,
          ""utc_offset"": -32400 ,
        },
        { 
          ""id"": ""America/Anchorage"" ,
          ""name"": ""America/Anchorage"" ,
          ""utc_offset"": -28800 ,
        },
        { 
          ""id"": ""America/Anguilla"" ,
          ""name"": ""America/Anguilla"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Antigua"" ,
          ""name"": ""America/Antigua"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Araguaina"" ,
          ""name"": ""America/Araguaina"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Argentina/Buenos_Aires"" ,
          ""name"": ""America/Argentina/Buenos Aires"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Argentina/Catamarca"" ,
          ""name"": ""America/Argentina/Catamarca"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Argentina/Cordoba"" ,
          ""name"": ""America/Argentina/Cordoba"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Argentina/Jujuy"" ,
          ""name"": ""America/Argentina/Jujuy"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Argentina/La_Rioja"" ,
          ""name"": ""America/Argentina/La Rioja"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Argentina/Mendoza"" ,
          ""name"": ""America/Argentina/Mendoza"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Argentina/Rio_Gallegos"" ,
          ""name"": ""America/Argentina/Rio Gallegos"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Argentina/Salta"" ,
          ""name"": ""America/Argentina/Salta"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Argentina/San_Juan"" ,
          ""name"": ""America/Argentina/San Juan"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Argentina/San_Luis"" ,
          ""name"": ""America/Argentina/San Luis"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Argentina/Tucuman"" ,
          ""name"": ""America/Argentina/Tucuman"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Argentina/Ushuaia"" ,
          ""name"": ""America/Argentina/Ushuaia"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Aruba"" ,
          ""name"": ""America/Aruba"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Asuncion"" ,
          ""name"": ""America/Asuncion"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Atikokan"" ,
          ""name"": ""America/Atikokan"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Bahia"" ,
          ""name"": ""America/Bahia"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Bahia_Banderas"" ,
          ""name"": ""America/Bahia Banderas"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Barbados"" ,
          ""name"": ""America/Barbados"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Belem"" ,
          ""name"": ""America/Belem"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Belize"" ,
          ""name"": ""America/Belize"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Blanc-Sablon"" ,
          ""name"": ""America/Blanc-Sablon"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Boa_Vista"" ,
          ""name"": ""America/Boa Vista"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Bogota"" ,
          ""name"": ""America/Bogota"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Boise"" ,
          ""name"": ""America/Boise"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Cambridge_Bay"" ,
          ""name"": ""America/Cambridge Bay"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Campo_Grande"" ,
          ""name"": ""America/Campo Grande"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Cancun"" ,
          ""name"": ""America/Cancun"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Caracas"" ,
          ""name"": ""America/Caracas"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Cayenne"" ,
          ""name"": ""America/Cayenne"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Cayman"" ,
          ""name"": ""America/Cayman"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Chicago"" ,
          ""name"": ""America/Chicago"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Chihuahua"" ,
          ""name"": ""America/Chihuahua"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Ciudad_Juarez"" ,
          ""name"": ""America/Ciudad Juarez"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Costa_Rica"" ,
          ""name"": ""America/Costa Rica"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Creston"" ,
          ""name"": ""America/Creston"" ,
          ""utc_offset"": -25200 ,
        },
        { 
          ""id"": ""America/Cuiaba"" ,
          ""name"": ""America/Cuiaba"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Curacao"" ,
          ""name"": ""America/Curacao"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Danmarkshavn"" ,
          ""name"": ""America/Danmarkshavn"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""America/Dawson"" ,
          ""name"": ""America/Dawson"" ,
          ""utc_offset"": -25200 ,
        },
        { 
          ""id"": ""America/Dawson_Creek"" ,
          ""name"": ""America/Dawson Creek"" ,
          ""utc_offset"": -25200 ,
        },
        { 
          ""id"": ""America/Denver"" ,
          ""name"": ""America/Denver"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Detroit"" ,
          ""name"": ""America/Detroit"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Dominica"" ,
          ""name"": ""America/Dominica"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Edmonton"" ,
          ""name"": ""America/Edmonton"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Eirunepe"" ,
          ""name"": ""America/Eirunepe"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/El_Salvador"" ,
          ""name"": ""America/El Salvador"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Fort_Nelson"" ,
          ""name"": ""America/Fort Nelson"" ,
          ""utc_offset"": -25200 ,
        },
        { 
          ""id"": ""America/Fortaleza"" ,
          ""name"": ""America/Fortaleza"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Glace_Bay"" ,
          ""name"": ""America/Glace Bay"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Goose_Bay"" ,
          ""name"": ""America/Goose Bay"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Grand_Turk"" ,
          ""name"": ""America/Grand Turk"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Grenada"" ,
          ""name"": ""America/Grenada"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Guadeloupe"" ,
          ""name"": ""America/Guadeloupe"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Guatemala"" ,
          ""name"": ""America/Guatemala"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Guayaquil"" ,
          ""name"": ""America/Guayaquil"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Guyana"" ,
          ""name"": ""America/Guyana"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Halifax"" ,
          ""name"": ""America/Halifax"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Havana"" ,
          ""name"": ""America/Havana"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Hermosillo"" ,
          ""name"": ""America/Hermosillo"" ,
          ""utc_offset"": -25200 ,
        },
        { 
          ""id"": ""America/Indiana/Indianapolis"" ,
          ""name"": ""America/Indiana/Indianapolis"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Indiana/Knox"" ,
          ""name"": ""America/Indiana/Knox"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Indiana/Marengo"" ,
          ""name"": ""America/Indiana/Marengo"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Indiana/Petersburg"" ,
          ""name"": ""America/Indiana/Petersburg"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Indiana/Tell_City"" ,
          ""name"": ""America/Indiana/Tell City"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Indiana/Vevay"" ,
          ""name"": ""America/Indiana/Vevay"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Indiana/Vincennes"" ,
          ""name"": ""America/Indiana/Vincennes"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Indiana/Winamac"" ,
          ""name"": ""America/Indiana/Winamac"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Inuvik"" ,
          ""name"": ""America/Inuvik"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Iqaluit"" ,
          ""name"": ""America/Iqaluit"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Jamaica"" ,
          ""name"": ""America/Jamaica"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Juneau"" ,
          ""name"": ""America/Juneau"" ,
          ""utc_offset"": -28800 ,
        },
        { 
          ""id"": ""America/Kentucky/Louisville"" ,
          ""name"": ""America/Kentucky/Louisville"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Kentucky/Monticello"" ,
          ""name"": ""America/Kentucky/Monticello"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Kralendijk"" ,
          ""name"": ""America/Kralendijk"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/La_Paz"" ,
          ""name"": ""America/La Paz"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Lima"" ,
          ""name"": ""America/Lima"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Los_Angeles"" ,
          ""name"": ""America/Los Angeles"" ,
          ""utc_offset"": -25200 ,
        },
        { 
          ""id"": ""America/Lower_Princes"" ,
          ""name"": ""America/Lower Princes"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Maceio"" ,
          ""name"": ""America/Maceio"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Managua"" ,
          ""name"": ""America/Managua"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Manaus"" ,
          ""name"": ""America/Manaus"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Marigot"" ,
          ""name"": ""America/Marigot"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Martinique"" ,
          ""name"": ""America/Martinique"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Matamoros"" ,
          ""name"": ""America/Matamoros"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Mazatlan"" ,
          ""name"": ""America/Mazatlan"" ,
          ""utc_offset"": -25200 ,
        },
        { 
          ""id"": ""America/Menominee"" ,
          ""name"": ""America/Menominee"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Merida"" ,
          ""name"": ""America/Merida"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Metlakatla"" ,
          ""name"": ""America/Metlakatla"" ,
          ""utc_offset"": -28800 ,
        },
        { 
          ""id"": ""America/Mexico_City"" ,
          ""name"": ""America/Mexico City"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Miquelon"" ,
          ""name"": ""America/Miquelon"" ,
          ""utc_offset"": -7200 ,
        },
        { 
          ""id"": ""America/Moncton"" ,
          ""name"": ""America/Moncton"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Monterrey"" ,
          ""name"": ""America/Monterrey"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Montevideo"" ,
          ""name"": ""America/Montevideo"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Montserrat"" ,
          ""name"": ""America/Montserrat"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Nassau"" ,
          ""name"": ""America/Nassau"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/New_York"" ,
          ""name"": ""America/New York"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Nome"" ,
          ""name"": ""America/Nome"" ,
          ""utc_offset"": -28800 ,
        },
        { 
          ""id"": ""America/Noronha"" ,
          ""name"": ""America/Noronha"" ,
          ""utc_offset"": -7200 ,
        },
        { 
          ""id"": ""America/North_Dakota/Beulah"" ,
          ""name"": ""America/North Dakota/Beulah"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/North_Dakota/Center"" ,
          ""name"": ""America/North Dakota/Center"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/North_Dakota/New_Salem"" ,
          ""name"": ""America/North Dakota/New Salem"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Nuuk"" ,
          ""name"": ""America/Nuuk"" ,
          ""utc_offset"": -3600 ,
        },
        { 
          ""id"": ""America/Ojinaga"" ,
          ""name"": ""America/Ojinaga"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Panama"" ,
          ""name"": ""America/Panama"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Paramaribo"" ,
          ""name"": ""America/Paramaribo"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Phoenix"" ,
          ""name"": ""America/Phoenix"" ,
          ""utc_offset"": -25200 ,
        },
        { 
          ""id"": ""America/Port-au-Prince"" ,
          ""name"": ""America/Port-au-Prince"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Port_of_Spain"" ,
          ""name"": ""America/Port of Spain"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Porto_Velho"" ,
          ""name"": ""America/Porto Velho"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Puerto_Rico"" ,
          ""name"": ""America/Puerto Rico"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Punta_Arenas"" ,
          ""name"": ""America/Punta Arenas"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Rankin_Inlet"" ,
          ""name"": ""America/Rankin Inlet"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Recife"" ,
          ""name"": ""America/Recife"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Regina"" ,
          ""name"": ""America/Regina"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Resolute"" ,
          ""name"": ""America/Resolute"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Rio_Branco"" ,
          ""name"": ""America/Rio Branco"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Santarem"" ,
          ""name"": ""America/Santarem"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Santiago"" ,
          ""name"": ""America/Santiago"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Santo_Domingo"" ,
          ""name"": ""America/Santo Domingo"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Sao_Paulo"" ,
          ""name"": ""America/Sao Paulo"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Scoresbysund"" ,
          ""name"": ""America/Scoresbysund"" ,
          ""utc_offset"": -3600 ,
        },
        { 
          ""id"": ""America/Sitka"" ,
          ""name"": ""America/Sitka"" ,
          ""utc_offset"": -28800 ,
        },
        { 
          ""id"": ""America/St_Barthelemy"" ,
          ""name"": ""America/St Barthelemy"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/St_Johns"" ,
          ""name"": ""America/St Johns"" ,
          ""utc_offset"": -9000 ,
        },
        { 
          ""id"": ""America/St_Kitts"" ,
          ""name"": ""America/St Kitts"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/St_Lucia"" ,
          ""name"": ""America/St Lucia"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/St_Thomas"" ,
          ""name"": ""America/St Thomas"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/St_Vincent"" ,
          ""name"": ""America/St Vincent"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Swift_Current"" ,
          ""name"": ""America/Swift Current"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Tegucigalpa"" ,
          ""name"": ""America/Tegucigalpa"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""America/Thule"" ,
          ""name"": ""America/Thule"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""America/Tijuana"" ,
          ""name"": ""America/Tijuana"" ,
          ""utc_offset"": -25200 ,
        },
        { 
          ""id"": ""America/Toronto"" ,
          ""name"": ""America/Toronto"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Tortola"" ,
          ""name"": ""America/Tortola"" ,
          ""utc_offset"": -14400 ,
        },
        { 
          ""id"": ""America/Vancouver"" ,
          ""name"": ""America/Vancouver"" ,
          ""utc_offset"": -25200 ,
        },
        { 
          ""id"": ""America/Whitehorse"" ,
          ""name"": ""America/Whitehorse"" ,
          ""utc_offset"": -25200 ,
        },
        { 
          ""id"": ""America/Winnipeg"" ,
          ""name"": ""America/Winnipeg"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""America/Yakutat"" ,
          ""name"": ""America/Yakutat"" ,
          ""utc_offset"": -28800 ,
        },
        { 
          ""id"": ""Antarctica/Casey"" ,
          ""name"": ""Antarctica/Casey"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Antarctica/Davis"" ,
          ""name"": ""Antarctica/Davis"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Antarctica/DumontDUrville"" ,
          ""name"": ""Antarctica/DumontDUrville"" ,
          ""utc_offset"": 36000 ,
        },
        { 
          ""id"": ""Antarctica/Macquarie"" ,
          ""name"": ""Antarctica/Macquarie"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Antarctica/Mawson"" ,
          ""name"": ""Antarctica/Mawson"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Antarctica/McMurdo"" ,
          ""name"": ""Antarctica/McMurdo"" ,
          ""utc_offset"": 46800 ,
        },
        { 
          ""id"": ""Antarctica/Palmer"" ,
          ""name"": ""Antarctica/Palmer"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""Antarctica/Rothera"" ,
          ""name"": ""Antarctica/Rothera"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""Antarctica/Syowa"" ,
          ""name"": ""Antarctica/Syowa"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Antarctica/Troll"" ,
          ""name"": ""Antarctica/Troll"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Antarctica/Vostok"" ,
          ""name"": ""Antarctica/Vostok"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Arctic/Longyearbyen"" ,
          ""name"": ""Arctic/Longyearbyen"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Asia/Aden"" ,
          ""name"": ""Asia/Aden"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Asia/Almaty"" ,
          ""name"": ""Asia/Almaty"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Amman"" ,
          ""name"": ""Asia/Amman"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Asia/Anadyr"" ,
          ""name"": ""Asia/Anadyr"" ,
          ""utc_offset"": 43200 ,
        },
        { 
          ""id"": ""Asia/Aqtau"" ,
          ""name"": ""Asia/Aqtau"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Aqtobe"" ,
          ""name"": ""Asia/Aqtobe"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Ashgabat"" ,
          ""name"": ""Asia/Ashgabat"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Atyrau"" ,
          ""name"": ""Asia/Atyrau"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Baghdad"" ,
          ""name"": ""Asia/Baghdad"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Asia/Bahrain"" ,
          ""name"": ""Asia/Bahrain"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Asia/Baku"" ,
          ""name"": ""Asia/Baku"" ,
          ""utc_offset"": 14400 ,
        },
        { 
          ""id"": ""Asia/Bangkok"" ,
          ""name"": ""Asia/Bangkok"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Asia/Barnaul"" ,
          ""name"": ""Asia/Barnaul"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Asia/Beirut"" ,
          ""name"": ""Asia/Beirut"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Asia/Bishkek"" ,
          ""name"": ""Asia/Bishkek"" ,
          ""utc_offset"": 21600 ,
        },
        { 
          ""id"": ""Asia/Brunei"" ,
          ""name"": ""Asia/Brunei"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Chita"" ,
          ""name"": ""Asia/Chita"" ,
          ""utc_offset"": 32400 ,
        },
        { 
          ""id"": ""Asia/Choibalsan"" ,
          ""name"": ""Asia/Choibalsan"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Colombo"" ,
          ""name"": ""Asia/Colombo"" ,
          ""utc_offset"": 19800 ,
        },
        { 
          ""id"": ""Asia/Damascus"" ,
          ""name"": ""Asia/Damascus"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Asia/Dhaka"" ,
          ""name"": ""Asia/Dhaka"" ,
          ""utc_offset"": 21600 ,
        },
        { 
          ""id"": ""Asia/Dili"" ,
          ""name"": ""Asia/Dili"" ,
          ""utc_offset"": 32400 ,
        },
        { 
          ""id"": ""Asia/Dubai"" ,
          ""name"": ""Asia/Dubai"" ,
          ""utc_offset"": 14400 ,
        },
        { 
          ""id"": ""Asia/Dushanbe"" ,
          ""name"": ""Asia/Dushanbe"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Famagusta"" ,
          ""name"": ""Asia/Famagusta"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Asia/Gaza"" ,
          ""name"": ""Asia/Gaza"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Asia/Hebron"" ,
          ""name"": ""Asia/Hebron"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Asia/Ho_Chi_Minh"" ,
          ""name"": ""Asia/Ho Chi Minh"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Asia/Hong_Kong"" ,
          ""name"": ""Asia/Hong Kong"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Hovd"" ,
          ""name"": ""Asia/Hovd"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Asia/Irkutsk"" ,
          ""name"": ""Asia/Irkutsk"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Jakarta"" ,
          ""name"": ""Asia/Jakarta"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Asia/Jayapura"" ,
          ""name"": ""Asia/Jayapura"" ,
          ""utc_offset"": 32400 ,
        },
        { 
          ""id"": ""Asia/Jerusalem"" ,
          ""name"": ""Asia/Jerusalem"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Asia/Kabul"" ,
          ""name"": ""Asia/Kabul"" ,
          ""utc_offset"": 16200 ,
        },
        { 
          ""id"": ""Asia/Kamchatka"" ,
          ""name"": ""Asia/Kamchatka"" ,
          ""utc_offset"": 43200 ,
        },
        { 
          ""id"": ""Asia/Karachi"" ,
          ""name"": ""Asia/Karachi"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Kathmandu"" ,
          ""name"": ""Asia/Kathmandu"" ,
          ""utc_offset"": 20700 ,
        },
        { 
          ""id"": ""Asia/Khandyga"" ,
          ""name"": ""Asia/Khandyga"" ,
          ""utc_offset"": 32400 ,
        },
        { 
          ""id"": ""Asia/Kolkata"" ,
          ""name"": ""Asia/Kolkata"" ,
          ""utc_offset"": 19800 ,
        },
        { 
          ""id"": ""Asia/Krasnoyarsk"" ,
          ""name"": ""Asia/Krasnoyarsk"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Asia/Kuala_Lumpur"" ,
          ""name"": ""Asia/Kuala Lumpur"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Kuching"" ,
          ""name"": ""Asia/Kuching"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Kuwait"" ,
          ""name"": ""Asia/Kuwait"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Asia/Macau"" ,
          ""name"": ""Asia/Macau"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Magadan"" ,
          ""name"": ""Asia/Magadan"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Asia/Makassar"" ,
          ""name"": ""Asia/Makassar"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Manila"" ,
          ""name"": ""Asia/Manila"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Muscat"" ,
          ""name"": ""Asia/Muscat"" ,
          ""utc_offset"": 14400 ,
        },
        { 
          ""id"": ""Asia/Nicosia"" ,
          ""name"": ""Asia/Nicosia"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Asia/Novokuznetsk"" ,
          ""name"": ""Asia/Novokuznetsk"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Asia/Novosibirsk"" ,
          ""name"": ""Asia/Novosibirsk"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Asia/Omsk"" ,
          ""name"": ""Asia/Omsk"" ,
          ""utc_offset"": 21600 ,
        },
        { 
          ""id"": ""Asia/Oral"" ,
          ""name"": ""Asia/Oral"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Phnom_Penh"" ,
          ""name"": ""Asia/Phnom Penh"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Asia/Pontianak"" ,
          ""name"": ""Asia/Pontianak"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Asia/Pyongyang"" ,
          ""name"": ""Asia/Pyongyang"" ,
          ""utc_offset"": 32400 ,
        },
        { 
          ""id"": ""Asia/Qatar"" ,
          ""name"": ""Asia/Qatar"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Asia/Qostanay"" ,
          ""name"": ""Asia/Qostanay"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Qyzylorda"" ,
          ""name"": ""Asia/Qyzylorda"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Riyadh"" ,
          ""name"": ""Asia/Riyadh"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Asia/Sakhalin"" ,
          ""name"": ""Asia/Sakhalin"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Asia/Samarkand"" ,
          ""name"": ""Asia/Samarkand"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Seoul"" ,
          ""name"": ""Asia/Seoul"" ,
          ""utc_offset"": 32400 ,
        },
        { 
          ""id"": ""Asia/Shanghai"" ,
          ""name"": ""Asia/Shanghai"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Singapore"" ,
          ""name"": ""Asia/Singapore"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Srednekolymsk"" ,
          ""name"": ""Asia/Srednekolymsk"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Asia/Taipei"" ,
          ""name"": ""Asia/Taipei"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Tashkent"" ,
          ""name"": ""Asia/Tashkent"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Tbilisi"" ,
          ""name"": ""Asia/Tbilisi"" ,
          ""utc_offset"": 14400 ,
        },
        { 
          ""id"": ""Asia/Tehran"" ,
          ""name"": ""Asia/Tehran"" ,
          ""utc_offset"": 12600 ,
        },
        { 
          ""id"": ""Asia/Thimphu"" ,
          ""name"": ""Asia/Thimphu"" ,
          ""utc_offset"": 21600 ,
        },
        { 
          ""id"": ""Asia/Tokyo"" ,
          ""name"": ""Asia/Tokyo"" ,
          ""utc_offset"": 32400 ,
        },
        { 
          ""id"": ""Asia/Tomsk"" ,
          ""name"": ""Asia/Tomsk"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Asia/Ulaanbaatar"" ,
          ""name"": ""Asia/Ulaanbaatar"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Asia/Urumqi"" ,
          ""name"": ""Asia/Urumqi"" ,
          ""utc_offset"": 21600 ,
        },
        { 
          ""id"": ""Asia/Ust-Nera"" ,
          ""name"": ""Asia/Ust-Nera"" ,
          ""utc_offset"": 36000 ,
        },
        { 
          ""id"": ""Asia/Vientiane"" ,
          ""name"": ""Asia/Vientiane"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Asia/Vladivostok"" ,
          ""name"": ""Asia/Vladivostok"" ,
          ""utc_offset"": 36000 ,
        },
        { 
          ""id"": ""Asia/Yakutsk"" ,
          ""name"": ""Asia/Yakutsk"" ,
          ""utc_offset"": 32400 ,
        },
        { 
          ""id"": ""Asia/Yangon"" ,
          ""name"": ""Asia/Yangon"" ,
          ""utc_offset"": 23400 ,
        },
        { 
          ""id"": ""Asia/Yekaterinburg"" ,
          ""name"": ""Asia/Yekaterinburg"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Asia/Yerevan"" ,
          ""name"": ""Asia/Yerevan"" ,
          ""utc_offset"": 14400 ,
        },
        { 
          ""id"": ""Atlantic/Azores"" ,
          ""name"": ""Atlantic/Azores"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Atlantic/Bermuda"" ,
          ""name"": ""Atlantic/Bermuda"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""Atlantic/Canary"" ,
          ""name"": ""Atlantic/Canary"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Atlantic/Cape_Verde"" ,
          ""name"": ""Atlantic/Cape Verde"" ,
          ""utc_offset"": -3600 ,
        },
        { 
          ""id"": ""Atlantic/Faroe"" ,
          ""name"": ""Atlantic/Faroe"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Atlantic/Madeira"" ,
          ""name"": ""Atlantic/Madeira"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Atlantic/Reykjavik"" ,
          ""name"": ""Atlantic/Reykjavik"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Atlantic/South_Georgia"" ,
          ""name"": ""Atlantic/South Georgia"" ,
          ""utc_offset"": -7200 ,
        },
        { 
          ""id"": ""Atlantic/St_Helena"" ,
          ""name"": ""Atlantic/St Helena"" ,
          ""utc_offset"": 0 ,
        },
        { 
          ""id"": ""Atlantic/Stanley"" ,
          ""name"": ""Atlantic/Stanley"" ,
          ""utc_offset"": -10800 ,
        },
        { 
          ""id"": ""Australia/Adelaide"" ,
          ""name"": ""Australia/Adelaide"" ,
          ""utc_offset"": 37800 ,
        },
        { 
          ""id"": ""Australia/Brisbane"" ,
          ""name"": ""Australia/Brisbane"" ,
          ""utc_offset"": 36000 ,
        },
        { 
          ""id"": ""Australia/Broken_Hill"" ,
          ""name"": ""Australia/Broken Hill"" ,
          ""utc_offset"": 37800 ,
        },
        { 
          ""id"": ""Australia/Darwin"" ,
          ""name"": ""Australia/Darwin"" ,
          ""utc_offset"": 34200 ,
        },
        { 
          ""id"": ""Australia/Eucla"" ,
          ""name"": ""Australia/Eucla"" ,
          ""utc_offset"": 31500 ,
        },
        { 
          ""id"": ""Australia/Hobart"" ,
          ""name"": ""Australia/Hobart"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Australia/Lindeman"" ,
          ""name"": ""Australia/Lindeman"" ,
          ""utc_offset"": 36000 ,
        },
        { 
          ""id"": ""Australia/Lord_Howe"" ,
          ""name"": ""Australia/Lord Howe"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Australia/Melbourne"" ,
          ""name"": ""Australia/Melbourne"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Australia/Perth"" ,
          ""name"": ""Australia/Perth"" ,
          ""utc_offset"": 28800 ,
        },
        { 
          ""id"": ""Australia/Sydney"" ,
          ""name"": ""Australia/Sydney"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Europe/Amsterdam"" ,
          ""name"": ""Europe/Amsterdam"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Andorra"" ,
          ""name"": ""Europe/Andorra"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Astrakhan"" ,
          ""name"": ""Europe/Astrakhan"" ,
          ""utc_offset"": 14400 ,
        },
        { 
          ""id"": ""Europe/Athens"" ,
          ""name"": ""Europe/Athens"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Belgrade"" ,
          ""name"": ""Europe/Belgrade"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Berlin"" ,
          ""name"": ""Europe/Berlin"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Bratislava"" ,
          ""name"": ""Europe/Bratislava"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Brussels"" ,
          ""name"": ""Europe/Brussels"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Bucharest"" ,
          ""name"": ""Europe/Bucharest"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Budapest"" ,
          ""name"": ""Europe/Budapest"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Busingen"" ,
          ""name"": ""Europe/Busingen"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Chisinau"" ,
          ""name"": ""Europe/Chisinau"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Copenhagen"" ,
          ""name"": ""Europe/Copenhagen"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Dublin"" ,
          ""name"": ""Europe/Dublin"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Europe/Gibraltar"" ,
          ""name"": ""Europe/Gibraltar"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Guernsey"" ,
          ""name"": ""Europe/Guernsey"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Europe/Helsinki"" ,
          ""name"": ""Europe/Helsinki"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Isle_of_Man"" ,
          ""name"": ""Europe/Isle of Man"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Europe/Istanbul"" ,
          ""name"": ""Europe/Istanbul"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Jersey"" ,
          ""name"": ""Europe/Jersey"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Europe/Kaliningrad"" ,
          ""name"": ""Europe/Kaliningrad"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Kirov"" ,
          ""name"": ""Europe/Kirov"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Kyiv"" ,
          ""name"": ""Europe/Kyiv"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Lisbon"" ,
          ""name"": ""Europe/Lisbon"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Europe/Ljubljana"" ,
          ""name"": ""Europe/Ljubljana"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/London"" ,
          ""name"": ""Europe/London"" ,
          ""utc_offset"": 3600 ,
        },
        { 
          ""id"": ""Europe/Luxembourg"" ,
          ""name"": ""Europe/Luxembourg"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Madrid"" ,
          ""name"": ""Europe/Madrid"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Malta"" ,
          ""name"": ""Europe/Malta"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Mariehamn"" ,
          ""name"": ""Europe/Mariehamn"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Minsk"" ,
          ""name"": ""Europe/Minsk"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Monaco"" ,
          ""name"": ""Europe/Monaco"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Moscow"" ,
          ""name"": ""Europe/Moscow"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Oslo"" ,
          ""name"": ""Europe/Oslo"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Paris"" ,
          ""name"": ""Europe/Paris"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Podgorica"" ,
          ""name"": ""Europe/Podgorica"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Prague"" ,
          ""name"": ""Europe/Prague"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Riga"" ,
          ""name"": ""Europe/Riga"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Rome"" ,
          ""name"": ""Europe/Rome"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Samara"" ,
          ""name"": ""Europe/Samara"" ,
          ""utc_offset"": 14400 ,
        },
        { 
          ""id"": ""Europe/San_Marino"" ,
          ""name"": ""Europe/San Marino"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Sarajevo"" ,
          ""name"": ""Europe/Sarajevo"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Saratov"" ,
          ""name"": ""Europe/Saratov"" ,
          ""utc_offset"": 14400 ,
        },
        { 
          ""id"": ""Europe/Simferopol"" ,
          ""name"": ""Europe/Simferopol"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Skopje"" ,
          ""name"": ""Europe/Skopje"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Sofia"" ,
          ""name"": ""Europe/Sofia"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Stockholm"" ,
          ""name"": ""Europe/Stockholm"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Tallinn"" ,
          ""name"": ""Europe/Tallinn"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Tirane"" ,
          ""name"": ""Europe/Tirane"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Ulyanovsk"" ,
          ""name"": ""Europe/Ulyanovsk"" ,
          ""utc_offset"": 14400 ,
        },
        { 
          ""id"": ""Europe/Vaduz"" ,
          ""name"": ""Europe/Vaduz"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Vatican"" ,
          ""name"": ""Europe/Vatican"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Vienna"" ,
          ""name"": ""Europe/Vienna"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Vilnius"" ,
          ""name"": ""Europe/Vilnius"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Volgograd"" ,
          ""name"": ""Europe/Volgograd"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Europe/Warsaw"" ,
          ""name"": ""Europe/Warsaw"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Zagreb"" ,
          ""name"": ""Europe/Zagreb"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Europe/Zurich"" ,
          ""name"": ""Europe/Zurich"" ,
          ""utc_offset"": 7200 ,
        },
        { 
          ""id"": ""Indian/Antananarivo"" ,
          ""name"": ""Indian/Antananarivo"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Indian/Chagos"" ,
          ""name"": ""Indian/Chagos"" ,
          ""utc_offset"": 21600 ,
        },
        { 
          ""id"": ""Indian/Christmas"" ,
          ""name"": ""Indian/Christmas"" ,
          ""utc_offset"": 25200 ,
        },
        { 
          ""id"": ""Indian/Cocos"" ,
          ""name"": ""Indian/Cocos"" ,
          ""utc_offset"": 23400 ,
        },
        { 
          ""id"": ""Indian/Comoro"" ,
          ""name"": ""Indian/Comoro"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Indian/Kerguelen"" ,
          ""name"": ""Indian/Kerguelen"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Indian/Mahe"" ,
          ""name"": ""Indian/Mahe"" ,
          ""utc_offset"": 14400 ,
        },
        { 
          ""id"": ""Indian/Maldives"" ,
          ""name"": ""Indian/Maldives"" ,
          ""utc_offset"": 18000 ,
        },
        { 
          ""id"": ""Indian/Mauritius"" ,
          ""name"": ""Indian/Mauritius"" ,
          ""utc_offset"": 14400 ,
        },
        { 
          ""id"": ""Indian/Mayotte"" ,
          ""name"": ""Indian/Mayotte"" ,
          ""utc_offset"": 10800 ,
        },
        { 
          ""id"": ""Indian/Reunion"" ,
          ""name"": ""Indian/Reunion"" ,
          ""utc_offset"": 14400 ,
        },
        { 
          ""id"": ""Pacific/Apia"" ,
          ""name"": ""Pacific/Apia"" ,
          ""utc_offset"": 46800 ,
        },
        { 
          ""id"": ""Pacific/Auckland"" ,
          ""name"": ""Pacific/Auckland"" ,
          ""utc_offset"": 46800 ,
        },
        { 
          ""id"": ""Pacific/Bougainville"" ,
          ""name"": ""Pacific/Bougainville"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Pacific/Chatham"" ,
          ""name"": ""Pacific/Chatham"" ,
          ""utc_offset"": 49500 ,
        },
        { 
          ""id"": ""Pacific/Chuuk"" ,
          ""name"": ""Pacific/Chuuk"" ,
          ""utc_offset"": 36000 ,
        },
        { 
          ""id"": ""Pacific/Easter"" ,
          ""name"": ""Pacific/Easter"" ,
          ""utc_offset"": -18000 ,
        },
        { 
          ""id"": ""Pacific/Efate"" ,
          ""name"": ""Pacific/Efate"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Pacific/Fakaofo"" ,
          ""name"": ""Pacific/Fakaofo"" ,
          ""utc_offset"": 46800 ,
        },
        { 
          ""id"": ""Pacific/Fiji"" ,
          ""name"": ""Pacific/Fiji"" ,
          ""utc_offset"": 43200 ,
        },
        { 
          ""id"": ""Pacific/Funafuti"" ,
          ""name"": ""Pacific/Funafuti"" ,
          ""utc_offset"": 43200 ,
        },
        { 
          ""id"": ""Pacific/Galapagos"" ,
          ""name"": ""Pacific/Galapagos"" ,
          ""utc_offset"": -21600 ,
        },
        { 
          ""id"": ""Pacific/Gambier"" ,
          ""name"": ""Pacific/Gambier"" ,
          ""utc_offset"": -32400 ,
        },
        { 
          ""id"": ""Pacific/Guadalcanal"" ,
          ""name"": ""Pacific/Guadalcanal"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Pacific/Guam"" ,
          ""name"": ""Pacific/Guam"" ,
          ""utc_offset"": 36000 ,
        },
        { 
          ""id"": ""Pacific/Honolulu"" ,
          ""name"": ""Pacific/Honolulu"" ,
          ""utc_offset"": -36000 ,
        },
        { 
          ""id"": ""Pacific/Kanton"" ,
          ""name"": ""Pacific/Kanton"" ,
          ""utc_offset"": 46800 ,
        },
        { 
          ""id"": ""Pacific/Kiritimati"" ,
          ""name"": ""Pacific/Kiritimati"" ,
          ""utc_offset"": 50400 ,
        },
        { 
          ""id"": ""Pacific/Kosrae"" ,
          ""name"": ""Pacific/Kosrae"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Pacific/Kwajalein"" ,
          ""name"": ""Pacific/Kwajalein"" ,
          ""utc_offset"": 43200 ,
        },
        { 
          ""id"": ""Pacific/Majuro"" ,
          ""name"": ""Pacific/Majuro"" ,
          ""utc_offset"": 43200 ,
        },
        { 
          ""id"": ""Pacific/Marquesas"" ,
          ""name"": ""Pacific/Marquesas"" ,
          ""utc_offset"": -34200 ,
        },
        { 
          ""id"": ""Pacific/Midway"" ,
          ""name"": ""Pacific/Midway"" ,
          ""utc_offset"": -39600 ,
        },
        { 
          ""id"": ""Pacific/Nauru"" ,
          ""name"": ""Pacific/Nauru"" ,
          ""utc_offset"": 43200 ,
        },
        { 
          ""id"": ""Pacific/Niue"" ,
          ""name"": ""Pacific/Niue"" ,
          ""utc_offset"": -39600 ,
        },
        { 
          ""id"": ""Pacific/Norfolk"" ,
          ""name"": ""Pacific/Norfolk"" ,
          ""utc_offset"": 43200 ,
        },
        { 
          ""id"": ""Pacific/Noumea"" ,
          ""name"": ""Pacific/Noumea"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Pacific/Pago_Pago"" ,
          ""name"": ""Pacific/Pago Pago"" ,
          ""utc_offset"": -39600 ,
        },
        { 
          ""id"": ""Pacific/Palau"" ,
          ""name"": ""Pacific/Palau"" ,
          ""utc_offset"": 32400 ,
        },
        { 
          ""id"": ""Pacific/Pitcairn"" ,
          ""name"": ""Pacific/Pitcairn"" ,
          ""utc_offset"": -28800 ,
        },
        { 
          ""id"": ""Pacific/Pohnpei"" ,
          ""name"": ""Pacific/Pohnpei"" ,
          ""utc_offset"": 39600 ,
        },
        { 
          ""id"": ""Pacific/Port_Moresby"" ,
          ""name"": ""Pacific/Port Moresby"" ,
          ""utc_offset"": 36000 ,
        },
        { 
          ""id"": ""Pacific/Rarotonga"" ,
          ""name"": ""Pacific/Rarotonga"" ,
          ""utc_offset"": -36000 ,
        },
        { 
          ""id"": ""Pacific/Saipan"" ,
          ""name"": ""Pacific/Saipan"" ,
          ""utc_offset"": 36000 ,
        },
        { 
          ""id"": ""Pacific/Tahiti"" ,
          ""name"": ""Pacific/Tahiti"" ,
          ""utc_offset"": -36000 ,
        },
        { 
          ""id"": ""Pacific/Tarawa"" ,
          ""name"": ""Pacific/Tarawa"" ,
          ""utc_offset"": 43200 ,
        },
        { 
          ""id"": ""Pacific/Tongatapu"" ,
          ""name"": ""Pacific/Tongatapu"" ,
          ""utc_offset"": 46800 ,
        },
        { 
          ""id"": ""Pacific/Wake"" ,
          ""name"": ""Pacific/Wake"" ,
          ""utc_offset"": 43200 ,
        },
        { 
          ""id"": ""Pacific/Wallis"" ,
          ""name"": ""Pacific/Wallis"" ,
          ""utc_offset"": 43200 ,
        },
        { 
          ""id"": ""UTC"" ,
          ""name"": ""UTC"" ,
          ""utc_offset"": 0 ,
        }]";

    private static List<TimeZoneItem> _allZoneItems = [];
    private static FrozenDictionary<string, TimeZoneItem>? _timeZoneItems;

    public IReadOnlyCollection<TimeZoneItem> GetTimeZoneList()
    {
        LoadAllTimezonesIfNeed();

        return _allZoneItems;
    }

    public TimeZoneItem? GetTimeZoneItem(string timezoneName)
    {
        LoadAllTimezonesIfNeed();

        return _timeZoneItems!.GetValueOrDefault(timezoneName);
    }

    public bool TryGeTimeZoneItem(string timezoneName, [NotNullWhen(true)] out TimeZoneItem? item)
    {
        LoadAllTimezonesIfNeed();

        return _timeZoneItems!.TryGetValue(timezoneName, out item);
    }

    private void LoadAllTimezonesIfNeed()
    {
        if (_timeZoneItems == null || _allZoneItems.Count == 0)
        {
            _allZoneItems = JsonSerializer.Deserialize<List<TimeZoneItem>>(AllTimezonesJsonText,
                new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                }) ?? [];

            _timeZoneItems = _allZoneItems.ToDictionary(k => k.Name, v => v).ToFrozenDictionary();
        }
    }
}