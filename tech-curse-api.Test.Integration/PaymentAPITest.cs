using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Domain.Enums;

namespace ScreenSound.Tests.Integracao;

[Trait("Categoria", "Integração")]
public class PaymentAPITest
{
    private static HttpClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> handler, Uri? baseAddress = null)
    {
        var messageHandler = new StubHttpMessageHandler(handler);
        var client = new HttpClient(messageHandler);
        if (baseAddress != null)
            client.BaseAddress = baseAddress;
        return client;
    }

    [Fact(DisplayName = "GET /payments should return list of payments")]
    public async Task ListPayments_ReturnsOkAndPayments()
    {
        // Arrange
        IEnumerable<Payment> payments = new List<Payment>()
        {
            new Payment() { PaymentId = 1, EnrollmentId = 1, StudentId = 1, Amount = 100.50m, Status = PaymentStatus.Paid, IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-5), PaidAt = null, ExternalTransactionId = "PAID_123124123321" },
            new Payment() { PaymentId = 2, EnrollmentId = 2, StudentId = 2, Amount = 250.75m, Status = PaymentStatus.Pending, IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-3), PaidAt = null, ExternalTransactionId = "PAID_123124123321" },
            new Payment() { PaymentId = 3, EnrollmentId = 3, StudentId = 3, Amount = 75.00m, Status = PaymentStatus.Failed, IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-1), PaidAt = null, ExternalTransactionId = "PAID_123124123321" }
        };

        var paymentsOutputDto = payments.Select(p => new PaymentOutputDto(
            p.PaymentId,
            p.EnrollmentId,
            p.StudentId,
            p.Amount,
            p.Status,
            p.IsActive,
            p.CreatedAt,
            p.PaidAt,
            p.ExternalTransactionId
        ));

        var responseObject = new PagedResultDto<PaymentOutputDto>(
            paymentsOutputDto,
            paymentsOutputDto.Count(),
            1,
            10
        );

        var handler = new Func<HttpRequestMessage, HttpResponseMessage>(request =>
        {
            if (request.Method == HttpMethod.Get && request.RequestUri!.PathAndQuery == "/tech-curse/Payment")
            {
                var json = JsonSerializer.Serialize(responseObject);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        var httpClient = CreateClient(handler, new Uri("http://localhost/"));

        //Act
        var response = await httpClient.GetAsync("/tech-curse/Payment");

        //Assert
        Assert.NotNull(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pagedResponse = JsonSerializer.Deserialize<PagedResultDto<PaymentOutputDto>>(
            response.Content.ReadAsStringAsync().Result,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.Equal(paymentsOutputDto.Count(), pagedResponse.TotalCount);

        var responseContent = pagedResponse.Items;

        for (int i = 0; i < paymentsOutputDto.Count(); i++)
        {
            Assert.Equal(paymentsOutputDto.ElementAt(i).PaymentId, responseContent.ElementAt(i).PaymentId);
            Assert.Equal(paymentsOutputDto.ElementAt(i).EnrollmentId, responseContent.ElementAt(i).EnrollmentId);
            Assert.Equal(paymentsOutputDto.ElementAt(i).StudentId, responseContent.ElementAt(i).StudentId);
            Assert.Equal(paymentsOutputDto.ElementAt(i).Amount, responseContent.ElementAt(i).Amount);
            Assert.Equal(paymentsOutputDto.ElementAt(i).Status, responseContent.ElementAt(i).Status);
            Assert.Equal(paymentsOutputDto.ElementAt(i).IsActive, responseContent.ElementAt(i).IsActive);
            Assert.Equal(paymentsOutputDto.ElementAt(i).CreatedAt, responseContent.ElementAt(i).CreatedAt);
            Assert.Equal(paymentsOutputDto.ElementAt(i).PaidAt, responseContent.ElementAt(i).PaidAt);
            Assert.Equal(paymentsOutputDto.ElementAt(i).ExternalTransactionId, responseContent.ElementAt(i).ExternalTransactionId);
        }

        Assert.Equal(1, pagedResponse.PageNumber);
        Assert.Equal(10, pagedResponse.PageSize);
        Assert.Equal(1, pagedResponse.TotalPages);
    }

    private sealed class FuncHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
        public FuncHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_responder(request));
    }
}