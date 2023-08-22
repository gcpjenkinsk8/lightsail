using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DARRefDataTest
{
    public class CallBitcoinNode
    {

        public static async void get_best_block_hash()
        {
            try
            {
                Console.WriteLine("Hello");
                HttpClient client = new HttpClient();

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:8332/");

                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("myusername:")));

                request.Content = new StringContent("{\"jsonrpc\": \"1.0\", \"id\": \"curltest\", \"method\": \"getbestblockhash\", \"params\": []}");
                //request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain;");

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("INFO: ******************");
                Console.WriteLine(responseBody);
                Console.WriteLine("INFO: ******************");

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:******************");
                Console.WriteLine(ex.Message);
                Console.WriteLine("ERROR:******************");
            }
        }
    }
}
