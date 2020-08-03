using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using API_GEO.Models;
using Microsoft.Data.SqlClient;

namespace API_GEO.Librery
{
    public class SQLClient
    {
        private readonly ApiContext _context;

        public SQLClient(ApiContext context)
        {
            _context = context;
        }

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

        public LocalizadorData GetID(long _id)
        {
            var LocalizadoData = _context.GetID(_id);

            if(LocalizadoData == null)
            {
                return null;
            }
            return LocalizadoData;
        }

        public bool Update(long _id, float _latitud, float _longitud, string _estado)
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

        public long AddNew(LocalizadorData _LocData)
        {
            _context.SetLocalizadorData(_LocData);
            long v = _LocData.id;
            return v;
        }
    }
}