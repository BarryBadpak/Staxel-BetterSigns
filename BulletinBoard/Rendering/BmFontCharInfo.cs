namespace BulletinBoard.Rendering
{
	internal class BmFontCharInfo
	{
		public readonly char Char;
		public readonly int FirstVertexIndex;
		public readonly float XOffset;
		public readonly float YOffset;
		public readonly float Width;
		public readonly float Height;
		public readonly float XAdvance;
		public int LineNumber;
		public int WordNumber;

		public BmFontCharInfo(char character, int firstVertexIndex, float xOffset, float yOffset, float width, float height, float xAdvance, int lineNumber, int wordNumber)
		{
			this.Char = character;
			this.FirstVertexIndex = firstVertexIndex;
			this.XOffset = xOffset;
			this.YOffset = yOffset;
			this.Width = width;
			this.Height = height;
			this.XAdvance = xAdvance;
			this.LineNumber = lineNumber;
			this.WordNumber = wordNumber;
		}
	}
}
