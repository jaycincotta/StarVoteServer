using System;
using System.Collections.Generic;

namespace StarVoteServer
{
    public class IdGenerator
    {
        public static string NextVoterId(IList<string> existing = null)
        {
            var idGenerator = new IdGenerator();
            if (existing != null)
            {
                idGenerator.Preload(existing);
            }
            return idGenerator.NextId();
        }

        private const int IdLength = 6;
        private const string _alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // excludes I O 0 1

        private Random _rand = new Random();
        private HashSet<string> _generated = new HashSet<string>();

        public void Preload(IList<string> idValues)
        {
            foreach (var value in idValues)
            {
                _generated.Add(value);
            }
        }

        public string NextId()
        {
            string id;
            do
            {
                id = NextWord();
            } while (_generated.Contains(id));

            return id;
        }

        private string NextWord()
        {
            string word = "";
            while (word.Length < IdLength)
            {
                word += NextLetter();
            }
            return word;
        }

        private string NextLetter()
        {
            var index = _rand.Next(0, _alphabet.Length);
            return _alphabet.Substring(index, 1);
        }
    }
}
