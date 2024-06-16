using AutoMapper;
using Dadata.Model;

namespace AddressCleanerAM
{
	public class AddressProfile : Profile
	{
		public AddressProfile()
		{
			CreateMap<Address, CleanAddress>();
		}
	}
}
