using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.SK.Dtos
{
    public class TextProcessingOptions
    {
        public int SentencesPerBlock { get; set; } = 4;
        public char[] SentenceSeparators { get; set; } = { '。', '！', '？' };
        public int MinSentenceLength { get; set; } = 3;
        public bool ForceSplitAtImages { get; set; } = true;
    }
}
