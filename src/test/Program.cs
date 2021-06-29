using System;
using System.Collections.Generic;
using System.Data;

using Newtonsoft.Json;

using libaccumula;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            TestAdd_BigqueryServel();
            TestJsonToDataTable();
        }

        static void TestJsonToDataTable()
        {
            Console.WriteLine("** TestJsonToDataTable **");
            string requestBody = @"{
                'schema': 'BigQuery',
                'table': 'Servel',
                'data': [
                    {
                        'CreatedFromProcess': 'TestJsonToDataTable()',
                        'CreatedByQuery': '12.432.234-2',
                        'domicilio_electoral': 'Las Pailas 432',
                        'dv': '2',
                        'glosa_comuna': 'Las Condes',
                        'nombre': 'Jorge Sutil',
                        'rut': '12432234',
                        'sexo': 0
                    },
                    {
                        'CreatedFromProcess': 'TestJsonToDataTable()',
                        'CreatedByQuery': '12.432.234-2',
                        'domicilio_electoral': 'Las Pailas 432',
                        'dv': '2',
                        'glosa_comuna': 'Las Condes',
                        'nombre': 'Jorge Sutil',
                        'rut': '12432234',
                        'sexo': 0
                    }
                ]
            }".Replace("'", "\"");            
            //dynamic data = JsonSerializer.Deserialize(requestBody);
            var table = JsonConvert.DeserializeObject<TableRows>(requestBody); 
            Console.WriteLine($"{table.schema}.{table.table} has {table.data.Rows.Count} rows.");

            var db = new AccumulaDB();
            db.AddRows(table).Wait();    

            var json = JsonConvert.SerializeObject(table);
            Console.WriteLine(json);
        }
            

        static void TestAdd_BigqueryServel()
        {
            Console.WriteLine("** TestAdd Bigquery.Servel **");
            var db = new AccumulaDB();
            
            var table = new TableRows() 
            {
                schema = "BigQuery",
                table = "Servel"
            };
            table.data = new DataTable();
            table.data.Columns.Add("CreatedByUser");
            table.data.Columns.Add("CreatedFromProcess");
            table.data.Columns.Add("CreatedByQuery");
            table.data.Columns.Add("domicilio_electoral");
            table.data.Columns.Add("dv");
            table.data.Columns.Add("glosa_comuna");
            table.data.Columns.Add("nombre");
            table.data.Columns.Add("rut", typeof(int));
            table.data.Columns.Add("sexo", typeof(int));

            table.data.Rows.Add( new object[] { "dkottow@gmail.com", "Test", "10.562.459-K", "Paris 712H", "k", "Santiago", "Daniel", 10562459, 1  } );
            table.data.Rows.Add( new object[] { "dkottow@gmail.com", "Test", "10.562.458-1", "Portugal 1500", "1", "Santiago", "Andrea", 10562458, 0 } );

            Console.WriteLine($"{table.schema}.{table.table} has {table.data.Rows.Count} rows.");

            var json = JsonConvert.SerializeObject(table);
            Console.WriteLine(json);

            db.AddRows(table).Wait();
        }

    }
}
