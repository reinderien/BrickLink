namespace BrickLink.Analysis
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using CsvHelper;
    using CsvHelper.Configuration;
    using CsvHelper.Configuration.Attributes;

    public record CatalogueItem
    {
        [Name("Category ID")]
        public int CategoryID { get; init; }
        
        [Name("Category Name")]
        public string CategoryName { get; init; }
        
        public string Number { get; init; }
        public string Name { get; init; }

        private readonly int? _year;
        public int? Year => _year;

        [Name("Year Released")]
        public string YearInit
        {
            init
            {
                int year;
                if (int.TryParse(value, out year))
                    _year = year;
            }
        }
        
        private readonly float? _weight;
        public float? Weight => _weight;

        [Name("Weight (in Grams)")]
        public string WeightInit
        {
            init
            {
                float weight;
                if (float.TryParse(value, out weight))
                    _weight = weight;
            }
        }

        /// <summary>
        /// Iterate through a TSV from
        /// https://www.bricklink.com/catalogDownload.asp
        /// </summary>
        /// <param name="filename">Path to the tab-separated value file</param>
        /// <returns></returns>
        public static IEnumerable<CatalogueItem> FromTsv(
            string filename = "Minifigures.txt")
        {
            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                IgnoreBlankLines = true,
                // Don't interpret quote " characters with any special meaning.
                Escape = '\0', Quote = '\0' 
            };

            using (StreamReader stream = new(filename))
            using (CsvReader csv = new(stream, config))
            {
                foreach (CatalogueItem row in csv.GetRecords<CatalogueItem>())
                    yield return row;
            }
        }
    }
}
