using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml;

namespace QuotesWebApp.Models
{
    public class Program
    {
        //small code for converting old json data in new that the main program will use
        public static void Main()
        {
            string oldFile = "C:\\Users\\User\\source\\repos\\QuotesWebApp\\JsonToSql\\quotes.json";
            string newFile = "C:\\Users\\User\\source\\repos\\QuotesWebApp\\QuotesWebApp\\wwwroot\\lib\\json\\quotesDb.json";
            //basically getting old json file and transforming it in the list of objects and creating new list
            List<QuoteOld> oldJson = JsonSerializer.Deserialize<List<QuoteOld>>(File.ReadAllText(oldFile));
            List<QuoteNew> newJson = new List<QuoteNew>();
            foreach (QuoteOld quoteOld in oldJson)
            {

                // Checks for duplicates (old Db was poorly orginased
                bool isSame = false;
                foreach (QuoteNew quote in newJson)
                {
                    if(quote.Quote == quoteOld.Quote)
                    {
                        isSame = true;
                    }
                }
                if(isSame == true) {
                    continue;
                }
                // checks for empty properties
                string category = "overall";
                if (quoteOld.Category != null && quoteOld.Category != "" && quoteOld.Category != " ")
                {
                    category = quoteOld.Category;
                }
                List<string> tags = new List<string>();
                int max = (quoteOld.Tags.Count() > 8) ? 8 : quoteOld.Tags.Count() - 1;
                for(int i = 0; i < max; i++)
                {
                    if (!(quoteOld.Tags[i] == "attributed-no-source" || quoteOld.Tags[i].Contains("misattributed") || quoteOld.Tags[i].Contains("attributed") || quoteOld.Tags[i].Contains("-")))
                    {
                        tags.Add(quoteOld.Tags[i]);

                    }

                }
                if (tags.Count() == 0)
                {
                    tags.Add("overall");
                } 
                //creating a new object
                QuoteNew quoteNew = new QuoteNew()
                {
                    
                    Quote = quoteOld.Quote,
                    Author = quoteOld.Author,
                    Tags = string.Join(", ", tags.Select(item => item.ToString())),
                    Category = category
                };
                Console.WriteLine(quoteNew.Tags.ToString()); ;
                //adding to the overall list
                newJson.Add(quoteNew);
            }
            // for readability
            var options = new JsonSerializerOptions  
            {
                WriteIndented = true,
            };
            //writing new json file
            File.WriteAllText(newFile, JsonSerializer.Serialize(newJson, options));
        }





        public class QuoteNew
        {
            public string Quote { get; set; }
            public string Author { get; set; }
            public string Tags { get; set; }
            public string Category { get; set; }


        }
        public class QuoteOld
        {
            public string Quote { get; set; }
            public string Author { get; set; }
            public List<string> Tags { get; set; }
            public string Category { get; set; }


        }

    }
}
