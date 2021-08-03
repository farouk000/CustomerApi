using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Version = CustomerApi.Models.Version;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly CustomerApiContext _context;

        public StatsController(CustomerApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Stat>> GetStat()
        {
            Stat stat = new Stat();
            List<Customer> customers = await _context.Customer.ToListAsync();
            stat.Customers = customers.Count();
            var connetionString = "Data Source=DESKTOP-Q03LFQJ\\SQLEXPRESS;Initial Catalog=Hrmaps.Master;Integrated Security=True";
            var sql = "select * from Application.AppVersion ; ";
            int result = 0;
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
                                result = result + 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            stat.Versions = result;
            List<User> admins = await _context.User.ToListAsync();
            stat.Admins = admins.Count();
            return stat;
        }

        
    }
}
