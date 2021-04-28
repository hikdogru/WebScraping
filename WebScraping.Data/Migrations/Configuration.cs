using System.Collections.Generic;
using WebScraping.Data.Concrete.Ef;
using WebScraping.Entities;

namespace WebScraping.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WebScraping.Data.Concrete.Ef.WebScrapingContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebScrapingContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
           

            List<Website> websites = new List<Website>()
            {
                new Website(){Name="Bkm Kitap", LogoUrl= "http://www.bkmkitap.com/Data/EditorFiles/logonew23.png", WebsiteUrl = "http://www.bkmkitap.com"},
                new Website(){Name="Amazon", LogoUrl= "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a9/Amazon_logo.svg/1920px-Amazon_logo.svg.png",WebsiteUrl = "https://www.amazon.com.tr"},
                new Website(){Name="Kidega", LogoUrl= "https://cdn.kidega.com/assets/web/img/kidega-logo.png", WebsiteUrl = "https://www.kidega.com"},
                new Website(){Name="Kitap16", LogoUrl= "https://www.kitap16.com/u/kitap16/kitap-transparan-1579509339.png", WebsiteUrl = "https://www.kitap16.com"} ,
                new Website(){Name="D&R", LogoUrl= "https://www.dr.com.tr/Themes/DR/Content/assets/images/general/head-logo.png",WebsiteUrl = "https://www.dr.com.tr"},
                new Website(){Name="İlknokta", LogoUrl= "https://www.ilknokta.com/u/ilknokta/ilknokta-logosu-1613392480.jpg", WebsiteUrl = "https://www.ilknokta.com"},
                new Website(){Name="Eganba", LogoUrl= "https://www.eganba.com/wwwroot/images/eganba-logo.png", WebsiteUrl = "https://www.eganba.com"},
                new Website(){Name="Kitapseç", LogoUrl= "https://cdn.kitapsec.com//temalar/KitapSec2017/img/logo.jpg", WebsiteUrl = "https://www.kitapsec.com"},
                new Website(){Name="Idefix", LogoUrl= "https://fragtist.com/wp-content/uploads/2017/04/fragtist-IDEFIX-750x349.gif" , WebsiteUrl = "https://www.idefix.com"},
                new Website(){Name="Fidan Kitap", LogoUrl= "https://www.fidankitap.com/u/fidankitap/fidan-kitap-logo-9-1576766279.png", WebsiteUrl = "https://www.fidankitap.com"},
                new Website(){Name="Hepsiburada", LogoUrl= "https://cdn.freelogovectors.net/wp-content/uploads/2018/02/hepsiburada-logo.png", WebsiteUrl = "https://www.hepsiburada.com"},
            };
            websites.ForEach(w=>context.Websites.AddOrUpdate(w));
            context.SaveChanges();

            List<WebsiteUrl> websiteUrls = new List<WebsiteUrl>()
            {
                new WebsiteUrl(){WebsiteId=2,Id= 1, Url="https://www.amazon.com.tr/gp/bestsellers/books/", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteId=2,Id= 2, Url="https://www.amazon.com.tr/gp/bestsellers/books/ref=zg_bs_pg_2?ie=UTF8&pg=2", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteId=1,Id= 3, Url="https://www.bkmkitap.com/kitap/cok-satan-kitaplar/", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteId=1,Id= 4, Url="https://www.bkmkitap.com/kitap/cok-satan-kitaplar?pg=2/", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteId=3,Id= 5, Url="https://kidega.com/cok-satan-kitaplar/", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteId=3,Id= 6, Url="https://kidega.com/cok-satan-kitaplar?page=2/", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteId=4,Id= 5, Url="https://www.kitap16.com/cok-satanlar-kitapligi", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteId=4,Id= 6, Url="https://www.kitap16.com/index.php?p=Products&fpt_id=114&sort_type=rel-desc&page=2", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteId=5,Id= 5, Url="https://www.dr.com.tr/CokSatanlar/Kitap/", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteId=5,Id= 6, Url="https://www.dr.com.tr/CokSatanlar/Kitap/#/page=2/", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteId=6,Id= 5, Url="https://www.ilknokta.com/index.php?p=ProductBestsellers&mod_id=41&page=1&period=yearly", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteId=6,Id= 6, Url="https://www.ilknokta.com/index.php?p=ProductBestsellers&mod_id=41&page=2&period=yearly", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteId=7,Id= 5, Url="https://www.eganba.com/kitap/cok-satanlar/", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteId=7,Id= 6, Url="https://www.eganba.com/kitap/cok-satanlar/?page=2", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteId=8,Id= 5, Url="https://www.kitapsec.com/Products/Edebiyat/Cok-Satan-Kitaplar/", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteId=8,Id= 6, Url="https://www.kitapsec.com/Products/Edebiyat/Cok-Satan-Kitaplar/2-6-0a0-0-0-0-0-0.xhtml", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteId=9,Id= 5, Url="https://www.idefix.com/CokSatanlar/Kitap", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteId=9,Id= 6, Url="https://www.idefix.com/CokSatanlar/Kitap#/page=2", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteId=10,Id= 5, Url="https://www.fidankitap.com/cok-satanlar-2", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteId=10,Id= 6, Url="https://www.fidankitap.com/index.php?p=ProductBestsellers&mod_id=146&page=2", UrlType="Best-Seller" },

                new WebsiteUrl(){WebsiteId=11,Id= 5, Url="https://www.hepsiburada.com/kampanyalar/cok-satan-kitaplar", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteId=11,Id= 6, Url="https://www.hepsiburada.com/kampanyalar/cok-satan-kitaplar?sayfa=2", UrlType="Best-Seller" },

            };

            websiteUrls.ForEach(w => context.WebsiteUrls.AddOrUpdate(w));
            context.SaveChanges();

            List<BookNode> bookNodes = new List<BookNode>()
            {
                //Hepsiburada
                new BookNode(){WebsiteId = 11, Name = "/*[contains(@id, 'product-name')]",
                    Author = "//*[contains(@id, 'product-name')]",
                    Price = "//div[contains(@class, 'product-price')]/span[contains(@class, 'price')]",
                    Publisher = "//div[contains(@class, 'product-information')]/span[@class='brand-name']/a",
                    Image = "//img[@class='product-image']",
                    Detail = "//div[contains(@class, 'product')]/a",
                    ItemCount = 100,
                },
                //Amazon
                new BookNode()
                {
                    WebsiteId = 2, Name = "//*[@id='productTitle']", 
                    Author = "//span[1][contains(@class , 'author')]/a",
                    Price = "//div[@class='a-row']/span[contains(@class , 'inlineBlock-display')]/span",
                    Publisher = "//div[@id='detailBulletsWrapper_feature_div'] //ul/li[1]/span/span[2]",
                    Image = "//div[@id='img-canvas']/img",
                    Detail = "//*[@class='aok-inline-block zg-item']/a[@class='a-link-normal']",
                    ItemCount = 100,
                },
                //Bkm
                new BookNode()
                {
                    WebsiteId = 1,
                    Name = "//h1[@id='productName']",
                    Author = "//a[@id='productModelText']",
                    Price = "//span[@class='product-price'])",
                    Publisher = "//a[contains(@class, 'product-brand')]/span",
                    Image = "//span[@class='imgInner']/img/@src",
                    Detail = "//a[@class='fl col-12 text-description detailLink']",
                    ItemCount = 100,

                }
            };

            bookNodes.ForEach(b=>context.BookNodes.AddOrUpdate(b));
            context.SaveChanges();
            base.Seed(context);
        }
    }
}
