﻿namespace Talabat.APIs.Errors
{
	public class ApiResponse
	{
		public ApiResponse(int statusCode, string? message = null)
		{ 
			StatusCode = statusCode;
			Message = message ?? GetDefaultMessageForStatusCode(statusCode);
		}

		private string? GetDefaultMessageForStatusCode(int statusCode)
		{
			return statusCode switch
			{
				400 => "Bad Request",
				401 => "You Are Not Authorized",
				404 => "Resourse Not Found",
				500 => "Internal Server Error",
				_=>null
			};
		}

		public int StatusCode { get; set; }

		public string? Message { get; set; }

	}
}
