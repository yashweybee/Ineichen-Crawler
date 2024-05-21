using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Ineichen_Crawler.View_Models;
using System.Data.SqlClient;
using System;

namespace Ineichen_Crawler
{
    public class Program
    {

        private static readonly string url = "https://ineichen.com/auctions/past/";

        static void Main(string[] args)
        {
            ScrapeAndStoreData().Wait();
        }

        private static async Task ScrapeAndStoreData()
        {
            var connectionString = "Server=DESKTOP-AH3AP4P\\MSSQLSERVER01;Database=Ineichen;Trusted_Connection=True;TrustServerCertificate=True";
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var auctionNodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='auctions-list']//div[@class='auction-item']");
            if (auctionNodes == null)
            {
                Console.WriteLine("No auction nodes found.");
                return;
            }

            foreach (var node in auctionNodes)
            {
                try
                {
                    var titleNode = node.SelectSingleNode(".//h2[@class='auction-item__name']/a");
                    var descriptionNode = node.SelectSingleNode(".//div[contains(@class, 'auction-date-location')]");
                    var imageUrlNode = node.SelectSingleNode(".//a[@class='auction-item__image']/img");
                    var linkNode = node.SelectSingleNode(".//a[@class='auction-item__image']");
                    var lotCountNode = node.SelectSingleNode(".//div[@class='auction-item__btns']/a");
                    //var dateNode = node.SelectSingleNode(".//div[contains(@class, 'auction-date-location')]/b");
                    var dateNode = node.SelectSingleNode(".//div[contains(@class, 'auction-date-location')]/div[1]");
                    var timeNode = node.SelectSingleNode(".//div[contains(@class, 'auction-date-location')]/span");
                    var locationNode = node.SelectSingleNode(".//div[contains(@class, 'auction-date-location')]/div[2]/span");

                    var txtxtt = dateNode.InnerText?.Trim();
                    var dateNewGenerated = ExtractDate(dateNode?.InnerText?.Trim());

                    var timenodetxt = timeNode.InnerText?.Trim();

                    var auction = new VMAuction
                    {
                        Title = titleNode?.InnerText.Trim(),
                        Description = descriptionNode?.InnerText.Trim(),
                        ImageUrl = "https://ineichen.com" + imageUrlNode?.GetAttributeValue("src", string.Empty),
                        Link = "https://ineichen.com" + linkNode?.GetAttributeValue("href", string.Empty),
                        LotCount = lotCountNode != null ? ExtractLotCount(lotCountNode.InnerText.Trim()) : 0,
                        StartDate = dateNode != null ? dateNewGenerated.StartDate : null,
                        StartMonth = dateNode != null ? dateNewGenerated.StartMonth : null,
                        StartYear = dateNode != null ? dateNewGenerated.StartYear : null,
                        StartTime = timeNode?.InnerText?.Trim(),
                        EndDate = dateNode != null ? dateNewGenerated.EndDate : null,
                        EndMonth = dateNode != null ? dateNewGenerated.EndMonth : null,
                        EndYear = dateNode != null ? dateNewGenerated.EndYear : null,
                        EndTime = null,
                        Location = locationNode?.SelectSingleNode(".//a")?.InnerText.Trim() ?? locationNode?.InnerText.Trim()
                    };

                    // Print or store the auction object
                    Console.WriteLine($"Title: {auction.Title}");
                    Console.WriteLine($"ImageUrl: {auction.ImageUrl}");
                    Console.WriteLine($"Link: {auction.Link}");
                    Console.WriteLine($"LotCount: {auction.LotCount}");
                    Console.WriteLine($"StartDate: {auction.StartDate}");
                    Console.WriteLine($"EndDate: {auction.EndDate}");
                    Console.WriteLine($"Location: {auction.Location}");
                    Console.WriteLine();

                    // Save to the database
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        var sql = @"INSERT INTO Auctions (Title, Description, ImageUrl, Link, LotCount, Location, StartDate, EndDate, StartMonth, EndMonth, StartYear, EndYear, StartTime)
            VALUES (@Title, @Description, @ImageUrl, @Link, @LotCount, @Location, @StartDate, @EndDate, @StartMonth, @EndMonth, @StartYear, @EndYear, @StartTime)";

                        using (var command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@Title", auction.Title);
                            command.Parameters.AddWithValue("@Description", auction.Description);
                            command.Parameters.AddWithValue("@ImageUrl", auction.ImageUrl);
                            command.Parameters.AddWithValue("@Link", auction.Link);
                            command.Parameters.AddWithValue("@LotCount", auction.LotCount);
                            command.Parameters.AddWithValue("@Location", auction.Location);
                            command.Parameters.AddWithValue("@StartDate", auction.StartDate);
                            command.Parameters.AddWithValue("@EndDate", auction.EndDate);
                            command.Parameters.AddWithValue("@StartMonth", auction.StartMonth);
                            command.Parameters.AddWithValue("@EndMonth", auction.EndMonth);
                            command.Parameters.AddWithValue("@StartYear", auction.StartYear);
                            command.Parameters.AddWithValue("@EndYear", auction.EndYear);
                            command.Parameters.AddWithValue("@StartTime", auction.StartTime);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing in node: {ex.Message}");
                }
            }
        }


