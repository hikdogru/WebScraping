using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 100;
            List<string> bkmList = new List<string>();
            List<string> kidegaList = new List<string>();
            List<string> kitap16List = new List<string>();
            List<string> drList = new List<string>();

            List<string> bkmPriceList = new List<string>();
            List<string> kidegaPriceList = new List<string>();
            List<string> kitap16PriceList = new List<string>();
            List<string> drPriceList = new List<string>();

            List<string> bookNames = new List<string>();
            List<string> bookPrices = new List<string>();
            List<string> websitesUrl = new List<string>()
            {
                "https://www.bkmkitap.com/kitap/cok-satan-kitaplar/",
                "https://kidega.com/cok-satan-kitaplar/",
                "https://www.kitap16.com/cok-satanlar-kitapligi",
                "https://www.dr.com.tr/CokSatanlar/Kitap/",
            };

            List<int> bkmDups = new List<int>();
            List<int> kidegaDups = new List<int>();
            List<int> drDups = new List<int>();
            List<int> kitap16Dups = new List<int>();

            int s = 0;
            foreach (var url in websitesUrl)
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);

                if (url.Contains("bkm"))
                {


                    foreach (var selectNode in doc.DocumentNode.SelectNodes("//a[@class='fl col-12 text-description detailLink']"))
                    {
                        s++;
                        if (bkmList.Contains(selectNode.InnerText))
                        {
                            var foundIndex = bkmList
                                    .Select((v, i) => new { Index = i, Value = v })
                                    .Where(x => x.Value == selectNode.InnerText)
                                    .Select(x => x.Index)
                                ;

                            bkmDups.AddRange(foundIndex);


                        }
                        else if (!bkmList.Contains(selectNode.InnerText))
                        {
                            bkmList.Add(selectNode.InnerText);

                        }

                        if (s == n)
                        {
                            break;
                        }


                    }

                    s = 0;
                    foreach (var selectNode in doc.DocumentNode.SelectNodes("//div[@class='col col-12 currentPrice']"))
                    {
                        s++;
                        bkmPriceList.Add(selectNode.InnerText);
                        if (s == n)
                        {
                            break;
                        }
                    }

                    foreach (var i in bkmDups)
                    {
                        bkmPriceList.RemoveAt(i);
                    }

                    s = 0;
                }
                else if (url.Contains("kidega"))
                {

                    foreach (var selectNode in doc.DocumentNode.SelectNodes("//a[@class='book-name']"))
                    {
                        s++;
                        if (kidegaList.Contains(selectNode.InnerText))
                        {
                            var foundIndex = kidegaList
                                .Select((v, i) => new { Index = i, Value = v })
                                .Where(x => x.Value == selectNode.InnerText)
                                .Select(x => x.Index)
                                ;

                            kidegaDups.AddRange(foundIndex);


                        }
                        else if (!kidegaList.Contains(selectNode.InnerText))
                        {
                            kidegaList.Add(selectNode.InnerText);

                        }
                        if (s == n)
                        {
                            break;
                        }

                    }

                    s = 0;
                    foreach (var selectNode in doc.DocumentNode.SelectNodes("//b[@class='lastPrice']"))
                    {
                        s++;
                        kidegaPriceList.Add(selectNode.InnerText);
                        if (s == n)
                        {
                            break;
                        }
                    }

                    foreach (var i in kidegaDups)
                    {
                        kidegaPriceList.RemoveAt(i-1);

                    }

                    s = 0;
                }
                else if (url.Contains("kitap16"))
                {
                    foreach (var selectNode in doc.DocumentNode.SelectNodes("//div[@class='name']"))
                    {
                        s++;
                        if (kitap16List.Contains(selectNode.InnerText))
                        {
                            var foundIndex = kitap16List
                                    .Select((v, i) => new { Index = i, Value = v })
                                    .Where(x => x.Value == selectNode.InnerText)
                                    .Select(x => x.Index)
                                ;

                            kitap16Dups.AddRange(foundIndex);


                        }
                        else if (!kitap16List.Contains(selectNode.InnerText))
                        {
                            kitap16List.Add(selectNode.InnerText);

                        }
                        if (s == n)
                        {
                            break;
                        }
                    }

                    s = 0;
                    foreach (var selectNode in doc.DocumentNode.SelectNodes("//span[@class='price price_sale convert_cur']"))
                    {
                        s++;
                        kitap16PriceList.Add(selectNode.InnerText);
                        if (s == n)
                        {
                            break;
                        }
                    }


                    foreach (var i in kitap16Dups)
                    {
                        kitap16PriceList.RemoveAt(i-1);
                    }
                    s = 0;
                }
                else if (url.Contains("dr"))
                {
                    foreach (var selectNode in doc.DocumentNode.SelectNodes("//h3[@class='ellipsis']"))
                    {
                        s++;
                        if (drList.Contains(selectNode.InnerText))
                        {
                            var foundIndex = drList
                                    .Select((v, i) => new { Index = i, Value = v })
                                    .Where(x => x.Value == selectNode.InnerText)
                                    .Select(x => x.Index)
                                ;

                            drDups.AddRange(foundIndex);


                        }
                        else if (!drList.Contains(selectNode.InnerText))
                        {
                            drList.Add(selectNode.InnerText);

                        }

                        if (s == n)
                        {
                            break;
                        }
                    }

                    s = 0;
                    foreach (var selectNode in doc.DocumentNode.SelectNodes("//span[@class='price']"))
                    {
                        s++;
                        drPriceList.Add(selectNode.InnerText);
                        if (s == n)
                        {
                            break;
                        }
                    }
                    foreach (var i in drDups)
                    {
                        drPriceList.RemoveAt(i);
                    }
                    s = 0;
                }
            }

            bookNames.AddRange(bkmList);
            bookNames.AddRange(kidegaList);
            bookNames.AddRange(kitap16List);
            bookNames.AddRange(drList);

            bookPrices.AddRange(bkmPriceList);
            bookPrices.AddRange(kidegaPriceList);
            bookPrices.AddRange(kitap16PriceList);
            bookPrices.AddRange(drPriceList);

            for (int i = 0; i < bookPrices.Count; i++)
            {
                bookNames[i] = bookNames[i].Replace("\n", "");
                bookPrices[i] = bookPrices[i].Replace("\n", "").Replace("₺", "TL");
            }

            for (int i = 0; i < bkmPriceList.Count; i++)
            {
                bkmPriceList[i] = bkmPriceList[i].Replace("\n", "").Replace("₺", "TL"); ;
                bkmList[i] = bkmList[i].Replace("\n", "");
            }
            for (int i = 0; i < kidegaPriceList.Count; i++)
            {
                kidegaPriceList[i] = kidegaPriceList[i].Replace("₺", "TL");

            }
            for (int i = 0; i < kitap16PriceList.Count; i++)
            {
                kitap16PriceList[i] = kitap16PriceList[i].Replace("\n", "").Replace("₺", "TL"); ;
            }
            for (int i = 0; i < drPriceList.Count; i++)
            {
                drPriceList[i] = drPriceList[i].Replace("\n", "").Replace("₺", "TL"); ;
            }

            //Console.WriteLine($"                        Website : {"Bkm Kitap" + "  " + "Kidega"}");
            var duplicates = bookNames.GroupBy(b => b)
                .SelectMany(grp => grp.Skip(3));
            foreach (var item in duplicates)
            {

                var bkmIndice = bkmList.Select((value, index) => new { value, index })
                    .Where(a => string.Equals(a.value, item))
                    .Select(a => a.index).ToList();

                var kidegaIndice = kidegaList.Select((value, index) => new { value, index })
                    .Where(a => string.Equals(a.value, item))
                    .Select(a => a.index).ToList();

                var kitap16Indice = kitap16List.Select((value, index) => new { value, index })
                    .Where(a => string.Equals(a.value, item))
                    .Select(a => a.index).ToList();

                var drIndice = drList.Select((value, index) => new { value, index })
                    .Where(a => string.Equals(a.value, item))
                    .Select(a => a.index).ToList();


                Console.WriteLine($"               Book name:{item}\n");
                Console.WriteLine($" BKM           {bkmPriceList[bkmIndice[0] - bkmDups.Count]}");
                Console.WriteLine($" Kidega        {kidegaPriceList[kidegaIndice[0]-1]}");
                Console.WriteLine($" Kitap16       {kitap16PriceList[kitap16Indice[0] - kitap16Dups.Count]}");
                Console.WriteLine($" D&R           {drPriceList[drIndice[0] - drDups.Count]}");
                Console.WriteLine();



            }


            Console.ReadKey();

            Console.WriteLine();

        }

    }


}
