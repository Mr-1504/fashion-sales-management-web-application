using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Text;

namespace BTL_LTWeb.Services
{
    public class VietQrGenerator
    {
        public static Bank GetBankList()
        {
            using (WebClient webClient = new WebClient())
            {
                var htmlData = webClient.DownloadData("https://api.vietqr.io/v2/banks");
                var bankListData = Encoding.UTF8.GetString(htmlData);
                return JsonConvert.DeserializeObject<Bank>(bankListData);
            }
        }

        public static string GetQR(int money, string infor)
        {
            var apiRequest = new ApiRequest
            {
                accountNo = 104876346894,
                accountName = "TRUONG VAN MINH",
                acqId = 970415,
                amount = money,
                addInfo = infor,
                format = "text",
                template = "compact"
            };

            var json = JsonConvert.SerializeObject(apiRequest);

            var client = new RestClient("https://api.vietqr.io/v2/generate");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Accept", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = client.Execute(request);
            var content = response.Content;
            
            var data = JsonConvert.DeserializeObject<ApiResponse>(content);

            return data.data.qrDataURL;
        }
    }
}
