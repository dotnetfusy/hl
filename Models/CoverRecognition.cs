using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteka1.Models
{
    public class CoverRecognition
    {
        public Guid Id { get; set; }
    }

    public class Rootobject
    {
        public string Status { get; set; }
        public Recognitionresult RecognitionResult { get; set; }
    }

    public class Recognitionresult
    {
        public Line[] Lines { get; set; }
    }

    public class Line
    {
        public int[] BoundingBox { get; set; }
        public string Text { get; set; }
        public Word[] Words { get; set; }
    }

    public class Word
    {
        public int[] BoundingBox { get; set; }
        public string Text { get; set; }
    }
}
