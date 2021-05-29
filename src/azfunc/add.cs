using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using System.Data.SqlClient;
using libaccumula;

namespace accumula_sottaycia
{
    public static class add
    {
        [FunctionName("add")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
//            , ILogger log)
        {
            //log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);

            try 
            {
                var rows = JsonConvert.DeserializeObject<TableRows>(requestBody); 
                //Console.WriteLine($"{rows.schema}.{rows.table} has {rows.data.Rows.Count} rows.");

                var db = new AccumulaDB();         
                await db.AddRows(rows);
                return new OkObjectResult($"OK. Added {rows.data.Rows.Count} rows to {rows.schema}.{rows.table}"); 
            }
            catch(Exception ex)
            {
                var rsp = new ObjectResult(ex);    
                rsp.StatusCode = 500;
                return rsp;
            }
        }

        static async Task addRows(dynamic data)
        {
            try 
            { 
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = "almacenes-sottaycia.database.windows.net"; 
                builder.UserID = "admin_almacenes";            
                builder.Password = "c3b!7jFnw9RjUy4-";     
                builder.InitialCatalog = "accumula";
         
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    await connection.OpenAsync();       

                    String sql = "SELECT * FROM BigQuery.Servel";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0} {1}", reader["Id"], reader["nombre"]);
                            }
                        }
                    }                    
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }            
        }
    }
}
