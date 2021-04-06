namespace Geocodificador.Librery
{
    /// <summary>
    /// Esta clase tiene contiene informacion necesaria para ser procesada y enviada.
    /// </summary>
    public class LocalizadorData
    {
        public long id{ get; set; }
        public double latitud{ get; set; }
        public double longitud{ get; set; }
        public string estado{ get; set; }
        public string calle{ get; set; }
        public int numero{ get; set; }
        public string ciudad{ get; set; }
        public string codigo_postal{ get; set; }
        public string provincia{ get; set; }
        public string pais{ get; set; }
    }
}