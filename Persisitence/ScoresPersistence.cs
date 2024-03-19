using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LunarLander.Persisitence
{
    [DataContract(Name = "ScoresPersistence")]
    class ScoresPersistence
    {
        public ScoresPersistence() { }

        public ScoresPersistence(List<float> scores, float newScore) 
        { 
            scores.Add(newScore);
            scores.Sort((a, b) => b.CompareTo(a));
            if (scores.Count > 5)
            { 
                this.Scores = scores.Take(5).ToList();
            }
        }

        [DataMember()]
        public List<float> Scores { get; set; }
    }
}
