# BarcodeToHtml #
(Brass9.Web.Documents.Barcode)

This library generates simple HTML and CSS to render a barcode on a webpage. The HTML and CSS used are intentionally very simple to achieve the broadest support possible between browsers and other downstream software that might be used to convert the HTML to some other format, like a PDF.

## Usage ##
A barcode has 2 parts: The CSS and the HTML.

**CSS**<br/>
The CSS only needs to be specified once (in the head tags, ideally) regardless of how many barcodes there are on a page. To get the default CSS on a page call:

    Brass9.Web.Documents.Barcode.Barcode3of9.Barcode3of9Settings.Default.Styles

This will return a string with the default CSS styles for a typical bar code - 2 pixels wide for narrow, 6 pixels wide for wide, and 50 pixels tall.

You can customize this with your own settings:

    var settings = new  Brass9.Web.Documents.Barcode.Barcode3of9.Barcode3of9Settings(
        narrowWidth: ".015in",
		wideWidth: ".045in",
		height: ".4in"
	);
    settings.Styles;


The last line returns the custom styles you just built.

**HTML**<br/>
For each barcode, call:

    Brass9.Web.Documents.Barcode.Barcode3of9.Current.BarcodeHtml("12345");

Obviously you should replace "12345" with whatever you want to make a barcode out of.

## Capability ##

For now, only standard barcode characters are supported. These are uppercase characters (no lowercase), numbers, and 8 special characters:

    -. *$/+%

So dash, period, space, asterisk, dollar sign, forward slash, plus, and percent. These special characters are used in the 3 of 9 format to double-character encode the rest of the ASCII alphabet. It should be relatively simple to add support for the wider ASCII alphabet and this double encoding, but no support is currently planned.

## Generated Code ##
The HTML might generated might seem a little odd. To keep the underlying HTML for the various bars short, short tags supported since HTML3.01 are used to represent narrow bars (`<i>`), wide bars (`<b>`), and wide gaps (`<em>`). Using tags with short names avoids bloating the HTML into something massive. Using tags that would normally be display: inline allows IE6/7 to apply inline-block to them, which is necessary for rendering blocks along-side each other.

Likewise the CSS selectors applied are very simple - they stick to the basic tags and classes that have been supported since CSS1 days. If a library you use supports even the most basic HTML and CSS, it should be able to handle the output of this library.