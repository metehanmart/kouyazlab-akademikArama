using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Neo4j.Driver;
using System.Threading.Tasks;
using akademikArama.Models;
using System.Text.RegularExpressions;

namespace akademikArama.Services
{
    public class Neo4jDriverHelper : IDisposable
    {
        //selam as
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

        public List<EklemeSayfasiModel> HepsiniGetir()
        {
            List<EklemeSayfasiModel> list = new List<EklemeSayfasiModel>();
            string query = "MATCH (n:ARASTIRMACI) return n";
            var session = _driver.Session();
            try
            {
                var readResults = session.ReadTransaction(tx =>
                {
                    var result = tx.Run(query);
                    return (result.ToList());
                });

                foreach (var result in readResults)
                {
                    EklemeSayfasiModel tmp = new EklemeSayfasiModel();
                    var Node = result["n"].As<INode>();

                    tmp.ArastirmaciID = Node["ArastirmaciID"].As<Int32>();
                    tmp.ArastirmaciAdi = Node["ArastirmaciAdi"].As<String>();
                    tmp.ArasirmaciSoyadi = Node["ArastirmaciSoyadi"].As<String>();

                    list.Add(tmp);
                }
            }
            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            return list;
        }

        public List<EklemeSayfasiModel> EserleriGetir()
        {
            List<EklemeSayfasiModel> list = new List<EklemeSayfasiModel>();

            string query = $"MATCH(y:YAYIN)-->(t:YAYINTURU) RETURN y,t;";
            System.Diagnostics.Debug.WriteLine(query);
            var session = _driver.Session();
            try
            {
                var readResults = session.ReadTransaction(tx =>
                {
                    var result = tx.Run(query);
                    return (result.ToList());
                });
                foreach (var result in readResults)
                {
                    EklemeSayfasiModel tmp = new EklemeSayfasiModel();
                    var node1 = result["y"].As<INode>();
                    var node2 = result["t"].As<INode>();

                    tmp.YayinAdi = node1["YayinAdi"].As<String>();
                    tmp.YayinYili = node1["YayinYili"].As<Int32>();
                    tmp.YayinID = node1["YayinID"].As<Int32>();
                    tmp.YayinYeri = node2["YayinYeri"].As<String>();
                    tmp.YayinTuru = node2["YayinTuru"].As<String>();
                    tmp.YayinTuruID = node2["YayinTuruID"].As<Int32>();
                    //System.Diagnostics.Debug.WriteLine("hadee " + tmp.YayinYeri);
                    list.Add(tmp);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("hata ====" + ex);
            }


            return list;
        }

        public List<AramaSayfasiModel> FindArastirmaci(string aranacakAd, string aranacakSoyad, string yayinAdi, int? yayinYili)
        {

            List<AramaSayfasiModel> list = new List<AramaSayfasiModel>();
            List<AramaSayfasiModel> listTmp = new List<AramaSayfasiModel>();
            List<AramaSayfasiModel> listTmp2 = new List<AramaSayfasiModel>();
            var session = _driver.Session();
            var query = "";
            bool eseriVarMi = false;
            // hiç eseri var mı yo mu kontrol
            query = $"MATCH(a:ARASTIRMACI),(y:YAYIN) WHERE a.ArastirmaciAdi CONTAINS '{aranacakAd}' AND a.ArastirmaciSoyadi CONTAINS '{aranacakSoyad}' AND (a)-[:ORTAKPROJE]->(y) RETURN a";
            try
            {
                var readResults = session.ReadTransaction(tx =>
                {
                    var result = tx.Run(query, new { ArastirmaciAdi = aranacakAd });
                    return (result.ToList());
                });

                foreach (var result in readResults)
                {
                    AramaSayfasiModel tmp = new AramaSayfasiModel();
                    var Node = result["a"].As<INode>();

                    tmp.ArastirmaciID = Node["ArastirmaciID"].As<Int32>();
                    tmp.ArastirmaciAdi = Node["ArastirmaciAdi"].As<String>();
                    tmp.ArasirmaciSoyadi = Node["ArastirmaciSoyadi"].As<String>();

                    listTmp.Add(tmp);
                }
            }
            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            if (listTmp.Count > 0)
                eseriVarMi = true;

            //Ortağı var mı?
            if (yayinYili == 0 || yayinYili == null)
            {
                query = $"MATCH(a:ARASTIRMACI),(y:YAYIN),(t:YAYINTURU) WHERE a.ArastirmaciAdi CONTAINS '{aranacakAd}' AND a.ArastirmaciSoyadi CONTAINS '{aranacakSoyad}' " +
                    $"AND y.YayinAdi CONTAINS '{yayinAdi}' AND (a)-[:YAYINLADI]->(y) AND (y)-[:TURU]->(t) AND NOT (a)-[:ORTAKPROJE]->()  RETURN a,y,t;";
            }
            else
            {
                query = $"MATCH(a:ARASTIRMACI),(y:YAYIN),(t:YAYINTURU) WHERE a.ArastirmaciAdi CONTAINS '{aranacakAd}' AND a.ArastirmaciSoyadi CONTAINS '{aranacakSoyad}' " +
                    $"AND y.YayinAdi CONTAINS '{yayinAdi}' AND y.YayinYili = {yayinYili} AND(a)-[:YAYINLADI]->(y) AND (y)-[:TURU]->(t) AND NOT (a)-[:ORTAKPROJE]->()  RETURN a,y,t;";
            }
            try
            {
                var readResults = session.ReadTransaction(tx =>
                {
                    var result = tx.Run(query, new { ArastirmaciAdi = aranacakAd });
                    return (result.ToList());
                });

                foreach (var result in readResults)
                {
                    AramaSayfasiModel tmp = new AramaSayfasiModel();
                    var Node = result["a"].As<INode>();
                    var Node2 = result["y"].As<INode>();
                    var Node4 = result["t"].As<INode>();
                    tmp.ArastirmaciID = Node["ArastirmaciID"].As<Int32>();
                    tmp.ArastirmaciAdi = Node["ArastirmaciAdi"].As<String>();
                    tmp.ArasirmaciSoyadi = Node["ArastirmaciSoyadi"].As<String>();
                    tmp.YayinAdi = Node2["YayinAdi"].As<String>();
                    tmp.YayinYili = Node2["YayinYili"].As<Int32>();
                    tmp.YayinTuru = Node4["YayinTuru"].As<String>();
                    list.Add(tmp);
                }
            }
            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }




            if (yayinYili == 0 || yayinYili == null)
            {
                query = $"MATCH(a:ARASTIRMACI),(y:YAYIN),(t:YAYINTURU),(b:ARASTIRMACI) WHERE a.ArastirmaciAdi CONTAINS '{aranacakAd}' AND a.ArastirmaciSoyadi CONTAINS '{aranacakSoyad}' AND " +
                    $"y.YayinAdi CONTAINS '{yayinAdi}' AND (a)-[:YAYINLADI]->(y) AND (y)-[:TURU]->(t) AND (a)-[:ORTAKPROJE]->(b)-[:YAYINLADI]->(y)  RETURN a,y,t,b;";
            }
            else
            {
                query = $"MATCH(a:ARASTIRMACI),(y:YAYIN),(t:YAYINTURU),(b:ARASTIRMACI) WHERE a.ArastirmaciAdi CONTAINS '{aranacakAd}' AND a.ArastirmaciSoyadi CONTAINS '{aranacakSoyad}' AND " +
                    $"y.YayinAdi CONTAINS '{yayinAdi}' AND y.YayinYili = {yayinYili} AND (a)-[:YAYINLADI]->(y) AND (y)-[:TURU]->(t) AND (a)-[:ORTAKPROJE]->(b)-[:YAYINLADI]->(y)  RETURN a,y,t,b;";
            }

            System.Diagnostics.Debug.WriteLine(query);
            try
            {
                var readResults = session.ReadTransaction(tx =>
                {
                    var result = tx.Run(query, new { ArastirmaciAdi = aranacakAd });
                    return (result.ToList());
                });

                foreach (var result in readResults)
                {
                    AramaSayfasiModel tmp = new AramaSayfasiModel();
                    var Node = result["a"].As<INode>();
                    var Node2 = result["y"].As<INode>();
                    var Node3 = result["b"].As<INode>();
                    var Node4 = result["t"].As<INode>();
                    tmp.ArastirmaciID = Node["ArastirmaciID"].As<Int32>();
                    tmp.ArastirmaciAdi = Node["ArastirmaciAdi"].As<String>();
                    tmp.ArasirmaciSoyadi = Node["ArastirmaciSoyadi"].As<String>();
                    tmp.YayinAdi = Node2["YayinAdi"].As<String>();
                    tmp.YayinYili = Node2["YayinYili"].As<Int32>();
                    tmp.YayinTuru = Node4["YayinTuru"].As<String>();
                    tmp.CalistigiKisiler = Node3["ArastirmaciAdi"].As<String>() + " " + Node3["ArastirmaciSoyadi"].As<String>();
                    list.Add(tmp);
                }
            }
            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            if (list.Count == 0 && eseriVarMi == false)
            {
                query = $"MATCH(a:ARASTIRMACI) WHERE a.ArastirmaciAdi CONTAINS '{aranacakAd}' AND a.ArastirmaciSoyadi CONTAINS '{aranacakSoyad}' RETURN a";
                try
                {
                    var readResults = session.ReadTransaction(tx =>
                    {
                        var result = tx.Run(query, new { ArastirmaciAdi = aranacakAd });
                        return (result.ToList());
                    });

                    foreach (var result in readResults)
                    {
                        AramaSayfasiModel tmp = new AramaSayfasiModel();
                        var Node = result["a"].As<INode>();

                        tmp.ArastirmaciID = Node["ArastirmaciID"].As<Int32>();
                        tmp.ArastirmaciAdi = Node["ArastirmaciAdi"].As<String>();
                        tmp.ArasirmaciSoyadi = Node["ArastirmaciSoyadi"].As<String>();

                        list.Add(tmp);
                    }
                }
                catch (Neo4jException ex)
                {
                    Console.WriteLine($"{query} - {ex}");
                    throw;
                }

            }


            return list;
        }

