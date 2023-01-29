using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuotesWebApp.Models
{
    public class Quote
    {
        public int Id { get; set; }
        [DisplayName("Quote Text")]
        public string QuoteText { get; set; }
        public string Author { get; set; }
        public string Tags { get; set; }

        public string Category { get; set; }


    }

}
