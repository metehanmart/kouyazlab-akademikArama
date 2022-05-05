using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Neo4j.Driver;
using System.Threading.Tasks;
using akademikArama.Models;
namespace akademikArama.Services
{
    public class Neo4jDriverHelper : IDisposable
    {

        private bool _disposed = false;
        private readonly IDriver _driver;

        ~Neo4jDriverHelper() => Dispose(false);

        public Neo4jDriverHelper(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        //public async Task CreateFriendship(string person1Name, string person2Name)
        //{
        //    // To learn more about the Cypher syntax, see https://neo4j.com/docs/cypher-manual/current/
        //    // The Reference Card is also a good resource for keywords https://neo4j.com/docs/cypher-refcard/current/
        //    var query = @"
        //MERGE (p1:Person { name: $person1Name })
        //MERGE (p2:Person { name: $person2Name })
        //MERGE (p1)-[:KNOWS]->(p2)
        //RETURN p1, p2";

        //    var session = _driver.AsyncSession();
        //    try
        //    {
        //        // Write transactions allow the driver to handle retries and transient error
        //        var writeResults = await session.WriteTransactionAsync(async tx =>
        //        {
        //            var result = await tx.RunAsync(query, new { person1Name, person2Name });
        //            return (await result.ToListAsync());
        //        });

        //        foreach (var result in writeResults)
        //        {
        //            var person1 = result["p1"].As<INode>().Properties["name"];
        //            var person2 = result["p2"].As<INode>().Properties["name"];
        //            Console.WriteLine($"Created friendship between: {person1}, {person2}");
        //        }
        //    }
        //    // Capture any errors along with the query and data for traceability
        //    catch (Neo4jException ex)
        //    {
        //        Console.WriteLine($"{query} - {ex}");
        //        throw;
        //    }
        //    finally
        //    {
        //        await session.CloseAsync();
        //    }
        //}

        public List<ArastirmaciModel> FindArastirmaci(string aranacakAd)
        {

            List<ArastirmaciModel> list = new List<ArastirmaciModel>();
            var query = @"
        MATCH (p:ARASTIRMACI)
        WHERE p.ArastirmaciAdi = $ArastirmaciAdi
        RETURN p";
            //using var session = _driver.Session();
            //try
            //{
            //    var readResults = session.ReadTransaction(tx =>
            //    {

            //    });
            //}
            //catch (Exception ex)
            //{

            //}

            var session = _driver.Session();
            try
            {
                var readResults = session.ReadTransaction(tx =>
                {
                    var result = tx.Run(query, new { ArastirmaciAdi = aranacakAd });
                    return (result.ToList());
                });

                foreach (var result in readResults)
                {
                    //System.Diagnostics.Debug.WriteLine($"Found person: {result["p.ArastirmaciAdi"].As<String>()}");
                    ArastirmaciModel tmp = new ArastirmaciModel();

                    ////tmp.ArastirmaciID = result["p.ArastirmaciID"].As<Int32>();
                    //tmp.ArastirmaciID = Convert.ToInt32(result["ArastirmaciID"]);
                    //tmp.ArastirmaciAdi = result["p.ArastirmaciAdi"].As<String>();
                    //tmp.ArasirmaciSoyadi = result["p.ArastirmaciSoyadi"].As<String>();
                    System.Diagnostics.Debug.WriteLine(result[0].ToString() + " hadeee ");
                    System.Diagnostics.Debug.WriteLine(result[1].ToString() + " hadeee ");
                    System.Diagnostics.Debug.WriteLine(result[2].ToString() + " hadeee ");
                    list.Add(tmp);
                }
            }
            // Capture any errors along with the query and data for traceability
            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            //finally
            //{
            //    await session.CloseAsync();
            //}
            return list;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _driver?.Dispose();
            }

            _disposed = true;
        }

        //public static async Task Main(string[] args)
        //{
        //    // Aura queries use an encrypted connection using the "neo4j+s" protocol
        //    var uri = "neo4j+s://<Bolt url for Neo4j Aura database>";

        //    var user = "<Username for Neo4j Aura database>";
        //    var password = "<Password for Neo4j Aura database>";

        //    using (var example = new Neo4jDriverHelper(uri, user, password))
        //    {
        //        //await example.CreateFriendship("Alice", "David");
        //        await example.FindPerson("Alice");
        //    }
        //}
    }
}