using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Data.Utilities
{
	public static class TextUtility
	{
		public static string ToSentenceCase(string text)
		{
			if (string.IsNullOrWhiteSpace(text)) return text;

			var words = text.ToLower().Split(' ');
			words[0] = char.ToUpper(words[0][0]) + words[0].Substring(1);

			return string.Join(" ", words);
		}

        public static string ToTitleCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var words = text.ToLower().Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }

            return string.Join(" ", words);
        }
    }

}