        public bool ArastirmaciEkleme(EklemeSayfasiModel eklemeSayfasiModel)
        {
            bool status = true;
            //string nodeName = eklemeSayfasiModel.ArastirmaciAdi + eklemeSayfasiModel.ArasirmaciSoyadi;
            string query = $"CREATE(a:ARASTIRMACI{{ArastirmaciID:{eklemeSayfasiModel.ArastirmaciID},ArastirmaciAdi:'{eklemeSayfasiModel.ArastirmaciAdi}',ArastirmaciSoyadi:'{eklemeSayfasiModel.ArasirmaciSoyadi}'}}) RETURN a";
            System.Diagnostics.Debug.WriteLine("query = " + query);
            List<EklemeSayfasiModel> list = new List<EklemeSayfasiModel>();
            using (var session = _driver.Session())
            {
                try
                {
                    var ekle = session.WriteTransaction(tx =>
                    {
                        var result = tx.Run(query, new { eklemeSayfasiModel.ArastirmaciAdi });
                        return result;
                    });
                }
                catch (Exception ex)
                {
                    status = false;
                    System.Diagnostics.Debug.WriteLine("hata" + ex);
                }

            }
            return status;
        }
        public void ArastirmaciSil(EklemeSayfasiModel eklemeSayfasiModel)
        {
            //Mete Query Yaz
            System.Diagnostics.Debug.WriteLine("Gelen Arastırmacı ID: " + eklemeSayfasiModel.ArastirmaciID);
            string query = $"MATCH(a:ARASTIRMACI) WHERE a.ArastirmaciID = {eklemeSayfasiModel.ArastirmaciID}  DETACH DELETE a";
            using (var session = _driver.Session())
            {
                try
                {
                    var ekle = session.WriteTransaction(tx =>
                    {
                        var result = tx.Run(query, new { eklemeSayfasiModel.ArastirmaciAdi });
                        return result;
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("hata" + ex);
                }

            }
        }
        public void YayinSil(EklemeSayfasiModel eklemeSayfasiModel)
        {
            //Mete Query Yaz
            System.Diagnostics.Debug.WriteLine("Gelen Yayın ID: " + eklemeSayfasiModel.YayinID);
            string query = $"MATCH(y:YAYIN) WHERE y.YayinID = {eklemeSayfasiModel.YayinID}  DETACH DELETE y";
            using (var session = _driver.Session())
            {
                try
                {
                    var ekle = session.WriteTransaction(tx =>
                    {
                        var result = tx.Run(query, new { eklemeSayfasiModel.ArastirmaciAdi });
                        return result;
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("hata" + ex);
                }

            }
        }
        public void YayinTuruSil(EklemeSayfasiModel eklemeSayfasiModel)
        {
            //Mete Query Yaz
            string query = $"MATCH(t:YAYINTURU) WHERE t.YayinTuruID = {eklemeSayfasiModel.YayinTuruID}  DETACH DELETE t";
            using (var session = _driver.Session())
            {
                try
                {
                    var ekle = session.WriteTransaction(tx =>
                    {
                        var result = tx.Run(query, new { eklemeSayfasiModel.ArastirmaciAdi });
                        return result;
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("hata" + ex);
                }

            }
            System.Diagnostics.Debug.WriteLine("Gelen Yayın Turu ID: " + eklemeSayfasiModel.YayinTuruID);
        }
        public void EserleArastirmaciyiBagla(EklemeSayfasiModel eklemeSayfasiModel)
        {
            // Burda turude baglamak lazım
            /**
             *  Ilk once Arastırmaci var mı kontrol yoksa arastırmacı olussun sonra eser var mı kontrol yoksa
             *  eser oluşsun sonra eserle arastirmaciyi baglama en son o esere kendisinden baska kim katkı yaptıysa
             *  o kisi ile ortak işte çalışma bağı
             */
            string query = $"MATCH (n:ARASTIRMACI{{ArastirmaciID:{eklemeSayfasiModel.ArastirmaciID}}}) RETURN n";
            var session = _driver.Session();
            var readResults = session.ReadTransaction(tx =>
            {
                var result = tx.Run(query);
                return (result.ToList());
            });
            // arastirmaci yoksa
            if (readResults.Count == 0)
            {
                // eser var mi
                query = $"MATCH(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID}}}) RETURN y";
                var readResult2 = session.ReadTransaction(tx =>
                {
                    var result = tx.Run(query);
                    return (result.ToList());
                });
                //eser yok
                if (readResult2.Count == 0)
                {
                    query = $"MATCH(t:YAYINTURU{{YayinTuruID:{eklemeSayfasiModel.YayinTuruID}}}) RETURN t";
                    var readResult3 = session.ReadTransaction(tx =>
                    {
                        var result = tx.Run(query);
                        return (result.ToList());
                    });
                    //tür yok hepsini oluştur bagla
                    if (readResult3.Count() == 0)
                    {
                        string nodeName = eklemeSayfasiModel.ArastirmaciAdi + eklemeSayfasiModel.ArasirmaciSoyadi;
                        query = $"CREATE({nodeName}:ARASTIRMACI{{ArastirmaciID:{eklemeSayfasiModel.ArastirmaciID},ArastirmaciAdi:'{eklemeSayfasiModel.ArastirmaciAdi}',ArastirmaciSoyadi:'{eklemeSayfasiModel.ArasirmaciSoyadi}'}}) RETURN {nodeName}";
                        var ekle = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });

                        query = $"CREATE(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID},YayinAdi:'{eklemeSayfasiModel.YayinAdi}',YayinYili:{eklemeSayfasiModel.YayinYili}}}) RETURN y";

                        var ekle2 = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });

                        query = $"CREATE(t:YAYINTURU{{YayinTuruID:{eklemeSayfasiModel.YayinTuruID},YayinTuru:'{eklemeSayfasiModel.YayinTuru}',YayinYeri:'{eklemeSayfasiModel.YayinYeri}'}}) RETURN t";

                        var ekle3 = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });

                        query = $"MATCH(a:ARASTIRMACI{{ArastirmaciID:{eklemeSayfasiModel.ArastirmaciID}}}),(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID}}}) CREATE (a)-[:YAYINLADI]->(y)";
                        System.Diagnostics.Debug.WriteLine("query ==  " + query);
                        var birlestir = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });

                        query = $"MATCH(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID}}}),(t:YAYINTURU{{YayinTuruID:{eklemeSayfasiModel.YayinTuruID}}}) CREATE (y)-[:TURU]->(t)";
                        System.Diagnostics.Debug.WriteLine("query ==  " + query);
                        var birlestir2 = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });
                    }
                    //tür var arastirmaci ve eser yok
                    else
                    {
                        string nodeName = eklemeSayfasiModel.ArastirmaciAdi + eklemeSayfasiModel.ArasirmaciSoyadi;
                        query = $"CREATE({nodeName}:ARASTIRMACI{{ArastirmaciID:{eklemeSayfasiModel.ArastirmaciID},ArastirmaciAdi:'{eklemeSayfasiModel.ArastirmaciAdi}',ArastirmaciSoyadi:'{eklemeSayfasiModel.ArasirmaciSoyadi}'}}) RETURN {nodeName}";
                        var ekle = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });

                        query = $"CREATE(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID},YayinAdi:'{eklemeSayfasiModel.YayinAdi}',YayinYili:{eklemeSayfasiModel.YayinYili}}}) RETURN y";

                        var ekle2 = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });

                        query = $"MATCH(a:ARASTIRMACI{{ArastirmaciID:{eklemeSayfasiModel.ArastirmaciID}}}),(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID}}}) CREATE (a)-[:YAYINLADI]->(y)";
                        System.Diagnostics.Debug.WriteLine("query ==  " + query);
                        var birlestir = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });

                        query = $"MATCH(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID}}}),(t:YAYINTURU{{YayinTuruID:{eklemeSayfasiModel.YayinTuruID}}}) CREATE (y)-[:TURU]->(t)";
                        System.Diagnostics.Debug.WriteLine("query ==  " + query);
                        var birlestir2 = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });
                    }
                }
                //arastirmaci yok eser var eser varsa ortak çalışan olacak burda
                else
                {

                    string nodeName = eklemeSayfasiModel.ArastirmaciAdi + eklemeSayfasiModel.ArasirmaciSoyadi;
                    query = $"CREATE({nodeName}:ARASTIRMACI{{ArastirmaciID:{eklemeSayfasiModel.ArastirmaciID},ArastirmaciAdi:'{eklemeSayfasiModel.ArastirmaciAdi}',ArastirmaciSoyadi:'{eklemeSayfasiModel.ArasirmaciSoyadi}'}}) RETURN {nodeName}";
                    var ekle = session.WriteTransaction(tx =>
                    {
                        var result = tx.Run(query);
                        return result;
                    });
                    query = $"MATCH(a:ARASTIRMACI{{ArastirmaciID:{eklemeSayfasiModel.ArastirmaciID}}}),(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID}}}) CREATE (a)-[:YAYINLADI]->(y)";
                    System.Diagnostics.Debug.WriteLine("query ==  " + query);
                    var birlestir = session.WriteTransaction(tx =>
                    {
                        var result = tx.Run(query);
                        return result;
                    });

                    query = $"MATCH(a:ARASTIRMACI{{ArastirmaciID:{eklemeSayfasiModel.ArastirmaciID}}}),(b:ARASTIRMACI),(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID}}}) WHERE b.ArastirmaciID <>{eklemeSayfasiModel.ArastirmaciID} and (b)-[:YAYINLADI]->(y) CREATE (a)-[:ORTAKPROJE]->(b),(b)-[:ORTAKPROJE]->(a)";
                    var birlestir3 = session.WriteTransaction(tx =>
                    {
                        var result = tx.Run(query);
                        return result;
                    });

                }
            }
            //arastirmaci varsa
            else
            {
                query = $"MATCH(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID}}}) RETURN y";
                var readResult2 = session.ReadTransaction(tx =>
                {
                    var result = tx.Run(query);
                    return (result.ToList());
                });
                //arastırmacı var yayın yok
                if (readResult2.Count() == 0)
                {
                    query = $"MATCH(t:YAYINTURU{{YayinTuruID:{eklemeSayfasiModel.YayinTuruID}}}) RETURN t";
                    var readResult3 = session.ReadTransaction(tx =>
                    {
                        var result = tx.Run(query);
                        return (result.ToList());
                    });
                    //tür yok arastırmaci var yayın yok
                    if (readResult3.Count() == 0)
                    {
                        query = $"CREATE(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID},YayinAdi:'{eklemeSayfasiModel.YayinAdi}',YayinYili:{eklemeSayfasiModel.YayinYili}}}) RETURN y";

                        var ekle2 = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });

                        query = $"CREATE(t:YAYINTURU{{YayinTuruID:{eklemeSayfasiModel.YayinTuruID},YayinTuru:'{eklemeSayfasiModel.YayinTuru}',YayinYeri:'{eklemeSayfasiModel.YayinYeri}'}}) RETURN t";

                        var ekle3 = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });

                        query = $"MATCH(a:ARASTIRMACI{{ArastirmaciID:{eklemeSayfasiModel.ArastirmaciID}}}),(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID}}}) CREATE (a)-[:YAYINLADI]->(y)";
                        System.Diagnostics.Debug.WriteLine("query ==  " + query);
                        var birlestir = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });

                        query = $"MATCH(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID}}}),(t:YAYINTURU{{YayinTuruID:{eklemeSayfasiModel.YayinTuruID}}}) CREATE (y)-[:TURU]->(t)";
                        System.Diagnostics.Debug.WriteLine("query ==  " + query);
                        var birlestir2 = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run(query);
                            return result;
                        });
                    }
                }
                //arastırmcai var yayın var
                else
                {
                    query = $"MATCH(a:ARASTIRMACI{{ArastirmaciID:{eklemeSayfasiModel.ArastirmaciID}}}),(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID}}}) CREATE (a)-[:YAYINLADI]->(y)";
                    System.Diagnostics.Debug.WriteLine("query ==  " + query);
                    var birlestir = session.WriteTransaction(tx =>
                    {
                        var result = tx.Run(query);
                        return result;
                    });

                    // yayın varsa türsüz olamaz
                    // o yayın olduğu takdirde türü kontrol etmeye gerek yok
                    query = $"MATCH(a:ARASTIRMACI{{ArastirmaciID:{eklemeSayfasiModel.ArastirmaciID}}}),(b:ARASTIRMACI),(y:YAYIN{{YayinID:{eklemeSayfasiModel.YayinID}}}) WHERE b.ArastirmaciID <>{eklemeSayfasiModel.ArastirmaciID} and (b)-[:YAYINLADI]->(y) CREATE (a)-[:ORTAKPROJE]->(b),(b)-[:ORTAKPROJE]->(a)";
                    var birlestir3 = session.WriteTransaction(tx =>
                    {
                        var result = tx.Run(query);
                        return result;
                    });
                }
            }
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
    }
}