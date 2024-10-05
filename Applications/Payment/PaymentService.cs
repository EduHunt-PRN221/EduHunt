using Eduhunt.Data;
using Net.payOS;
using Net.payOS.Types;

namespace Eduhunt.Applications.Payment
{
    public class PaymentService
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PayOS _payOS;
        private readonly IConfiguration _configuration;
        public PaymentService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
             IConfiguration configuration
            )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

            string clientID = _configuration["Environment:PAYOS_CLIENT_ID"] ?? throw new ArgumentNullException(nameof(clientID));
            string apiKey = _configuration["Environment:PAYOS_API_KEY"] ?? throw new ArgumentNullException(nameof(apiKey));
            string checksumKey = _configuration["Environment:PAYOS_CHECKSUM_KEY"] ?? throw new ArgumentNullException(nameof(checksumKey));

            _payOS = new PayOS(clientID, apiKey, checksumKey);
        }

        public async Task<string> PaymentProcessing(string cancelURL, string successURL,
            string productName,int productQuantity,int productPrice)
        {
            try
            {
                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                ItemData item = new ItemData(productName, productQuantity, productPrice);
                List<ItemData> items = new List<ItemData>();
                items.Add(item);

                PaymentData paymentData = new PaymentData(orderCode, productPrice * productQuantity, "Pay to be VIP", items, cancelURL, successURL);

                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

                return createPayment.checkoutUrl;
            }
            catch (System.Exception exception)
            {
                Console.WriteLine(exception);
                return cancelURL;
            }
        }
    }
}
