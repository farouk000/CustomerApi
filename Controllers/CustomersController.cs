using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Web.Administration;
using Microsoft.Data.SqlClient;
using System.IO;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerApiContext _context;

        public CustomersController(CustomerApiContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
        {
            List<Customer> customers = await _context.Customer.ToListAsync();
            foreach (Customer cu in customers)
            {
               if(cu.VersionId != 0)
                    cu.VersionNumber = GetVersionNumber(cu.VersionId);
             
            }
            return customers;
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customer.FindAsync(id);

            if (customer != null)
            {
                if (customer.VersionId != 0)
                    customer.VersionNumber = GetVersionNumber(customer.VersionId);
            }

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            var customer1 = await _context.Customer.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                int Id = GetCustomerId(customer1.BackOfficeUrl);
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo("F:\\IIS_SharedData\\customer_" + Id);
                di.Delete(true);
                DeleteDatabase(customer1.DBName);
                RemoveBinding(customer1.BackOfficeSite, customer1.BackOfficeUrl);
                RemoveBinding(customer1.FrontOfficeSite, customer1.FrontOfficeUrl);
                if (customer.VersionId != 0)
                    customer.VersionNumber = GetVersionNumber(customer.VersionId);
                await _context.SaveChangesAsync();
                string databaseName = customer.DBName;
                String backUpFile = customer.BackupFile;
                String serverName = "DESKTOP-Q03LFQJ\\SQLEXPRESS";
                RestoreDatabase(databaseName, backUpFile, serverName);
                UpdateDataBase(customer.DBName, customer.BackOfficeUrl, GetCustomerId(customer.BackOfficeUrl));
                AddBinding(customer.BackOfficeSite, customer.BackOfficePort, customer.BackOfficeUrl, customer.BackOfficeProtocol);
                AddBinding(customer.FrontOfficeSite, customer.FrontOfficePort, customer.FrontOfficeUrl, customer.FrontOfficeProtocol);
                UpdateCustomerTable(customer1.BackOfficeUrl, customer.VersionId, customer.BackOfficeUrl, customer.FrontOfficeUrl);
                CopyFolder(customer.BackOfficeUrl);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {

            try
            {
                if (customer.VersionId != 0)
                    customer.VersionNumber = GetVersionNumber(customer.VersionId);
                _context.Customer.Add(customer);
                await _context.SaveChangesAsync();
                string databaseName = customer.DBName;
                String backUpFile = customer.BackupFile;
                String serverName = "DESKTOP-Q03LFQJ\\SQLEXPRESS";
                RestoreDatabase(databaseName, backUpFile, serverName);
                UpdateDataBase(customer.DBName, customer.BackOfficeUrl, GetCustomerId(customer.BackOfficeUrl));
                InsertIntoCustomerTable(customer.VersionId, customer.BackOfficeUrl, customer.FrontOfficeUrl);
                CopyFolder(customer.BackOfficeUrl);
                AddBinding(customer.BackOfficeSite, customer.BackOfficePort, customer.BackOfficeUrl, customer.BackOfficeProtocol);
                AddBinding(customer.FrontOfficeSite, customer.FrontOfficePort, customer.FrontOfficeUrl, customer.FrontOfficeProtocol);
            } catch { }

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Customer>> DeleteCustomer(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            try
            {
                int Id = GetCustomerId(customer.BackOfficeUrl);
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo("F:\\IIS_SharedData\\customer_" + Id);
                di.Delete(true);
                DeleteDatabase(customer.DBName);
                RemoveBinding(customer.BackOfficeSite, customer.BackOfficeUrl);
                RemoveBinding(customer.FrontOfficeSite, customer.FrontOfficeUrl);
                DeleteFromCustomerTable(customer.BackOfficeUrl);
                _context.Customer.Remove(customer);
                await _context.SaveChangesAsync();
            } catch { }

            return customer;
        }

        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.Id == id);
        }

        static void RestoreDatabase(String databaseName, String backUpFile, String serverName)
        {
            Restore sqlRestore = new Restore();
            BackupDeviceItem deviceItem = new BackupDeviceItem(backUpFile, DeviceType.File);
            sqlRestore.Devices.Add(deviceItem);
            sqlRestore.Database = databaseName;
            //ServerConnection connection = new ServerConnection(serverName, userName, password);
            Server sqlServer = new Server(serverName);
            sqlRestore.Action = RestoreActionType.Database;

            string logFile = System.IO.Path.GetDirectoryName(backUpFile);
            logFile = System.IO.Path.Combine(logFile, databaseName + "_Log.ldf");

            string dataFile = System.IO.Path.GetDirectoryName(backUpFile);
            dataFile = System.IO.Path.Combine(dataFile, databaseName + ".mdf");

            Database db = sqlServer.Databases[databaseName];
            RelocateFile rf = new RelocateFile(databaseName, dataFile);
            System.Data.DataTable logicalRestoreFiles = sqlRestore.ReadFileList(sqlServer);
            sqlRestore.RelocateFiles.Add(new RelocateFile(logicalRestoreFiles.Rows[0][0].ToString(), dataFile));
            sqlRestore.RelocateFiles.Add(new RelocateFile(logicalRestoreFiles.Rows[1][0].ToString(), logFile));
            sqlRestore.SqlRestore(sqlServer);
            db = sqlServer.Databases[databaseName];
            db.SetOnline();
            sqlServer.Refresh();

        }

        static void AddBinding(String siteName, int port, String hostName, String protocol)
        {
            ServerManager serverManager = new ServerManager();
            ConfigurationElementCollection bindingsCollection = serverManager.Sites[siteName].GetCollection("bindings");
            ConfigurationElement bindingElement = bindingsCollection.CreateElement("binding");
            bindingElement["protocol"] = @protocol;
            bindingElement["bindingInformation"] = @"*:" + port + ":" + hostName;
            bindingsCollection.Add(bindingElement);
            serverManager.CommitChanges();
        }
        static void DeleteDatabase(String databaseName)
        {
            Server sqlServer = new Server("DESKTOP-Q03LFQJ\\SQLEXPRESS");
            Database db = sqlServer.Databases[databaseName];
            db.Refresh();
            db.Drop();
            sqlServer.Refresh();
        }

        static void RemoveBinding(String siteName, String hostName)
        {
            ServerManager serverManager = new ServerManager();
            Microsoft.Web.Administration.Site site = serverManager.Sites[siteName];

            for (int i = 0; i < site.Bindings.Count; i++)
            {
                if (site.Bindings[i].Host == hostName)
                {
                    site.Bindings.RemoveAt(i);
                    break;
                }
            }
            serverManager.CommitChanges();
        }

        static void InsertIntoCustomerTable(int? VersionId, string Name, string Url)
        {
            var connetionString = "Data Source=DESKTOP-Q03LFQJ\\SQLEXPRESS;Initial Catalog=Hrmaps.Master;Integrated Security=True";
            var sql = "INSERT INTO Account.Customer (VersionId, Name, Url) VALUES (" + VersionId + ", '"+Name+"', '"+Url+"'); ";
                using (var connection = new SqlConnection(connetionString))
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
        }

        static void UpdateDataBase(string DBName, string BackOfficeUrl, int customerId)
        {
            var connetionString = "Data Source=DESKTOP-Q03LFQJ\\SQLEXPRESS;Initial Catalog="+ DBName + ";Integrated Security=True";
            var sql1 = "update HR.QualificationType set Query=REPLACE(Query,'R2UFR.Main','"+ DBName + "'); ";
            var sql2 = "update Message.MessageTemplate set Body=REPLACE(Body,'r2ufr','"+ BackOfficeUrl + "'); ";
            var sql3 = "update Security.Company set Website=REPLACE(Website,'r2ufr','"+ BackOfficeUrl + "'),CatalogueWebsite=REPLACE(CatalogueWebsite,'r2ufr','"+ BackOfficeUrl + "'), URL = REPLACE(URL, 'r2ufr', '"+ BackOfficeUrl + "'); ";
            var sql4 = "update Configuration.Setting set Value=REPLACE(Value,'hrmaps.eu.com','hrmaps.cloud') where Value like '%hrmaps.eu.com%'; ";
            var sql5 = "update Configuration.Setting set Value=(select Website from Security.Company) where Name in ('companysettings.CandidatureService','companysettings.JobOfferService'); ";
            var sql6 = "update Configuration.Setting set Value=REPLACE(Value,'r2ufr','"+ BackOfficeUrl + "'); ";
            var sql7 = "update Configuration.Setting set Value="+customerId+" where Name like '%customerid%'; ";
            var sql8 = "update Security.Company set Website=REPLACE(Website,'hrmaps.eu.com','hrmaps.cloud'),CatalogueWebsite=REPLACE(CatalogueWebsite,'hrmaps.eu.com','hrmaps.cloud'), URL = REPLACE(URL, 'hrmaps.eu.com','hrmaps.cloud'); ";


            try
            {
                using (var connection = new SqlConnection(connetionString))
                {
                    using (var command = new SqlCommand(sql1, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    using (var command = new SqlCommand(sql2, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    using (var command = new SqlCommand(sql3, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    using (var command = new SqlCommand(sql4, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    using (var command = new SqlCommand(sql5, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    using (var command = new SqlCommand(sql6, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    using (var command = new SqlCommand(sql7, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    using (var command = new SqlCommand(sql8, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        static void UpdateCustomerTable(string backOfficeUrl, int? VersionId, string Name, string Url)
        {
            var connetionString = "Data Source=DESKTOP-Q03LFQJ\\SQLEXPRESS;Initial Catalog=Hrmaps.Master;Integrated Security=True";
            var sql = "UPDATE Account.Customer SET VersionId = "+VersionId+", Name = '"+Name+"', Url = '"+Url+"' where Name = '"+ backOfficeUrl + "' ;";
                using (var connection = new SqlConnection(connetionString))
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
        }

        static void DeleteFromCustomerTable(string name)
        {
            var connetionString = "Data Source=DESKTOP-Q03LFQJ\\SQLEXPRESS;Initial Catalog=Hrmaps.Master;Integrated Security=True";
            var sql = "DELETE FROM Account.Customer where Name = '" + name + "' ;";
            using (var connection = new SqlConnection(connetionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public string GetVersionNumber(int Id)
        {
            var connetionString = "Data Source=DESKTOP-Q03LFQJ\\SQLEXPRESS;Initial Catalog=Hrmaps.Master;Integrated Security=True";
            var sql = "select VersionNumber from Application.AppVersion where (Id = " + Id + "); ";
            string result = "";
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
                                result = String.Format("{0}", reader["VersionNumber"]);
                            }
                        }
                    }
                }
            }
            catch { }
            return result;
        }

        void CopyFolder(string Name)
        {
            int id = GetCustomerId(Name);
            string path = "customer_"+id;
            string sourceDirectory = @"C:\Users\moham\OneDrive\Bureau\2124_vertex";
            string targetDirectory = "F:\\IIS_SharedData\\" + path;

            Copy(sourceDirectory, targetDirectory);

        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public int GetCustomerId(string Name)
        {
            var connetionString = "Data Source=DESKTOP-Q03LFQJ\\SQLEXPRESS;Initial Catalog=Hrmaps.Master;Integrated Security=True";
            var sql = "select Id from Account.Customer where (Name = '" + Name + "'); ";
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
                                result = (int)reader["Id"];
                            }
                        }
                    }
                }
            }
            catch { }
            return result;
        }
    }
}
