using System;
using Hangfire;
using Microsoft.Owin.Hosting;
using Tweetinvi;

namespace TwitterBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Auth.SetUserCredentials("consumer_key", "consumer_secret", "access_token", "access_token_secret");

            GlobalConfiguration.Configuration.UseSqlServerStorage("<connection-string>");
            using (WebApp.Start("http://localhost:9000"))
            {
                RecurringJob.AddOrUpdate(() => Tweet("Text for the tweet."), Cron.Daily);

                Console.WriteLine("Hangfire server started. Press any key to exit.");
                Console.ReadKey();
            }
        }

        public static ITweet Tweet(string message)
        {
            var tweet = Tweet.PublishTweet(message);

            if (tweet == null)
            {
                Console.WriteLine("Failed to tweet.");
            }
            else
            {
                Console.WriteLine("Tweeted: {0}", message);
            }

            return tweet;
        }
    }

    [TestFixture]
    public class TwitterBotTests
    {
        [SetUp]
        public void Setup()
        {
            Auth.SetUserCredentials("test_consumer_key", "test_consumer_secret", "test_access_token", "test_access_token_secret");
        }

        [Test]
        public void Tweet_ValidMessage_ReturnsTweetId()
        {
            var message = "Text for the tweet.";

            var tweet = Program.Tweet(message);

            Assert.IsNotNull(tweet);
            Assert.IsInstanceOf(typeof(ITweet), tweet);
            Assert.AreEqual(tweet.FullText, message);
        }

        [Test]
        public void Tweet_InvalidMessage_ReturnsNull()
        {
            var message = new string('a', 281);

            var tweet = Program.Tweet(message);

            Assert.IsNull(tweet);
        }
    }
}
