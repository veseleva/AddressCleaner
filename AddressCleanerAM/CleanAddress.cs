namespace AddressCleanerAM
{
	public class CleanAddress
	{
		public string? result { get; set; }

		public string? postal_code { get; set; }

		public string? country { get; set; }		
		
		public string? country_iso_code { get; set; }	
		
		public string? federal_district { get; set; }	
		
		public string? region_with_type { get; set; }

		public string? area_with_type { get; set; }	

		public string? sub_area_with_type { get; set; }

		public string? city_with_type { get; set; }

		public string? settlement_with_type { get; set; }

		public string? street_with_type { get; set; }

		public string? house_with_type { get; set; }

		public string? unparsed_parts { get; set; }
	}
}
