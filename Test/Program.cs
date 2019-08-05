
using Flurl;
using Flurl.Http;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var body = new {
                grant_type = "authorization_code",
                code = "702",
                client_id = "42",
                redirect_uri = "http://localhost:10101/",
                code_verifier = "teGDCjiuMgMGd0zUapcD156ZH0WwlUWB"
            };

            var response = "https://api.cesium.com/"
                .AppendPathSegments("oauth", "token")
                .WithHeader("Accept", "application/json")
                .PostJsonAsync(body)
                .Result;

            Console.WriteLine(response);
            Console.ReadKey();
        }
    }
}
