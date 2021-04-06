using Nominatim.API.Geocoders;
using Nominatim.API.Models;
using System.Threading.Tasks;

namespace Geocodificador.Librery
{
    /// <summary>
    /// Esta clase se encarga de buscar la informacion faltante de LocalizadorData
    /// </summary>
    public class OSM_FG
    {
        /// <summary>
        /// Esta funcion busca a traves de las direccion proporcionada por el objeto LocalizadorData
        /// la latitud y la longitud de dicha direccion.
        /// </summary>
        /// <param name="LD"></param>
        /// <returns>LD</returns>
        public static LocalizadorData Find(LocalizadorData LD)
        {
            ForwardGeocodeRequest d = new ForwardGeocodeRequest();
            GeocodeResponse g = new GeocodeResponse();
            d.City = LD.ciudad;
            d.Country = LD.pais;
            d.StreetAddress = LD.calle + " " + LD.numero.ToString();
            d.County = LD.provincia;
            ForwardGeocoder FG = new ForwardGeocoder();
            Task<GeocodeResponse[]> r = FG.Geocode(d);
            g = r.Result[0];
            LD.latitud = g.Latitude;
            LD.longitud = g.Longitude;
            return LD;
        }
        /// <summary>
        /// Esta funcion busc a travez de la latitud y la longitud la direccion correspondiente
        /// Devolviendo un objeto de tipo LocalizadorData con los datos correspondienes.
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns>new LocalizadorData</returns>
        public static LocalizadorData Find(double lat, double lon)
        {
            LocalizadorData LD = new LocalizadorData();
            ReverseGeocodeRequest g = new ReverseGeocodeRequest();
            GeocodeResponse r = new GeocodeResponse();
            ReverseGeocoder RG = new ReverseGeocoder();
            g.Latitude = lat;
            g.Longitude = lon;
            Task<GeocodeResponse> rs = RG.ReverseGeocode(g);
            r = rs.Result;
            LD.calle = r.Address.Road;
            return LD;
        }
    }
}