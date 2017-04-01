using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterPoc
{
    class Program
    {
        static void Main(string[] args)
        {

            var phrases = new List<Thing>
            {
                new Thing { Phrase = "service bus", Sentiment = .5},
                //new Thing { Phrase = "sb", Sentiment = .45},
                //new Thing { Phrase = "connect", Sentiment = .5},
                //new Thing { Phrase = "kinect", Sentiment = .5},
                //new Thing { Phrase = "xbox360", Sentiment = .5},
                //new Thing { Phrase = "xboxOne", Sentiment = .5},
                //new Thing { Phrase = "xbox", Sentiment = .5},
                //new Thing { Phrase = "virtual machine", Sentiment = .5},
                //new Thing { Phrase = "vm", Sentiment = .5},
                new Thing { Phrase = "server", Sentiment = .5},
                new Thing { Phrase = "servers", Sentiment = .5},
                //new Thing { Phrase = "broken", Sentiment = .1},
                //new Thing { Phrase = "break", Sentiment = .1},
                //new Thing { Phrase = "AzureStack", Sentiment = .5},
                //new Thing { Phrase = "AzureStack#3 TLS", Sentiment = .5},
                //new Thing { Phrase = "AzStack#3 TLS", Sentiment = .5},
                //new Thing { Phrase = "serivc bus", Sentiment = .5},
            };


            var topicRepo = new TopicRelationRepository();


            var clusters = new Dictionary<string, List<ClusterDecision>>();

            foreach (var t in phrases)
            {
                foreach (var t2 in phrases)
                {
                    if (t.Phrase == t2.Phrase)
                        continue;

                    var confidence = topicRepo.Compare(t, t2);
                    Console.WriteLine($"{t.Phrase} vs {t2.Phrase}: {confidence}");

                    if (!clusters.ContainsKey(t.Phrase))
                        clusters.Add(t.Phrase, new List<ClusterDecision>());

                    //if confidence is > 80, cluster it.
                    if (confidence > 80.0)
                    {
                        clusters[t.Phrase].Add(new ClusterDecision { RelatedPhrase = t2.Phrase, ConfidenceRating = confidence });
                    }
                }
            }

            Console.WriteLine("\n\n");

            foreach (var cluster in clusters)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Cluster: {cluster.Key}:");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var related in cluster.Value)
                {
                    Console.WriteLine($"{related.RelatedPhrase}  @{Math.Round(related.ConfidenceRating, 3)}%");
                }
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
