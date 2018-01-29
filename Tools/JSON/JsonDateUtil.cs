using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct JsonDateTime
{
	public long value;
	public static implicit operator DateTime(JsonDateTime jdt)
	{
		return DateTime.FromFileTimeUtc(jdt.value);
	}

	public static implicit operator JsonDateTime(DateTime dt)
	{
		JsonDateTime jdt = new JsonDateTime();
		jdt.value = dt.ToFileTimeUtc();
		return jdt;
	}
}