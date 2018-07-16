using System;

internal struct JsonDateTime
{
	public long Value;

	public static implicit operator DateTime(JsonDateTime jdt)
	{
		return DateTime.FromFileTimeUtc(jdt.Value);
	}

	public static implicit operator JsonDateTime(DateTime dt)
	{
		var jdt = new JsonDateTime {Value = dt.ToFileTimeUtc()};
		return jdt;
	}
}