using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackOvF.Models;
using StackOvF.Models.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Web;

namespace StackOvF
{
    public class Program
    {
        private const int MAX_USER_COUNT = 6132420;
        private const int MAX_PAGE_COUNT = 170345;
        private const int USERS_PER_PAGE = 36;
        private const int USERS_SAVE_INTERVALS = 360;
        private static List<User> totalUsers = new List<User>();
        public static void Main(string[] args)
        {
            if (!System.IO.Directory.Exists("users")) System.IO.Directory.CreateDirectory("users");

            Program p = new Program();
            Console.WriteLine($"====== StackOvF Users ======");
            Console.WriteLine($"1\tGet Users From Pages");
            Console.WriteLine($"2\tGet Individual Users Info");
            int option = Convert.ToInt32(((char)Console.Read()).ToString());
            switch (option)
            {
                case 1:
                    p.getUsersFromPages();
                    break;
                case 2:
                    p.getUserInfo(1);
                    break;
                default:
                    break;
            }
            Console.Read();
        }

        public void getUsersFromPages()
        {
            List<User> users = new List<User>();
            int beginPage = Math.Max(1, Convert.ToInt32(new System.IO.DirectoryInfo("users").GetFiles().Count() * USERS_SAVE_INTERVALS / 36));
            int currentUserCount = beginPage * USERS_SAVE_INTERVALS;
            for (int i = beginPage; i <= MAX_PAGE_COUNT; i++)
            {
                Console.WriteLine($"Page {i} of {MAX_PAGE_COUNT}");
                users.AddRange(this.getUsersFromPage(i));
                if (users.Count >= USERS_SAVE_INTERVALS)
                {
                    string json = JsonConvert.SerializeObject(users);
                    string filePath = $"users/users-{i * USERS_PER_PAGE - USERS_SAVE_INTERVALS + 1}-{i * USERS_PER_PAGE}.json";
                    System.IO.File.WriteAllText(filePath, json);
                    Console.WriteLine($"Saved: ~/{filePath}");
                    totalUsers.AddRange(users);
                    users.Clear();
                }
                currentUserCount += USERS_PER_PAGE;
            }
        }

        public List<User> getUsersFromPage(int pageId)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Api.Get($"http://stackoverflow.com/users?page={pageId}&ab=Reputation&filter=all"));
            var span = HtmlNode.CreateNode("<span></span>");
            var img = HtmlNode.CreateNode("<img src=''></img>");

            List<User> users = new List<User>();
            var nodes = doc.GetElementsByClassName("user-info");
            foreach (var node in nodes)
            {
                var nodeDetail = node.Descendants("div").Where((d) => d.Attributes["class"].Value == "user-details").FirstOrDefault(span);
                User user = new User();
                try
                {
                    user.name = nodeDetail.Descendants("a").FirstOrDefault().InnerText.Trim();
                    user.imageUrl = node.Descendants("img").FirstOrDefault(img).GetAttributeValue("src", "");

                    user.flair = new User.Flair();
                    var nodeFlair = node.Descendants("div").Where((d) => d.Attributes["class"]?.Value == "-flair").FirstOrDefault(span);
                    var nodeFlairSpans = nodeFlair.Descendants("span");
                    user.reputationText = nodeFlairSpans.Where(d => d.Attributes["class"]?.Value == "reputation-score").FirstOrDefault(span).InnerText.Trim();
                    user.reputation = Convert.ToInt32(Convert.ToDecimal(user.reputationText.Replace("k", "")) * 1000);
                    try
                    {
                        user.flair.gold = nodeFlairSpans.Where(d => d.Attributes["class"]?.Value == ("badgecount")).ElementAt(0)?.InnerText.Trim();
                        user.flair.silver = nodeFlairSpans.Where(d => d.Attributes["class"]?.Value == ("badgecount")).ElementAt(1)?.InnerText.Trim();
                        user.flair.bronze = nodeFlairSpans.Where(d => d.Attributes["class"]?.Value == ("badgecount")).ElementAt(2)?.InnerText.Trim();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }


                    user.languages = node.Descendants("div").Where((d) => d.Attributes["class"]?.Value == "user-tags").FirstOrDefault(span).InnerText.Trim();
                    user.address = node.Descendants("span").Where((d) => d.Attributes["class"]?.Value == "user-location").FirstOrDefault(span).InnerText.Trim();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }

                users.Add(user);
            }
            return users;
        }

        public User getUserInfo(int userId)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Api.Get("http://stackoverflow.com/users/" + userId));
            User ret = new User();
            var span = HtmlNode.CreateNode("<span></span>");
            ret.name = HttpUtility.HtmlDecode(doc.GetElementsByClassName("user-card-name").FirstOrDefault(span).InnerText.Trim().Split('\n').FirstOrDefault("").Trim());
            ret.imageUrl = HttpUtility.HtmlDecode(doc.GetElementsByClassName("avatar-user").FirstOrDefault(span).GetAttributeValue("src", ""));
            ret.address = HttpUtility.HtmlDecode(doc.GetElementsByClassName("icon-location").FirstOrDefault(span).ParentNode.InnerText.Trim());
            ret.twitterUrl = HttpUtility.HtmlDecode(doc.GetElementsByClassName("icon-twitter").FirstOrDefault(span).NextSibling.NextSibling.GetAttributeValue("href", ""));
            ret.siteUrl = HttpUtility.HtmlDecode(doc.GetElementsByClassName("icon-site").FirstOrDefault(span).NextSibling.NextSibling.GetAttributeValue("href", ""));
            ret.stackOverFlowAge = HttpUtility.HtmlDecode(doc.GetElementsByClassName("icon-history").FirstOrDefault(span).ParentNode.InnerText.Trim());
            ret.githubUrl = HttpUtility.HtmlDecode(doc.GetElementsByClassName("icon-github").FirstOrDefault(span).NextSibling.NextSibling.GetAttributeValue("href", ""));
            return ret;
        }
    }
}
