using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace nHangman
{
    public class HangmanGame
    {
        private List<PosChar> currentWord = new List<PosChar>();
        private string currentWordStr;
        private List<char> guessedChars = new List<char>();
        private List<char> failGuessedChars = new List<char>();
        private int guesses = 0;
        private int failGuesses = 0;
        private char labelEmpty = '_';
        private char labelDivider = '/';
        private int maximumFailGuesses = 10;

        public bool StartGame(string word)
        {
            return this.StartGame(word, 10);
        }

        public bool StartGame(string word, int maxFailGuesses)
        {
            if (string.IsNullOrWhiteSpace(word) == true)
            {
                return false;
            }
            if (maxFailGuesses < 1)
            {
                maxFailGuesses = 1;
            }
            this.maximumFailGuesses = maxFailGuesses;
            word = word.ToUpper();

            // remove all unwanted chars
            word = Regex.Replace(word, @"[^A-Z\s]", "");
            word = Regex.Replace(word, @"([\s]{2,})", " ");

            this.guesses = 0;
            this.failGuesses = 0;
            this.guessedChars.Clear();
            this.failGuessedChars.Clear();
            this.currentWord.Clear();
            this.currentWordStr = word;

            foreach (char c in word)
            {
                this.currentWord.Add(new PosChar()
                {
                    Character = c,
                    IsGuessed = false,
                    IsSpace = char.IsWhiteSpace(c)
                });
            }

            return true;
        }

        public GuessResultState Guess(char c)
        {
            if (this.FailGuesses >= this.MaximumFailGuesses)
            {
                return GuessResultState.NoMoreGuesses;
            }
            c = char.ToUpper(c);
            if (this.guessedChars.Contains(c) == true)
            {
                return GuessResultState.Success;
            }
            if (this.failGuessedChars.Contains(c) == true)
            {
                return GuessResultState.Failed;
            }
            this.guesses++;
            var foundChars = from pc
                             in this.currentWord
                             where pc.Character == c
                             select pc;
            if (foundChars.Count() > 0)
            {
                this.guessedChars.Add(c);
                this.guesses++;
                foreach (PosChar pc in foundChars)
                {
                    pc.IsGuessed = true;
                }
                return GuessResultState.Success;
            }
            this.failGuessedChars.Add(c);
            this.failGuesses++;
            return GuessResultState.Failed;
        }

        public GuessResultState GuessSolution(string s)
        {
            if (this.FailGuesses >= this.MaximumFailGuesses)
            {
                return GuessResultState.NoMoreGuesses;
            }
            if (s == null)
            {
                this.failGuesses++;
                return GuessResultState.Failed;
            }
            s = s.Trim().ToUpper();
            if (s == this.currentWordStr)
            {
                this.guesses++;
                foreach (PosChar pc in this.currentWord)
                {
                    if (pc.IsSpace == false)
                    {
                        pc.IsGuessed = true;
                    }
                }
                return GuessResultState.Success;
            }
            this.failGuesses++;
            return GuessResultState.Failed;
        }

        public int Guesses
        {
            get
            {
                return this.guesses;
            }
        }

        public int FailGuesses
        {
            get
            {
                return this.failGuesses;
            }
        }

        public char LabelEmpty
        {
            set
            {
                this.labelEmpty = value;
            }
            get
            {
                return this.labelEmpty;
            }
        }

        public char LabelDivider
        {
            set
            {
                this.labelDivider = value;
            }
            get
            {
                return this.labelDivider;
            }
        }

        public int MaximumFailGuesses
        {
            set
            {
                if (value > 0)
                {
                    this.maximumFailGuesses = value;
                }
                else
                {
                    this.maximumFailGuesses = 1;
                }
            }
            get
            {
                return this.maximumFailGuesses;
            }
        }

        public string GetLabel(bool insertSpace)
        {
            StringBuilder buffer = new StringBuilder(this.currentWord.Count);
            foreach (PosChar pc in this.currentWord)
            {
                if (pc.IsGuessed == true)
                {
                    buffer.Append(pc.Character);
                    if (insertSpace)
                    {
                        buffer.Append(' ');
                    }
                }
                else if (pc.IsSpace == true)
                {
                    buffer.Append(this.LabelDivider);
                    if (insertSpace)
                    {
                        buffer.Append(' ');
                    }
                }
                else
                {
                    buffer.Append(this.LabelEmpty);
                    if (insertSpace)
                    {
                        buffer.Append(' ');
                    }
                }
            }
            return buffer.ToString();
        }

        public string CurrentWord
        {
            get
            {
                return this.currentWordStr;
            }
        }

        public IEnumerable<char> GuessedChars
        {
            get
            {
                return this.guessedChars.AsReadOnly();
            }
        }

        public IEnumerable<char> FailGuessedChars
        {
            get
            {
                return this.failGuessedChars.AsReadOnly();
            }
        }

        public bool IsGuessed
        {
            get
            {
                return (this.currentWord.All(pc => (pc.IsSpace) ? true : pc.IsGuessed));
            }
        }

        public bool IsGameOver
        {
            get
            {
                return (this.FailGuesses >= this.MaximumFailGuesses);
            }
        }

    }

    class PosChar
    {

        public char Character { set; get; }
        public bool IsGuessed { set; get; }
        public bool IsSpace { set; get; }

    }

    public enum GuessResultState
    {
        Success, Failed, NoMoreGuesses
    }
}
