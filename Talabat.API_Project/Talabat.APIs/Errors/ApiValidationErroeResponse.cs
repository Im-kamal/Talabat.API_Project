namespace Talabat.APIs.Errors
{
	public class ApiValidationErroeResponse : ApiResponse
	{
        public ApiValidationErroeResponse():base(400)
        {
            Errors=new List<string>();
        }
        public IEnumerable<string> Errors { get; set; }


    }
}
