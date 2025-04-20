namespace HbysApi.Application.Dtos;

// ContactDto for transferring contact data
// İletişim verilerini taşımak için ContactDto
public class ContactDto
{
    // Name of the unit
    // Birimin adı
    public required string BirimAdi { get; set; }
    // Phone number
    // Telefon numarası
    public required string PhoneNumber { get; set; }
}