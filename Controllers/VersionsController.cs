using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Version = CustomerApi.Models.Version;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionsController : ControllerBase
    {
        Version _oVersion = new Version();
        List<Version> _oVersions = new List<Version>();
        [HttpGet]
        public async Task<List<Version>> GetVersion()
        {
            _oVersions = new List<Version>();
            var connetionString = "Data Source=DESKTOP-Q03LFQJ\\SQLEXPRESS;Initial Catalog=Hrmaps.Master;Integrated Security=True";
            var sql = "select * from Application.AppVersion ; ";
            //string result = "";
            try
            {
                using (var connection = new SqlConnection(connetionString))
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _oVersion = new Version();
                                _oVersion.Id = (int)reader["Id"];
                                _oVersion.VersionNumber = String.Format("{0}", reader["VersionNumber"]);
                                _oVersion.Date = String.Format("{0}", reader["Date"]);
                                if (String.Format("{0}", reader["Description"]) != "")
                                    _oVersion.Description = String.Format("{0}", reader["Description"]);
                                if (String.Format("{0}", reader["IsDeleted"]) == "True")
                                    _oVersion.IsDeleted = 1;
                                else
                                    _oVersion.IsDeleted = 0;
                                _oVersions.Add(_oVersion);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }

            return _oVersions;
        }

        [HttpGet("{Id}")]
        public async Task<Version> GetVersion(int Id)
        {
            _oVersion = new Version();
            var connetionString = "Data Source=DESKTOP-Q03LFQJ\\SQLEXPRESS;Initial Catalog=Hrmaps.Master;Integrated Security=True";
            var sql = "select * from Application.AppVersion where (Id = "+Id+"); ";
            try
            {
                using (var connection = new SqlConnection(connetionString))
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _oVersion.Id = (int)reader["Id"];
                                _oVersion.VersionNumber = String.Format("{0}", reader["VersionNumber"]);
                                _oVersion.Date = String.Format("{0}", reader["Date"]);
                                if (String.Format("{0}", reader["Description"]) != "")
                                    _oVersion.Description = String.Format("{0}", reader["Description"]);
                                if (String.Format("{0}", reader["IsDeleted"]) == "True")
                                    _oVersion.IsDeleted = 1;
                                else
                                    _oVersion.IsDeleted = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }

            return _oVersion;
        }

        [HttpPost]
        public async Task<Version> PostVersion(Version version)
        {

            var sql = "INSERT INTO Application.AppVersion VALUES ('"+version.VersionNumber+"', '"+version.Date+"', '"+version.Description+"', "+version.IsDeleted+"); ";
            var connetionString = "Data Source=DESKTOP-Q03LFQJ\\SQLEXPRESS;Initial Catalog=Hrmaps.Master;Integrated Security=True";            
            try
            {
                using (var connection = new SqlConnection(connetionString))
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch { }
            return version;
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> PutVersion(int Id, Version version)
        {
            if (Id != version.Id)
            {
                return BadRequest();
            }
            var sql = "UPDATE Application.AppVersion SET VersionNumber = '"+version.VersionNumber+"', Date = '"+version.Date+"', Description = '"+version.Description+"', IsDeleted = "+version.IsDeleted+" WHERE Id = "+Id+";";
            var connetionString = "Data Source=DESKTOP-Q03LFQJ\\SQLEXPRESS;Initial Catalog=Hrmaps.Master;Integrated Security=True";
            try
            {
                using (var connection = new SqlConnection(connetionString))
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{Id}")]
        public async Task<string> DeleteVersion(int Id)
        {
            string message = "";
            var sql = "DELETE FROM Application.AppVersion where Id = "+Id+" ;";
            var connetionString = "Data Source=DESKTOP-Q03LFQJ\\SQLEXPRESS;Initial Catalog=Hrmaps.Master;Integrated Security=True";
            try
            {
                using (var connection = new SqlConnection(connetionString))
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                message = "succes";
            }
            catch
            {
                message="erreur";
            }

            return message;
        }
    }
}
