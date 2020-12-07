using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Youtube_Playlist_App.Controllers
{

        public class HomeController : Controller
        {

            public ActionResult Index()
            {


                return View();
            }
            [HttpGet]

            public string GetTime(string url, int min = 0)
            {
                try
                {
                    TimeSpan timeSpan = PlaylistApp.Driver(url, min);

                    return $"The Total time in  Playlist is {timeSpan.Days} Days, {timeSpan.Hours} Hours, {timeSpan.Minutes} Minutes and {timeSpan.Seconds} Seconds";
                }
                catch
                {
                    return "Please Read the Instructions and Enter Playlist Id in Given Format";
                }



            }

        }

        public class PlaylistApp
        {
            public static TimeSpan Driver(string url, int min = 0)
            {
                var html = new WebClient().DownloadString("https://www.youtube.com/playlist?list=" + url.Trim());

                string value = GetBetween(html, @"ytInitialData = ", @"</script>");

                JObject parsed = JObject.Parse(value);

                var list = new List<string>();
                int max = (int)parsed.SelectToken("sidebar.playlistSidebarRenderer.items.[0].playlistSidebarPrimaryInfoRenderer.stats.[0].runs.[0].text");

                for (int i = min; i < 99 && i < max; i++)
                {
                    var token = parsed.SelectToken("contents.twoColumnBrowseResultsRenderer.tabs.[0].tabRenderer.content.sectionListRenderer.contents.[0].itemSectionRenderer.contents[0].playlistVideoListRenderer.contents.[" + i + "].playlistVideoRenderer.lengthText.simpleText");
                    list.Add(token.ToString());
                }
                return Time(list);


            }

            private static TimeSpan Time(List<string> time)
            {
                TimeSpan timeSpan = new TimeSpan(0);
                var list = new List<TimeSpan>();
                foreach (var item in time)
                {
                    string timer = item.IndexOf(":") == -1 ? $"00:00:{item}" : item.IndexOf(":") == item.LastIndexOf(":") ? $"00:{item}" : item;
                    TimeSpan times = TimeSpan.Parse(timer);
                    list.Add(times);
                }
                foreach (TimeSpan value in list)
                {
                    timeSpan = timeSpan.Add(value);
                }
                return timeSpan;
            }

            private static string GetBetween(string content, string startString, string endString)
            {
            //|| content.Contains(endString)
            if (content.Contains(startString) && content.Contains(endString))
                {
                    int Start = content.IndexOf(startString, 0) + startString.Length;
                    int End = content.IndexOf(endString, Start);
                    return content.Substring(Start, (End - 1) - Start);
                }
                else
                    return string.Empty;
            }
        }
    }