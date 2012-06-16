using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Brass9.Web.Documents.Barcode
{
	public class Barcode3of9
	{
		#region Singleton
		// http://codereview.stackexchange.com/questions/79/implementing-a-singleton-pattern-in-c
		public static Barcode3of9 Current { get { return Nested.instance; } }

		class Nested
		{
			static Nested()
			{
			}

			internal static readonly Barcode3of9 instance = new Barcode3of9();
		}
		#endregion

		/// <summary>
		/// A sparse array of strings indexed by charcode. For example the string for "A" is at codes["A"].
		/// Only uppercase letters are encoded.
		/// 
		/// Lookup is simply codes[c.ToUpper()]
		/// </summary>
		protected string[] codes;

		protected Barcode3of9()
		{
			codes = new string[91];

            codes['0'] = "n n n w w n w n n";
            codes['1'] = "w n n w n n n n w";
            codes['2'] = "n n w w n n n n w";
            codes['3'] = "w n w w n n n n n";
            codes['4'] = "n n n w w n n n w";
            codes['5'] = "w n n w w n n n n";
            codes['6'] = "n n w w w n n n n";
            codes['7'] = "n n n w n n w n w";
            codes['8'] = "w n n w n n w n n";
            codes['9'] = "n n w w n n w n n";
            codes['A'] = "w n n n n w n n w";
            codes['B'] = "n n w n n w n n w";
            codes['C'] = "w n w n n w n n n";
            codes['D'] = "n n n n w w n n w";
            codes['E'] = "w n n n w w n n n";
            codes['F'] = "n n w n w w n n n";
            codes['G'] = "n n n n n w w n w";
            codes['H'] = "w n n n n w w n n";
            codes['I'] = "n n w n n w w n n";
            codes['J'] = "n n n n w w w n n";
            codes['K'] = "w n n n n n n w w";
            codes['L'] = "n n w n n n n w w";
            codes['M'] = "w n w n n n n w n";
            codes['N'] = "n n n n w n n w w";
            codes['O'] = "w n n n w n n w n";
            codes['P'] = "n n w n w n n w n";
            codes['Q'] = "n n n n n n w w w";
            codes['R'] = "w n n n n n w w n";
            codes['S'] = "n n w n n n w w n";
            codes['T'] = "n n n n w n w w n";
            codes['U'] = "w w n n n n n n w";
            codes['V'] = "n w w n n n n n w";
            codes['W'] = "w w w n n n n n n";
            codes['X'] = "n w n n w n n n w";
            codes['Y'] = "w w n n w n n n n";
            codes['Z'] = "n w w n w n n n n";
            codes['-'] = "n w n n n n w n w";
            codes['.'] = "w w n n n n w n n";
            codes[' '] = "n w w n n n w n n";
            codes['*'] = "n w n n w n w n n";
            codes['$'] = "n w n w n w n n n";
            codes['/'] = "n w n w n n n w n";
            codes['+'] = "n w n n n w n w n";
			codes['%'] = "n n n w n w n w n";
		}

		public string BarcodeHtml(string input)
		{
			return BarcodeHtmlBare("*" + input + "*");
		}

		public string BarcodeHtmlBare(string input)
		{
			// Input strings should only be coming from the hardcoded set in this class - no input validation
			/*
			if (String.IsNullOrEmpty(input))
				return input;
			*/

			string lineN = "<i></i>";
			string lineW = "<b></b>";
			string lineN1 = "<i class=first></i>";	// To handle the fence-post problem
			string lineW1 = "<b class=first></b>";
			//string gapN = "";
			string gapW = "<em></em>";

			/*
			Nearly all symbols have 5 lines and one wide gap.
			35 + 9 = 44 chars for most symbols.
			
			A few symbols are longer with several wide gaps, so we budget 60 to be safe.
			~30 for the wrapping tag.
			*/
			var sb = new StringBuilder(30 + 60 * input.Length);
			int iInput = 0;
			int iMask = 0;

			// Wrapping tag, span.barcode
			sb.Append("<span class=barcode>");

			// Get first symbol mask
			string mask = codes[input[iInput]];

			// Append first HTML tag from first mask - needs a special class=first applied
			sb.Append(mask[iMask] == 'n' ? lineN1 : lineW1);

			// We use this body twice, once for the first symbol and once for the remaining symbols; abstract it
			var maskLoopBody = new Action<string, int>((_mask, _iMask) =>
			{
				if (_iMask % 4 == 0)
				{
					// Draw a line, narrow (n) or wide (w)
					sb.Append(_mask[_iMask] == 'n' ? lineN : lineW);
				}
				else
				{
					// Draw a space
					if (_mask[_iMask] == 'w')
						sb.Append(gapW);
					// else
						//sb.Append(gapN);	// empty string
				}
			});

			// Finish writing out tags for first symbol, using first mask
			for (iMask += 2; iMask < mask.Length; iMask += 2)
			{
				maskLoopBody(mask, iMask);
			}

			// Begin the generalized loop over input characters and symbol masks, starting with input char 2
			for (iInput++; iInput < input.Length; iInput++)
			{
				mask = codes[input[iInput]];

				for (iMask = 0; iMask < mask.Length; iMask += 2)
				{
					maskLoopBody(mask, iMask);
				}
			}

			sb.Append("</span>");

			return sb.ToString();
		}

		/// <summary>
		/// Immutable
		/// </summary>
		public class Barcode3of9Settings
		{
			public string NarrowWidth { get; protected set; }
			public string WideWidth { get; protected set; }
			public string Height { get; protected set; }

			/// <summary>
			/// Generated. The CSS rules to use on your page based on these settings.
			/// Place inside a style tag, or as the contents of an external stylesheet.
			/// </summary>
			public string Styles { get; protected set; }

			public Barcode3of9Settings(string narrowWidth, string wideWidth, string height)
			{
				NarrowWidth = narrowWidth;
				WideWidth = wideWidth;
				Height = height;
				initCss();
			}

			public static Barcode3of9Settings Default = new Barcode3of9Settings("2px", "6px", "50px");

			protected void initCss()
			{
				string css =
@"
span.barcode, span.barcode i, span.barcode b, span.barcode em {
	display: inline-block;
	height: {2};
}

span.barcode i, span.barcode b {
	background: #000;
}

span.barcode b, span.barcode em {
	width: {1};
}

span.barcode i {
	width: {0};
}

span.barcode em {
	margin-right: -{0};
}

span.barcode i, span.barcode b {
	margin-left: {0};
}

span.barcode i.first, span.barcode b.first {
	margin-left: 0;
}
";

				// Avoid having to use {{ to escape braces in source CSS
				var regex = new Regex(@"\{([^\d])");
				css = regex.Replace(css, "{{$1");	// for String.Format http://msdn.microsoft.com/en-us/library/txafckwd.aspx
				regex = new Regex(@"([^\d])\}");
				css = regex.Replace(css, "$1}}");

				Styles = String.Format(css, NarrowWidth, WideWidth, Height);
			}
		}
	}
}
