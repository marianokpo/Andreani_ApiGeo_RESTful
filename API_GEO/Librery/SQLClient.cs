using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using API_GEO.Models;
using Microsoft.Data.SqlClient;

namespace API_GEO.Librery
{
    /// <summary>
    /// Clase definida para realizar todos lo necesario en la base de datos. ABM
    /// </summary>
    public class SQLClient
    {
        private readonly ApiContext _context;

        public SQLClient(ApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica si la base de datos existe.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <returns>true/false</returns>
        public static bool CheckDatabaseExist(string connectionString, string databaseName)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand($"SELECT db_id('{databaseName}')", connection))
                    {
                            connection.Open();
                            bool res = command.ExecuteScalar() != DBNull.Value;
                            return res;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Crea la base de datos.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static bool CreateDataBase(string connectionString)
        {
            string root = @"C:\DB";        
            // Verificamos si el directorio esta creado. y de no ser asi lo creamos.
            if (!Directory.Exists(root))  
            {  
                Directory.CreateDirectory(root);
            } 

            string script = File.ReadAllText(@".\API_GEO_DB.sql");

            try
            {
                IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                SqlConnection Connection = new SqlConnection(connectionString);
                Connection.Open();
                foreach (string commandString in commandStrings)
                {
                    if (!string.IsNullOrWhiteSpace(commandString.Trim()))
                    {
                        using(var command = new SqlCommand(commandString, Connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }     
                Connection.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Actualiza los parametros latitud, longitud y estado del id correspondiente.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_latitud"></param>
        /// <param name="_longitud"></param>
        /// <param name="_estado"></param>
        public static void UpdateLanLon(long _id, double _latitud, double _longitud, string _estado)
        {
            SqlConnection Connection = new SqlConnection(Startup.ConnecStringSQL);;
            SqlCommand command;
            try
            {                
                SqlDataAdapter adapter = new SqlDataAdapter(); 
                
                Connection.Open();

                string sqlq = "update LocalizadorData set latitud=" + _latitud + ", longitud=" + _longitud +
                ", estado='" + _estado + "' where id="+_id;

                command = new SqlCommand(sqlq,Connection);
		
                adapter.InsertCommand = new SqlCommand(sqlq,Connection); 
                adapter.InsertCommand.ExecuteNonQuery();

                command.Dispose();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        /// <summary>
        /// Obtiene el objeto LocalizadorData desde el id
        /// </summary>
        /// <param name="_id"></param>
        /// <returns>new LocalizadorData</returns>
        public LocalizadorData GetID(long _id)
        {
            var LocalizadoData = _context.GetID(_id);

            if(LocalizadoData == null)
            {
                return null;
            }
            return LocalizadoData;
        }

        /// <summary>
        /// otra forma de actualizar la base de datos con los nuevos datos de latitud, longitud y estado.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_latitud"></param>
        /// <param name="_longitud"></param>
        /// <param name="_estado"></param>
        /// <returns></returns>
        public bool Update(long _id, double _latitud, double _longitud, string _estado)
        {
            var LocalizadoData = _context.GetID(_id);

            if(LocalizadoData == null)
            {
                return false;
            }

            LocalizadoData.latitud = _latitud;
            LocalizadoData.longitud = _longitud;
            LocalizadoData.estado = _estado;

            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Actualiza un objeto en la base de datos.
        /// </summary>
        /// <param name="LD"></param>
        /// <returns></returns>
        public bool Update(LocalizadorData LD)
        {
            var LocalizadoData = _context.GetID(LD.id);

            if(LocalizadoData == null)
            {
                return false;
            }

            LocalizadoData.latitud = LD.latitud;
            LocalizadoData.longitud = LD.longitud;
            LocalizadoData.estado = LD.estado;

            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Agrega un nuevo objeto a la base de datos.
        /// </summary>
        /// <param name="_LocData"></param>
        /// <returns></returns>
        public long AddNew(LocalizadorData _LocData)
        {
            _context.SetLocalizadorData(_LocData);
            long v = _LocData.id;
            return v;
        }
    }
}