using System.Collections.Generic;
using System.Data.Entity;
using WebScraping.Entities;


namespace WebScraping.Data.Concrete.Ef
{
    public class DataInitializer : CreateDatabaseIfNotExists<WebScrapingContext>
    {
        protected override void Seed(WebScrapingContext context)
        {
            List<WebsiteUrl> websiteUrls = new List<WebsiteUrl>()
            {
                new WebsiteUrl(){WebsiteName="Amazon",Id= 1, Url="https://www.amazon.com.tr/gp/bestsellers/books/", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteName="Amazon",Id= 2, Url="https://www.amazon.com.tr/gp/bestsellers/books/ref=zg_bs_pg_2?ie=UTF8&pg=2", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteName="Bkm Kitap",Id= 3, Url="https://www.bkmkitap.com/kitap/cok-satan-kitaplar/", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteName="Bkm Kitap",Id= 4, Url="https://www.bkmkitap.com/kitap/cok-satan-kitaplar?pg=2/", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteName="Kidega",Id= 5, Url="https://kidega.com/cok-satan-kitaplar/", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteName="Kidega",Id= 6, Url="https://kidega.com/cok-satan-kitaplar?page=2/", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteName="Kitap16",Id= 5, Url="https://www.kitap16.com/cok-satanlar-kitapligi", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteName="Kitap16",Id= 6, Url="https://www.kitap16.com/index.php?p=Products&fpt_id=114&sort_type=rel-desc&page=2", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteName="D&R",Id= 5, Url="https://www.dr.com.tr/CokSatanlar/Kitap/", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteName="D&R",Id= 6, Url="https://www.dr.com.tr/CokSatanlar/Kitap/#/page=2/", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteName="İlknokta",Id= 5, Url="https://www.ilknokta.com/index.php?p=ProductBestsellers&mod_id=41&page=1&period=yearly", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteName="İlknokta",Id= 6, Url="https://www.ilknokta.com/index.php?p=ProductBestsellers&mod_id=41&page=2&period=yearly", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteName="İlknokta",Id= 5, Url="https://www.eganba.com/kitap/cok-satanlar/", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteName="İlknokta",Id= 6, Url="https://www.eganba.com/kitap/cok-satanlar/?page=2", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteName="Kitapseç",Id= 5, Url="https://www.kitapsec.com/Products/Edebiyat/Cok-Satan-Kitaplar/", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteName="Kitapseç",Id= 6, Url="https://www.kitapsec.com/Products/Edebiyat/Cok-Satan-Kitaplar/2-6-0a0-0-0-0-0-0.xhtml", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteName="Idefix",Id= 5, Url="https://www.idefix.com/CokSatanlar/Kitap", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteName="Idefix",Id= 6, Url="https://www.idefix.com/CokSatanlar/Kitap#/page=2", UrlType="Best-Seller"},

                new WebsiteUrl(){WebsiteName="Fidan Kitap",Id= 5, Url="https://www.fidankitap.com/cok-satanlar-2", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteName="Fidan Kitap",Id= 6, Url="https://www.fidankitap.com/index.php?p=ProductBestsellers&mod_id=146&page=2" },

                new WebsiteUrl(){WebsiteName="Hepsiburada",Id= 5, Url="https://www.hepsiburada.com/kampanyalar/cok-satan-kitaplar", UrlType="Best-Seller"},
                new WebsiteUrl(){WebsiteName="Hepsiburada",Id= 6, Url="https://www.hepsiburada.com/kampanyalar/cok-satan-kitaplar?sayfa=2" },

            };

            context.WebsiteUrls.AddRange(websiteUrls);
            context.SaveChanges();

            List<Website> websites = new List<Website>()
            {
                new Website(){Id=1,Name="Bkm Kitap", LogoUrl= "http://www.bkmkitap.com/Data/EditorFiles/logonew23.png"},
                new Website(){Id=2,Name="Amazon", LogoUrl= "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a9/Amazon_logo.svg/1920px-Amazon_logo.svg.png"},
                new Website(){Id=3,Name="Kidega", LogoUrl= "https://cdn.kidega.com/assets/web/img/kidega-logo.png"},
                new Website(){Id=4,Name="Kitap16", LogoUrl= "https://www.kitap16.com/u/kitap16/kitap-transparan-1579509339.png"} ,
                new Website(){Id=5,Name="D&R", LogoUrl= "https://www.dr.com.tr/Themes/DR/Content/assets/images/general/head-logo.png"},
                new Website(){Id=6,Name="İlknokta", LogoUrl= "https://www.ilknokta.com/u/ilknokta/ilknokta-logosu-1613392480.jpg"},
                new Website(){Id=7,Name="Eganba", LogoUrl= "https://www.eganba.com/wwwroot/images/eganba-logo.png"},
                new Website(){Id=8,Name="Kitapseç", LogoUrl= "https://cdn.kitapsec.com//temalar/KitapSec2017/img/logo.jpg"},
                new Website(){Id=9,Name="Idefix", LogoUrl= "https://fragtist.com/wp-content/uploads/2017/04/fragtist-IDEFIX-750x349.gif"},
                new Website(){Id=10,Name="Fidan Kitap", LogoUrl= "https://www.fidankitap.com/u/fidankitap/fidan-kitap-logo-9-1576766279.png"},
                new Website(){Id=11,Name="Hepsiburada", LogoUrl= "https://cdn.freelogovectors.net/wp-content/uploads/2018/02/hepsiburada-logo.png" },
            };
            context.Websites.AddRange(websites);
            context.SaveChanges();

            base.Seed(context);
        }
    }
}