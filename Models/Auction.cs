using System;
using System.Collections.Generic;

namespace Ineichen_Crawler.Models;

public partial class Auction
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? Link { get; set; }

    public int? LotCount { get; set; }

    public int? StartDate { get; set; }

    public string? StartMonth { get; set; }

    public int? StartYear { get; set; }

    public string? StartTime { get; set; }

    public int? EndDate { get; set; }

    public string? EndMonth { get; set; }

    public int? EndYear { get; set; }

    public string? EndTime { get; set; }

    public string? Location { get; set; }
}
