
namespace HolzShots.IO.Naming
{
    class TextPatternItem : PatternItem
    {
        public override string Keyword => "text";

        public TextPatternItem(string? propertyName)
            : base(propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
        }

        public override string TextRepresentation => PropertyName!;
        public override bool IsValid => !string.IsNullOrEmpty(PropertyName) && !PropertyName.ContainsInvalidChars();
        public override string FormatMetadata(FileMetadata metadata) => PropertyName.SanitizeFileName(string.Empty);
    }
}