        private static int ExtractLotCount(string text)
        {
            // Example text: "View 90 lots"
            var parts = text.Split(' ');
            foreach (var part in parts)
            {
                if (int.TryParse(part, out int lotCount))
                {
                    return lotCount;
                }
            }
            return 0;
        }

        private static VMDate ExtractDate(string dateText)
        {
            VMDate generatedDate = new VMDate();
            //string pattern = @"(?<startDay>\d{1,2})(?:\s*-\s*(?<endDay>\d{1,2}))?\s+(?<startMonth>[A-Za-z]+)\s*(?:\b(?<startYear>\d{4})\b)?(?:\s*-\s*(?<endMonth>[A-Za-z]+)\s*(?:\b(?<endYear>\d{4})\b)?)?";

            //string pattern1 = @"(?<startDay>\d{1,2})(?:\s*-\s*(?<endDay>\d{1,2}))?\s+(?<startMonth>[A-Za-z])\s*(?:\b(?<startYear>\d{4})\b)?(?:\s*-\s*(?<endMonth>[A-Za-z]{3})\s*(?:\b(?<endYear>\d{4})\b)?)?";
            //string pattern = @"(?<startDay>\d{1,2})(?:\s*-\s*(?<endDay>\d{1,2}))?\s+(?<startMonth>[A-Za-z]+)(?:\s+(?<startYear>\d{4}))?(?:\s*-\s*(?<endMonth>[A-Za-z]+)(?:\s+(?<endYear>\d{4}))?)?";

            //string pattern = @"(?<startDate>\d+)\s*(?:-)?\s*(?<endDate>\d+)\s*(?<startMonth>[A-Za-z]+)\s*(?<startYear>\d+)?\s*(?:-)?\s*(?<endMonth>[A-Za-z]+)\s*(?<endYear>\d+)?";
            string pattern = @"(?<StartDate>\d{1,2})\s*(?<StartMonth>(?:JANUARY|FEBRUARY|MARCH|APRIL|MAY|JUNE|JULY|AUGUST|SPETEMBER|OCTOMBER|NOVEMBER|DECEMBER|JAN|FRB|MAR|APR|MAY|JUN|JUL|AUG|SEPT|OCT|NOV|DEC))\,?\s*(?<Year>\d{4})?(\d{2}:\d{2}\s*\(?CET\)?\s*)?\s*(,|-)?\s*(?<EndDate>\d{1,2})?\s*(?<EndMonth>(?:JANUARY|FEBRUARY|MARCH|APRIL|MAY|JUNE|JULY|AUGUST|SPETEMBER|OCTOMBER|NOVEMBER|DECEMBER|JAN|FRB|MAR|APR|MAY|JUN|JUL|AUG|SEPT|OCT|NOV|DEC))?(\,)?\s*(?<EndYear>\d{4})?(?<Time>\d{2}:\d{2}\s*(CET)?)?";
            var match = Regex.Match(dateText, pattern);

            if (match.Success)
            {
                generatedDate.StartDate = int.Parse(match.Groups["StartDate"].Value);
                generatedDate.StartMonth = match.Groups["startMonth"].Value;
                generatedDate.StartYear = match.Groups["Year"].Success ? int.Parse(match.Groups["startYear"].Value) : 0;
                generatedDate.EndMonth = match.Groups["EndMonth"].Success ? match.Groups["endMonth"].Value : generatedDate.StartMonth;
                generatedDate.EndDate = match.Groups["EndDate"].Success ? int.Parse(match.Groups["endDay"].Value) : generatedDate.StartDate;
                generatedDate.EndYear = match.Groups["EndYear"].Success ? int.Parse(match.Groups["endYear"].Value) : generatedDate.StartYear;
            }
            return generatedDate;
        }
    }
}
