﻿namespace EazyQuizy.Common.Extensions;

public static class StringExtensions
{
	public static bool IsNullOrEmpty(this string? value)
	{
		return string.IsNullOrEmpty(value);
	}
}