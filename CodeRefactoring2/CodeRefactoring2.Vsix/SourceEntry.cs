using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CodeRefactoring2.Vsix
{
    [DebuggerDisplay("{FileHash}, {LineNumber}, {LineOffset}, {ThisHash},{NextHash}, {PreviousHash}")]
    [Serializable]
    public class SourceEntry
    {
        public SourceEntry()
        {
            
        }
        /// <inheritdoc />
        public SourceEntry(int fileHash, int lineNumber, int lineOffset, int thisHash, int previousHash, int nextHash)
        {
            FileHash = fileHash;
            LineNumber = lineNumber;
            LineOffset = lineOffset;
            PreviousHash = previousHash;
            NextHash = nextHash;
            ThisHash = thisHash;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(FileHash)}: {FileHash}, {nameof(LineNumber)}: {LineNumber}, {nameof(LineOffset)}: {LineOffset}, {nameof(ThisHash)}: {ThisHash}, {nameof(PreviousHash)}: {PreviousHash}, {nameof(NextHash)}: {NextHash}";
        }

        [XmlAttribute("FH")]
        public int FileHash { get; set; }
        [XmlAttribute("LN")]
        public int LineNumber { get; set; }

        protected bool Equals(
            SourceEntry other)
        {
            return FileHash == other.FileHash && LineNumber == other.LineNumber && LineOffset == other.LineOffset && ThisHash == other.ThisHash && PreviousHash == other.PreviousHash && NextHash == other.NextHash;
        }

        public override bool Equals(
            object obj)
        {
            if (ReferenceEquals(
                null,
                obj)) return false;
            if (ReferenceEquals(
                this,
                obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SourceEntry)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FileHash;
                hashCode = (hashCode * 397) ^ LineNumber;
                hashCode = (hashCode * 397) ^ LineOffset;
                hashCode = (hashCode * 397) ^ ThisHash;
                hashCode = (hashCode * 397) ^ PreviousHash;
                hashCode = (hashCode * 397) ^ NextHash;
                return hashCode;
            }
        }

        [XmlAttribute("LO")]
        public int LineOffset { get; set; }
        [XmlAttribute("TH")]
        public int ThisHash { get; set; }
        [XmlAttribute("PH")]
        public int PreviousHash { get; set; }
        [XmlAttribute("NH")]
        public int NextHash { get; set; }
    }
}
