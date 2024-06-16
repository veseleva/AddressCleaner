using System.Text;
using Newtonsoft.Json;
using Dadata.Model;
using static System.Net.Mime.MediaTypeNames;
using AutoMapper;

namespace AddressCleanerAM
{
	public class AddressCleanerService
	{
		private readonly string _remoteServiceUrl;
		private readonly HttpClient _httpClient;
		private readonly IMapper _mapper;

        public AddressCleanerService(HttpClient httpClient, IMapper mapper)
        {
			_remoteServiceUrl = httpClient.BaseAddress.ToString();
			_httpClient = httpClient;
            _mapper = mapper;
        }

		public async Task<CleanAddress> GetCleanAddressAsync(string address, CancellationToken cancellationToken)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, _remoteServiceUrl))
            {
                var data = new string[] { address };

                //Староверские юзинги
                using (var httpContent = new StringContent(
                                                JsonConvert.SerializeObject(data),
                                                Encoding.UTF8,
                                                Application.Json))
                {

                    httpRequest.Content = httpContent;
                    using (var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken))
                    {
						httpResponse.EnsureSuccessStatusCode();

                        var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                        var list = JsonConvert.DeserializeObject<IList<Address>>(responseString);

						return _mapper.Map<CleanAddress>(list?[0]);
					}
                }
            }
        }
	}
}
