using System.Collections;
using System.Collections.Generic;

public class GfycatItem
{
    public string[] tags { get; set; }
    public string[] languageCategories { get; set; }
    public string[] domainWhitelist { get; set; }
    public string[] geoWhitelist { get; set; }
    public bool published { get; set; } //might be int
    public string nsfw { get; set; }
    public int gatekeeper { get; set; }

    public string mp4Url { get; set; }
    public string gifUrl { get; set; }
    public string webmUrl { get; set; }
    public string webpUrl { get; set; }
    public string mobileUrl { get; set; }
    public string mobilePosterUrl { get; set; }

    public string extraLemmas { get; set; }

    public string thumb100PosterUrl { get; set; }
    public string miniUrl { get; set; }
    public string gif100px { get; set; }
    public string miniPosterUrl { get; set; }
    public string max5mbGif { get; set; }
    public string title { get; set; }

    public string max2mbGif { get; set; }
    public string max1mbGif { get; set; }
    public string posterUrl { get; set; }
    public string languageText { get; set; }
    public int views { get; set; }

    public string userName { get; set; }
    public string description { get; set; }

    public bool hasTransparency { get; set; }
    public bool hasAudio { get; set; }
    public int likes { get; set; } //might be string
    public int dislikes { get; set; } //might be string

    public int gfyNumber { get; set; } //might be string
    public string gfyId { get; set; }
    public string gfyName { get; set; }
    public string avgColor { get; set; }
    public string rating { get; set; }
    public string gfySlug { get; set; }

    public int width { get; set; }
    public int height { get; set; }
    public float frameRate { get; set; }
    public float numFrames { get; set; }
    public int mp4Size { get; set; }
    public int webmSize { get; set; }
    public long createDate { get; set; }
    public int source { get; set; }

    public Dictionary<string, GfycatContentUrls> content_urls { get; set; }
}