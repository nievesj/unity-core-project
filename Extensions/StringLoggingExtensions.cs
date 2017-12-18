using System;

public static class StringLoggingExtensions
{
	/// <summary>
	/// Sets the color of the text according to the parameter value.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="color">Color.</param>
	public static string Colored(this string message, Colors color)
	{
		return string.Format("<color={0}>{1}</color>", color.ToString(), message);
	}

	/// <summary>
	/// Sets the color of the text according to the traditional HTML format parameter value.
	/// </summary>
	/// <param name="message">Message</param>
	/// <param name="color">Color</param>
	public static string Colored(this string message, string colorCode)
	{
		return string.Format("<color={0}>{1}</color>", colorCode, message);
	}

	/// <summary>
	/// Sets the size of the text according to the parameter value, given in pixels.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="size">Size.</param>
	public static string Sized(this string message, int size)
	{
		return string.Format("<size={0}>{1}</size>", size, message);
	}

	/// <summary>
	/// Renders the text in boldface.
	/// </summary>
	/// <param name="message">Message.</param>
	public static string Bold(this string message)
	{
		return string.Format("<b>{0}</b>", message);
	}

	/// <summary>
	/// Renders the text in italics.
	/// </summary>
	/// <param name="message">Message.</param>
	public static string Italics(this string message)
	{
		return string.Format("<i>{0}</i>", message);
	}
}

public enum Colors
{
	aqua,
	black,
	blue,
	brown,
	cyan,
	darkblue,
	fuchsia,
	green,
	grey,
	lightblue,
	lime,
	magenta,
	maroon,
	navy,
	olive,
	purple,
	red,
	silver,
	teal,
	white,
	yellow
}