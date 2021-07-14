namespace WebScraping.WebUI.Models
{
    public class ItemCheckedModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemEntityName { get; set; }
        /// <summary>
        /// Best seller or All books
        /// </summary>
        public string ItemCategoryType { get; set; }
        public bool IsCheck { get; set; }

    }
}