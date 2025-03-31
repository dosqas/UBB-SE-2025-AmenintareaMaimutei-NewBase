//e un model de topic ajutator

namespace Duolingo2.Models
{
    public class Topic
    {
        public string Name { get; set; }

        public Topic(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
