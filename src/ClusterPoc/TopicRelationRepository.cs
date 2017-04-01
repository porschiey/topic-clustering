using Phonix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClusterPoc
{
    public class TopicRelationRepository
    {
        private static Regex allNonAlphanumeric = new Regex("[^a-zA-Z0-9]", RegexOptions.IgnoreCase);
        private static DoubleMetaphone dblMetaphone = new DoubleMetaphone();

        //potentially un-used acyronym words
        private static string[] unused = new string[] { "the", "and", "or" };

        public double Compare(Thing a, Thing b)
        {
            if (string.IsNullOrWhiteSpace(a.Phrase) || string.IsNullOrWhiteSpace(b.Phrase))
                throw new ArgumentNullException("One or more of the phrases was found to be null/empty.");

            //strip all non-alphanumeric characters from the phrase, including spaces, and lowcap the entire string
            var x = allNonAlphanumeric.Replace(a.Phrase, string.Empty).ToLowerInvariant();
            var y = allNonAlphanumeric.Replace(b.Phrase, string.Empty).ToLowerInvariant();

            var confidence = 0.0;

            confidence += this.TextInEither(x, y);
            confidence += this.CharacterComparison(x, y);
            confidence += this.PhoneticComparison(x, y);
            confidence += this.IsAcyronym(a.Phrase, b.Phrase);

            //last straw
            if (confidence < 65.0)
                confidence += this.SentimentRelation(a.Sentiment, b.Sentiment);
            

            return confidence;
        }

        public double TextInEither(string a, string b)
        {
            const double worth = 75.0;

            if (a.Contains(b))
                return worth;

            if (b.Contains(a))
                return worth;

            return 0.0;
        }

        public double CharacterComparison(string a, string b)
        {
            var larger = a.Length > b.Length ? a.Length : b.Length;
            var distance = LevenshteinDistance.Compute(a, b);
            var rating = (larger - distance) / (double)larger;

            return rating * 100;
        }

        /// <summary>
        /// Max percent return on this is 40%. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public double PhoneticComparison(string a, string b)
        {
            var aKey = dblMetaphone.BuildKey(a);
            var bKey = dblMetaphone.BuildKey(b);
            var subRating = this.CharacterComparison(aKey, bKey) / 100; //char comp already gives percent at above base/1
            var rating = (subRating * 40);

            return rating;
        }

        public double IsAcyronym(string x, string y)
        {
            string f, s;
            if (x.Length > y.Length)
            {
                //x is the longer (therefore the one that could be the full version)
                f = x;
                s = y;
            }
            else
            {
                f = y;
                s = x;
            }

            f = f.ToLowerInvariant().Trim();
            s = s.ToLowerInvariant().Trim();

            //if the "spaced phrase" doesn't have the space (or what would be the shorthand does), bail out
            if (!f.Contains(" ") || s.Contains(" "))
                return 0.0;            

            //split f into words
            var words = f.Split(' ');

            var acyronymLetterCount = words.Count(w => !unused.Contains(w) && !string.IsNullOrWhiteSpace(w));
            if (acyronymLetterCount != s.Length)
                return 0.0; //the word count to character count simply wouldn't match up

            //drill out first char in each word
            var letters = new char[s.Length];
            for (int i = 0; i < words.Count(); i++)
            {
                letters[i] = words[i][0];
            }

            var fullAsAcryo = new string(letters);
            if (fullAsAcryo == s)
                return 65.0; //worth 65 points

            return 0.0;
        }

        public double SentimentRelation(double a, double b)
        {
            const double worth = 15.0;
            var diff = Math.Abs(a - b);
            if (diff < 0.10)
                return worth;

            return 0.0;
        }        
    }


    public class Thing
    {
        public string Phrase { get; set; }

        public double Sentiment { get; set; }
    }

    public class ClusterDecision
    {
        public string RelatedPhrase { get; set; }

        public double ConfidenceRating { get; set; }
    }
}
